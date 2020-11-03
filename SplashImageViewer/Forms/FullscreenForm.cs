namespace SplashImageViewer.Forms
{
    using System;
    using System.Windows.Forms;
    using SplashImageViewer.Helpers;
    using SplashImageViewer.Models;

    public partial class FullscreenForm : Form
    {
        private const int TimerIntervalMs = 10000;

        private readonly Timer hideInfoLabelTimer;
        private readonly Timer hideBottomLabelsTimer;
        private readonly Screen screen;
        private readonly bool slideshowIsEnabled;

        public FullscreenForm(Screen screen, bool slideshowIsEnabled)
        {
            InitializeComponent();
            hideInfoLabelTimer = new Timer();
            hideBottomLabelsTimer = new Timer();
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
            if (disposing && (components != null))
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
            totalFilesLabel.ForeColor = AppSettings.LabelsColor;
            infoLabel.ForeColor = AppSettings.LabelsColor;
            filePathLabel.ForeColor = AppSettings.LabelsColor;

            // Bounds = Screen.PrimaryScreen.Bounds;
            Bounds = screen.Bounds; // use passed-in screen reference bounds

            fullscreenPictureBox.Image = ImagesModel.Singleton.Image;
            fullscreenPictureBox.BackColor = AppSettings.ThemeColor;

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

            if (slideshowIsEnabled)
            {
                infoLabel.Text += "\n[SLIDESHOW ENABLED]";
            }
        }

        private void InitTimers()
        {
            hideInfoLabelTimer.Tick += HideInfoLabel;
            hideInfoLabelTimer.Interval = TimerIntervalMs;
            hideInfoLabelTimer.Start();

            hideBottomLabelsTimer.Tick += HideBottomLabels;
            hideBottomLabelsTimer.Interval = TimerIntervalMs;
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

            try
            {
                ImagesModel.Singleton.LoadImage();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (ImagesModel.Singleton.Image is object)
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
