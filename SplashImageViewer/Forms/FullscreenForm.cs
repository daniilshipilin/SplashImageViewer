using System;
using System.Windows.Forms;
using SplashImageViewer.Helpers;

namespace SplashImageViewer.Forms
{
    public partial class FullscreenForm : Form
    {
        const int TIMER_INTERVAL_MS = 10000;

        Timer _hideInfoLabelTimer;
        Timer _hideBottomLabelsTimer;
        readonly Screen _screen;
        readonly bool _slideshowIsEnabled;

        public FullscreenForm(Screen screen, bool slideshowIsEnabled)
        {
            _screen = screen;
            _slideshowIsEnabled = slideshowIsEnabled;
            InitializeComponent();
        }

        private void FullscreenForm_Load(object sender, EventArgs e)
        {
            totalFilesLabel.ForeColor = AppSettings.LabelsColor;
            infoLabel.ForeColor = AppSettings.LabelsColor;
            filePathLabel.ForeColor = AppSettings.LabelsColor;

            //Bounds = Screen.PrimaryScreen.Bounds;
            Bounds = _screen.Bounds; // use passed-in screen reference bounds

            fullscreenPictureBox.Image = MainForm.Img.Image;
            fullscreenPictureBox.BackColor = AppSettings.ThemeColor;

            CheckFormSize();

            // change totalFilesLabel parent
            totalFilesLabel.Parent = fullscreenPictureBox;
            //totalFilesLabel.Location = new Point(fullscreenPictureBox.Size.Width - totalFilesLabel.Size.Width,
            //    fullscreenPictureBox.Size.Height - totalFilesLabel.Size.Height);

            // change infoLabel parent
            infoLabel.Parent = fullscreenPictureBox;

            // change infoLabel parent
            filePathLabel.Parent = fullscreenPictureBox;
            filePathLabel.Text = MainForm.Img.CurrentFilePath;

            MainForm.Img.CurrentFilePathIndexChanged += SetPictureBoxEvent;

            UpdateBottomLabels();
            InitTimers();
            SetFullscreenMode();

            if (_slideshowIsEnabled)
            {
                infoLabel.Text += "\n[SLIDESHOW ENABLED]";
            }
        }

        private void InitTimers()
        {
            _hideInfoLabelTimer = new Timer();
            _hideInfoLabelTimer.Tick += HideInfoLabel;
            _hideInfoLabelTimer.Interval = TIMER_INTERVAL_MS;
            _hideInfoLabelTimer.Start();

            _hideBottomLabelsTimer = new Timer();
            _hideBottomLabelsTimer.Tick += HideBottomLabels;
            _hideBottomLabelsTimer.Interval = TIMER_INTERVAL_MS;
            _hideBottomLabelsTimer.Start();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape || keyData == Keys.F)
            {
                ResetFullscreenMode();
                Close();
            }
            else if (keyData == Keys.Left || keyData == Keys.Down)
            {
                MainForm.Img.SelectPreviousImageIndex();
                RestartHideBottomLabelsTimer();
            }
            else if (keyData == Keys.Right || keyData == Keys.Up)
            {
                MainForm.Img.SelectNextImageIndex();
                RestartHideBottomLabelsTimer();
            }
            else if (keyData == Keys.R)
            {
                MainForm.Img.SelectRandomImageIndex();
                RestartHideBottomLabelsTimer();
            }
            else
            {
                RestartHideInfoLabeTimer();
                RestartHideBottomLabelsTimer();
                Update();
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

        private void HideInfoLabel(object sender, EventArgs e)
        {
            infoLabel.Visible = false;
            _hideInfoLabelTimer.Stop();
            Update();
        }

        private void HideBottomLabels(object sender, EventArgs e)
        {
            filePathLabel.Visible = false;
            totalFilesLabel.Visible = false;
            _hideBottomLabelsTimer.Stop();
            Update();
        }

        private void SetPictureBoxEvent(object sender)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object>(SetPictureBoxEvent), new object[] { this });
                return;
            }

            if (MainForm.Img.FilePaths.Count == 0)
            {
                ResetFullscreenMode();
                Close();
            }

            try
            {
                MainForm.Img.LoadImage();
            }
            catch (Exception)
            {
                ResetFullscreenMode();
                throw new Exception("Couldn't open file");
            }

            if (MainForm.Img.Image != null)
            {
                fullscreenPictureBox.Image = MainForm.Img.Image;
                CheckFormSize();
                UpdateBottomLabels();
            }
        }

        private void CheckFormSize()
        {
            if (fullscreenPictureBox.Image == null) { return; }

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
            totalFilesLabel.Text = $"{MainForm.Img.CurrentFilePathIndex + 1} / {MainForm.Img.FilePaths.Count}";
            filePathLabel.Text = MainForm.Img.CurrentFilePath;
        }

        private void SetFullscreenMode()
        {
            Cursor.Hide();
        }

        private void ResetFullscreenMode()
        {
            Cursor.Show();
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

            fullscreenPictureBox?.Image.Dispose();

            _hideInfoLabelTimer.Tick -= HideInfoLabel;
            _hideBottomLabelsTimer.Tick -= HideBottomLabels;
            MainForm.Img.CurrentFilePathIndexChanged -= SetPictureBoxEvent;

            _hideInfoLabelTimer?.Dispose();
            _hideBottomLabelsTimer?.Dispose();

            // free native resources
            base.Dispose(disposing);
        }
    }
}
