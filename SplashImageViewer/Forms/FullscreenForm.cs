namespace SplashImageViewer.Forms
{
    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using SplashImageViewer.Helpers;
    using SplashImageViewer.Models;
    using SplashImageViewer.Properties;

    public partial class FullscreenForm : Form
    {
        private readonly Timer hideInfoLabelTimer = new();
        private readonly Timer hideBottomLabelsTimer = new();
        private readonly Screen screen;
        private readonly bool slideshowIsEnabled;

        public FullscreenForm(Screen screen, bool slideshowIsEnabled)
        {
            this.InitializeComponent();
            this.screen = screen;
            this.slideshowIsEnabled = slideshowIsEnabled;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                case Keys.F:
                    this.Close();
                    break;

                case Keys.Left:
                case Keys.Down:
                    ImagesModel.Singleton.SelectPreviousImageIndex();
                    this.RestartHideBottomLabelsTimer();
                    break;

                case Keys.Right:
                case Keys.Up:
                    ImagesModel.Singleton.SelectNextImageIndex();
                    this.RestartHideBottomLabelsTimer();
                    break;

                case Keys.R:
                    ImagesModel.Singleton.SelectRandomImageIndex();
                    this.RestartHideBottomLabelsTimer();
                    break;

                default:
                    this.RestartHideInfoLabeTimer();
                    this.RestartHideBottomLabelsTimer();
                    this.Update();
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
            if (disposing && (this.components is not null))
            {
                // dispose managed resources
                this.components.Dispose();
            }

            this.fullscreenPictureBox.Image.Dispose();

            this.hideInfoLabelTimer.Tick -= this.HideInfoLabel;
            this.hideBottomLabelsTimer.Tick -= this.HideBottomLabels;
            ImagesModel.Singleton.CurrentFilePathIndexChanged -= this.UpdatePictureBoxEvent;
            this.hideInfoLabelTimer.Dispose();
            this.hideBottomLabelsTimer.Dispose();

            // free native resources
            base.Dispose(disposing);
        }

        private void FullscreenForm_Load(object sender, EventArgs e)
        {
            this.LocalizeUIElements();

            this.totalFilesLabel.ForeColor = Color.FromArgb(AppSettings.LabelsColorArgb);
            this.infoLabel.ForeColor = Color.FromArgb(AppSettings.LabelsColorArgb);
            this.filePathLabel.ForeColor = Color.FromArgb(AppSettings.LabelsColorArgb);

            // Bounds = Screen.PrimaryScreen.Bounds;
            this.Bounds = this.screen.Bounds; // use passed-in screen reference bounds

            this.fullscreenPictureBox.Image = ImagesModel.Singleton.Image;
            this.fullscreenPictureBox.BackColor = Color.FromArgb(AppSettings.ThemeColorArgb);

            this.CheckFormSize();

            // change totalFilesLabel parent
            this.totalFilesLabel.Parent = this.fullscreenPictureBox;

            // totalFilesLabel.Location = new Point(fullscreenPictureBox.Size.Width - totalFilesLabel.Size.Width,
            //                                     fullscreenPictureBox.Size.Height - totalFilesLabel.Size.Height);

            // change infoLabel parent
            this.infoLabel.Parent = this.fullscreenPictureBox;

            // change infoLabel parent
            this.filePathLabel.Parent = this.fullscreenPictureBox;
            this.filePathLabel.Text = ImagesModel.Singleton.CurrentFilePath;

            ImagesModel.Singleton.CurrentFilePathIndexChanged += this.UpdatePictureBoxEvent;

            this.UpdateBottomLabels();
            this.InitTimers();
        }

        private void LocalizeUIElements()
        {
            this.infoLabel.Text = this.slideshowIsEnabled ?
                $"{Strings.SlideshowInfoLabel}{Environment.NewLine}{Strings.SlideshowEnabled}" :
                Strings.SlideshowInfoLabel;
        }

        private void InitTimers()
        {
            this.hideInfoLabelTimer.Tick += this.HideInfoLabel;
            this.hideInfoLabelTimer.Interval = AppSettings.FullscreenFormHideInfoTimerIntervalMs;
            this.hideInfoLabelTimer.Start();

            this.hideBottomLabelsTimer.Tick += this.HideBottomLabels;
            this.hideBottomLabelsTimer.Interval = AppSettings.FullscreenFormHideInfoTimerIntervalMs;
            this.hideBottomLabelsTimer.Start();
        }

        private void RestartHideInfoLabeTimer()
        {
            this.infoLabel.Visible = true;
            this.hideInfoLabelTimer.Stop();
            this.hideInfoLabelTimer.Start();
        }

        private void RestartHideBottomLabelsTimer()
        {
            this.filePathLabel.Visible = true;
            this.totalFilesLabel.Visible = true;
            this.hideBottomLabelsTimer.Stop();
            this.hideBottomLabelsTimer.Start();
        }

        private void HideInfoLabel(object? sender, EventArgs e)
        {
            this.infoLabel.Visible = false;
            this.hideInfoLabelTimer.Stop();
            this.Update();
        }

        private void HideBottomLabels(object? sender, EventArgs e)
        {
            this.filePathLabel.Visible = false;
            this.totalFilesLabel.Visible = false;
            this.hideBottomLabelsTimer.Stop();
            this.Update();
        }

        private void UpdatePictureBoxEvent(object sender)
        {
            // check if caller is on a different thread (invoke required)
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(this.UpdatePictureBox));
            }
            else
            {
                this.UpdatePictureBox();
            }
        }

        private async void UpdatePictureBox()
        {
            if (ImagesModel.Singleton.FilePaths.Count == 0)
            {
                this.Close();
                return;
            }

            // load image async in background
            await Task.Run(() => ImagesModel.Singleton.LoadImage());

            if (ImagesModel.Singleton.Image is not null)
            {
                this.fullscreenPictureBox.Image = ImagesModel.Singleton.Image;
                this.CheckFormSize();
                this.UpdateBottomLabels();
            }
        }

        private void CheckFormSize()
        {
            this.fullscreenPictureBox.SizeMode = this.fullscreenPictureBox.ClientSize.Width < this.fullscreenPictureBox.Image.Width ||
                this.fullscreenPictureBox.ClientSize.Height < this.fullscreenPictureBox.Image.Height
                ? PictureBoxSizeMode.Zoom
                : PictureBoxSizeMode.CenterImage;
        }

        private void UpdateBottomLabels()
        {
            this.totalFilesLabel.Text = $"{ImagesModel.Singleton.CurrentFilePathIndex + 1} / {ImagesModel.Singleton.FilePaths.Count}";
            this.filePathLabel.Text = ImagesModel.Singleton.CurrentFilePath;
        }
    }
}
