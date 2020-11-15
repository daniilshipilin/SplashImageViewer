namespace SplashImageViewer.Forms
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using SplashImageViewer.Helpers;
    using SplashImageViewer.Models;
    using SplashImageViewer.Properties;

    public partial class FullscreenForm : Form
    {
        private readonly Timer hideInfoLabelTimer = new Timer();
        private readonly Timer hideBottomLabelsTimer = new Timer();
        private readonly Screen screen;
        private readonly bool slideshowIsEnabled;

        public FullscreenForm(Screen screen, bool slideshowIsEnabled)
        {
            InitializeComponent();
            this.screen = screen;
            this.slideshowIsEnabled = slideshowIsEnabled;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                case Keys.F:
                    Close();
                    break;

                case Keys.Left:
                case Keys.Down:
                    ImagesModel.Singleton.SelectPreviousImageIndex();
                    RestartHideBottomLabelsTimer();
                    break;

                case Keys.Right:
                case Keys.Up:
                    ImagesModel.Singleton.SelectNextImageIndex();
                    RestartHideBottomLabelsTimer();
                    break;

                case Keys.R:
                    ImagesModel.Singleton.SelectRandomImageIndex();
                    RestartHideBottomLabelsTimer();
                    break;

                default:
                    RestartHideInfoLabeTimer();
                    RestartHideBottomLabelsTimer();
                    Update();
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components is not null))
            {
                // dispose managed resources
                components.Dispose();
            }

            fullscreenPictureBox.Image.Dispose();

            hideInfoLabelTimer.Tick -= HideInfoLabel;
            hideBottomLabelsTimer.Tick -= HideBottomLabels;
            ImagesModel.Singleton.CurrentFilePathIndexChanged -= UpdatePictureBoxEvent;
            hideInfoLabelTimer.Dispose();
            hideBottomLabelsTimer.Dispose();

            // free native resources
            base.Dispose(disposing);
        }

        private void FullscreenForm_Load(object sender, EventArgs e)
        {
            LocalizeUIElements();

            totalFilesLabel.ForeColor = Color.FromArgb(AppSettings.LabelsColorArgb);
            infoLabel.ForeColor = Color.FromArgb(AppSettings.LabelsColorArgb);
            filePathLabel.ForeColor = Color.FromArgb(AppSettings.LabelsColorArgb);

            // Bounds = Screen.PrimaryScreen.Bounds;
            Bounds = screen.Bounds; // use passed-in screen reference bounds

            fullscreenPictureBox.Image = ImagesModel.Singleton.Image;
            fullscreenPictureBox.BackColor = Color.FromArgb(AppSettings.ThemeColorArgb);

            CheckFormSize();

            // change totalFilesLabel parent
            totalFilesLabel.Parent = fullscreenPictureBox;

            // totalFilesLabel.Location = new Point(fullscreenPictureBox.Size.Width - totalFilesLabel.Size.Width,
            //                                     fullscreenPictureBox.Size.Height - totalFilesLabel.Size.Height);

            // change infoLabel parent
            infoLabel.Parent = fullscreenPictureBox;

            // change infoLabel parent
            filePathLabel.Parent = fullscreenPictureBox;
            filePathLabel.Text = ImagesModel.Singleton.CurrentFilePath;

            ImagesModel.Singleton.CurrentFilePathIndexChanged += UpdatePictureBoxEvent;

            UpdateBottomLabels();
            InitTimers();
        }

        private void LocalizeUIElements()
        {
            infoLabel.Text = slideshowIsEnabled ?
                $"{Strings.SlideshowInfoLabel}{Environment.NewLine}{Strings.SlideshowEnabled}" :
                Strings.SlideshowInfoLabel;
        }

        private void InitTimers()
        {
            hideInfoLabelTimer.Tick += HideInfoLabel;
            hideInfoLabelTimer.Interval = AppSettings.FullscreenFormHideInfoTimerIntervalMs;
            hideInfoLabelTimer.Start();

            hideBottomLabelsTimer.Tick += HideBottomLabels;
            hideBottomLabelsTimer.Interval = AppSettings.FullscreenFormHideInfoTimerIntervalMs;
            hideBottomLabelsTimer.Start();
        }

        private void RestartHideInfoLabeTimer()
        {
            infoLabel.Visible = true;
            hideInfoLabelTimer.Stop();
            hideInfoLabelTimer.Start();
        }

        private void RestartHideBottomLabelsTimer()
        {
            filePathLabel.Visible = true;
            totalFilesLabel.Visible = true;
            hideBottomLabelsTimer.Stop();
            hideBottomLabelsTimer.Start();
        }

        private void HideInfoLabel(object? sender, EventArgs e)
        {
            infoLabel.Visible = false;
            hideInfoLabelTimer.Stop();
            Update();
        }

        private void HideBottomLabels(object? sender, EventArgs e)
        {
            filePathLabel.Visible = false;
            totalFilesLabel.Visible = false;
            hideBottomLabelsTimer.Stop();
            Update();
        }

        private void UpdatePictureBoxEvent(object sender)
        {
            // check if caller is on a different thread (invoke required)
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(UpdatePictureBox));
            }
            else
            {
                UpdatePictureBox();
            }
        }

        private void UpdatePictureBox()
        {
            if (ImagesModel.Singleton.FilePaths.Count == 0)
            {
                Close();
            }

            ImagesModel.Singleton.LoadImage();

            if (ImagesModel.Singleton.Image is not null)
            {
                fullscreenPictureBox.Image = ImagesModel.Singleton.Image;
                CheckFormSize();
                UpdateBottomLabels();
            }
        }

        private void CheckFormSize()
        {
            if (fullscreenPictureBox.ClientSize.Width < fullscreenPictureBox.Image.Width ||
                fullscreenPictureBox.ClientSize.Height < fullscreenPictureBox.Image.Height)
            {
                fullscreenPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                fullscreenPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            }
        }

        private void UpdateBottomLabels()
        {
            totalFilesLabel.Text = $"{ImagesModel.Singleton.CurrentFilePathIndex + 1} / {ImagesModel.Singleton.FilePaths.Count}";
            filePathLabel.Text = ImagesModel.Singleton.CurrentFilePath;
        }
    }
}
