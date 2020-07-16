using System;
using System.Windows.Forms;
using SplashImageViewer.Helpers;
using SplashImageViewer.Models;

namespace SplashImageViewer.Forms
{
    public partial class FullscreenForm : Form
    {
        const int TIMER_INTERVAL_MS = 10000;

        readonly Timer _hideInfoLabelTimer;
        readonly Timer _hideBottomLabelsTimer;
        readonly Screen _screen;
        readonly bool _slideshowIsEnabled;

        public FullscreenForm(Screen screen, bool slideshowIsEnabled)
        {
            InitializeComponent();
            _hideInfoLabelTimer = new Timer();
            _hideBottomLabelsTimer = new Timer();
            _screen = screen;
            _slideshowIsEnabled = slideshowIsEnabled;
        }

        private void FullscreenForm_Load(object sender, EventArgs e)
        {
            totalFilesLabel.ForeColor = AppSettings.LabelsColor;
            infoLabel.ForeColor = AppSettings.LabelsColor;
            filePathLabel.ForeColor = AppSettings.LabelsColor;

            //Bounds = Screen.PrimaryScreen.Bounds;
            Bounds = _screen.Bounds; // use passed-in screen reference bounds

            fullscreenPictureBox.Image = ImagesModel.Singleton.Image;
            fullscreenPictureBox.BackColor = AppSettings.ThemeColor;

            CheckFormSize();

            // change totalFilesLabel parent
            totalFilesLabel.Parent = fullscreenPictureBox;
            //totalFilesLabel.Location = new Point(fullscreenPictureBox.Size.Width - totalFilesLabel.Size.Width,
            //                                     fullscreenPictureBox.Size.Height - totalFilesLabel.Size.Height);

            // change infoLabel parent
            infoLabel.Parent = fullscreenPictureBox;

            // change infoLabel parent
            filePathLabel.Parent = fullscreenPictureBox;
            filePathLabel.Text = ImagesModel.Singleton.CurrentFilePath;

            ImagesModel.Singleton.CurrentFilePathIndexChanged += UpdatePictureBoxEvent;

            UpdateBottomLabels();
            InitTimers();

            if (_slideshowIsEnabled)
            {
                infoLabel.Text += "\n[SLIDESHOW ENABLED]";
            }
        }

        private void InitTimers()
        {
            _hideInfoLabelTimer.Tick += HideInfoLabel;
            _hideInfoLabelTimer.Interval = TIMER_INTERVAL_MS;
            _hideInfoLabelTimer.Start();

            _hideBottomLabelsTimer.Tick += HideBottomLabels;
            _hideBottomLabelsTimer.Interval = TIMER_INTERVAL_MS;
            _hideBottomLabelsTimer.Start();
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

        private void RestartHideInfoLabeTimer()
        {
            infoLabel.Visible = true;
            _hideInfoLabelTimer.Stop();
            _hideInfoLabelTimer.Start();
        }

        private void RestartHideBottomLabelsTimer()
        {
            filePathLabel.Visible = true;
            totalFilesLabel.Visible = true;
            _hideBottomLabelsTimer.Stop();
            _hideBottomLabelsTimer.Start();
        }

        private void HideInfoLabel(object? sender, EventArgs e)
        {
            infoLabel.Visible = false;
            _hideInfoLabelTimer.Stop();
            Update();
        }

        private void HideBottomLabels(object? sender, EventArgs e)
        {
            filePathLabel.Visible = false;
            totalFilesLabel.Visible = false;
            _hideBottomLabelsTimer.Stop();
            Update();
        }

        private void UpdatePictureBoxEvent(object sender)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { UpdatePictureBoxEvent(sender); });
                return;
            }

            UpdatePictureBox();
        }

        private void UpdatePictureBox()
        {
            if (ImagesModel.Singleton.FilePaths.Count == 0) { Close(); }

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

            _hideInfoLabelTimer.Tick -= HideInfoLabel;
            _hideBottomLabelsTimer.Tick -= HideBottomLabels;
            ImagesModel.Singleton.CurrentFilePathIndexChanged -= UpdatePictureBoxEvent;
            _hideInfoLabelTimer.Dispose();
            _hideBottomLabelsTimer.Dispose();

            // free native resources
            base.Dispose(disposing);
        }
    }
}
