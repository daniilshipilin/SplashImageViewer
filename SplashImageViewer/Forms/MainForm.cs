using SplashImageViewer.Helpers;
using SplashImageViewer.Models;
using SplashImageViewer.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Updater;

namespace SplashImageViewer.Forms
{
    public partial class MainForm : Form
    {
        const int CHECK_MEMORY_MS = 1000;
        const int RECENT_ITEMS_CAPACITY = 10;
        const int INITIAL_TOOLSTRIPMENUITEM_ELEMENTS = 2;

        public static ImagesModel Img { get; private set; }

        bool _fullscreenFormIsActive;
        Timer _slideshowTimer;
        Timer _allocatedMemoryTimer;
        bool _imageIsModified;
        bool _eventsSubscribed;
        readonly List<string> _recentItems = new List<string>(RECENT_ITEMS_CAPACITY);

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CheckScreenDimensions();

            programInfoLabel.Text = GitVersionInformation.InformationalVersion;
            imageDimensionsLabel.Text = string.Empty;

            AppSettings.CheckSettings();

            if (AppSettings.ForceCheckUpdates ||
                DateTime.Now.Subtract(AppSettings.UpdatesLastChecked).TotalDays > 1.0)
            {
                // check for updates in the background
                Task.Run(CheckUpdates);
            }

            mainPanel.BackColor = AppSettings.ThemeColor;
            totalFilesLabel.ForeColor = AppSettings.LabelsColor;

            SetControls(false);

            // change totalFilesLabel parent
            totalFilesLabel.Parent = pictureBox;
            totalFilesLabel.Location = new Point(pictureBox.Size.Width - totalFilesLabel.Size.Width,
                                                 pictureBox.Size.Height - totalFilesLabel.Size.Height);

            UpdateTotalFilesLabel();
            InitTimers();
            CheckMemoryAllocated(this, null);

            toolTip.SetToolTip(previousButton, "Previous image [LEFT ARROW]");
            toolTip.SetToolTip(nextButton, "Next image [RIGHT ARROW]");
            toolTip.SetToolTip(slideshowButton, "Start/stop slideshow [SPACE]");
            toolTip.SetToolTip(randomButton, "Select random image [R]");
            toolTip.SetToolTip(deleteFileButton, "Delete a file currently open [DEL]");
            toolTip.SetToolTip(fullscreenButton, "Fullscreen mode [F]");
            toolTip.SetToolTip(zoomButton, "Zoom in/out image [Z]");
            toolTip.SetToolTip(rotateImageButton, "Rotate image [D]");
            toolTip.SetToolTip(settingsButton, "Open settings [S]");

            // mouse wheel event handler
            MouseWheel += new MouseEventHandler(PictureBox_MouseWheel);

            // add recent items
            PopulateRecentItemsList();

            if (Program.CmdArgs != null) { OpenImageUsingCmdArgs(); }
        }

        private void CheckScreenDimensions()
        {
            // get current screen size
            var screen = Screen.FromControl(this).Bounds;

            if (screen.Width < AppSettings.MinScreenSizeWidth || screen.Height < AppSettings.MinScreenSizeHeight)
            {
                MessageBox.Show("The minimum screen resolution requirements not met", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
        }

        private void PopulateRecentItemsList()
        {
            // add info items
            recentItemsMenuItem.DropDownItems.Add(new ToolStripMenuItem
            {
                Enabled = false,
                Text = $"--- {RECENT_ITEMS_CAPACITY} recently opened items ---"
            });

            recentItemsMenuItem.DropDownItems.Add(new ToolStripSeparator());

            // get saved values from registry
            foreach (var item in AppSettings.GetRecentItemsFromRegistry())
            {
                _recentItems.Add(item);
            }

            // populate ToolStripMenuItem recent items list
            if (_recentItems.Count > 0)
            {
                for (int i = 0; i < _recentItems.Count; i++)
                {
                    var item = new ToolStripMenuItem { Text = _recentItems[i] };
                    item.Click += OpenRecentItem_Click;
                    recentItemsMenuItem.DropDownItems.Add(item);
                }
            }
            else { recentItemsMenuItem.Enabled = false; }
        }

        private void UpdateRecentItemsEvent(object sender)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object>(UpdateRecentItemsEvent), sender);
                return;
            }

            // check if file paths exist (remove, if don't)
            for (int i = INITIAL_TOOLSTRIPMENUITEM_ELEMENTS; i < recentItemsMenuItem.DropDownItems.Count; i++)
            {
                if (!File.Exists(recentItemsMenuItem.DropDownItems[i].Text))
                {
                    _recentItems.Remove(recentItemsMenuItem.DropDownItems[i].Text);
                    recentItemsMenuItem.DropDownItems.RemoveAt(i);
                    --i;
                }
            }

            string path = Img.CurrentFilePath;

            if (path == null) { return; }

            bool addItem = true;

            //add / remove items
            for (int i = INITIAL_TOOLSTRIPMENUITEM_ELEMENTS; i < recentItemsMenuItem.DropDownItems.Count; i++)
            {
                if (recentItemsMenuItem.DropDownItems[i].Text.Equals(path, StringComparison.OrdinalIgnoreCase))
                {
                    addItem = false;
                    break;
                }
            }

            if (addItem)
            {
                if (recentItemsMenuItem.DropDownItems.Count >=
                    RECENT_ITEMS_CAPACITY + INITIAL_TOOLSTRIPMENUITEM_ELEMENTS)
                {
                    _recentItems.RemoveAt(0);
                    recentItemsMenuItem.DropDownItems.RemoveAt(2);
                }

                _recentItems.Add(path);
                var item = new ToolStripMenuItem { Text = path };
                item.Click += OpenRecentItem_Click;
                recentItemsMenuItem.DropDownItems.Add(item);
            }

            recentItemsMenuItem.Enabled = (recentItemsMenuItem.DropDownItems.Count > 0);
            AppSettings.WriteRecentItemsToRegistry(_recentItems);
        }

        private void UpdatePictureBoxEvent(object sender)
        {
            // check if caller is on a different thread (invoke required)
            if (InvokeRequired)
            {
                Invoke(new Action<object>(UpdatePictureBoxEvent), sender);
                return;
            }

            // set picturebox image when fullscreen mode is not active
            if (!_fullscreenFormIsActive)
            {
                if (Img.FilePaths.Count == 0)
                {
                    CloseImage();
                    return;
                }

                try
                {
                    Img.LoadImage();
                }
                catch (Exception ex)
                {
                    ShowExceptionMessage(ex);
                    return;
                }

                if (Img.Image != null)
                {
                    pictureBox.Image = Img.Image;
                    UpdateFilePathText();
                    UpdateImageDimensionsLabel();
                    ZoomOut();
                    ModifyPictureBoxSizeMode();
                    UpdateTotalFilesLabel();
                    SetControls(true);
                }
            }
        }

        private void SubscribeEvents()
        {
            if (!_eventsSubscribed)
            {
                Img.CurrentFilePathIndexChanged += UpdateRecentItemsEvent;
                Img.CurrentFilePathIndexChanged += UpdatePictureBoxEvent;
                _eventsSubscribed = true;
            }

            UpdateRecentItemsEvent(this);
            UpdatePictureBoxEvent(this);
        }

        private void UnsubscribeEvents()
        {
            if (_eventsSubscribed)
            {
                Img.CurrentFilePathIndexChanged -= UpdateRecentItemsEvent;
                Img.CurrentFilePathIndexChanged -= UpdatePictureBoxEvent;
                _eventsSubscribed = false;
            }
        }

        private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            //// The amount by which we adjust scale per wheel click.
            //const float scale_per_delta = 0.1f / 120;

            //// Update the drawing based upon the mouse wheel scrolling.
            //_mainImageScale += e.Delta * scale_per_delta;

            //if (_mainImageScale < 0)
            //{
            //    _mainImageScale = 0;
            //}

            //// Size the image.
            //pictureBox.Size = new Size(
            //    (int)(_mainImageWidth * _mainImageScale),
            //    (int)(_mainImageHeight * _mainImageScale));

            // Display the new scale.
            //lblScale.Text = ImageScale.ToString("p0");

            if (pictureBox.Image == null)
            {
                return;
            }

            if (e.Delta > 0)
            {
                ZoomIn();
            }
            else if (e.Delta < 0)
            {
                ZoomOut();
            }
        }

        private void MainPanel_MouseEnter(object sender, EventArgs e)
        {
            if (!mainPanel.Focused)
            {
                mainPanel.Focus();
            }
        }

        private void ZoomIn()
        {
            if (mainPanel.ClientSize.Width < pictureBox.Image.Width ||
                mainPanel.ClientSize.Height < pictureBox.Image.Height)
            {
                pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
                pictureBox.Size = new Size(Img.Image.Width, Img.Image.Height);
                pictureBox.Dock = DockStyle.None;
                zoomButton.Image = Resources.ZoomOut_img;

                // center panel scroll bars (centered zoom in effect)
                mainPanel.AutoScrollPosition = new Point(pictureBox.Width / 2 - mainPanel.ClientSize.Width / 2,
                                                         pictureBox.Height / 2 - mainPanel.ClientSize.Height / 2);
            }
        }

        private void ZoomOut()
        {
            if (mainPanel.ClientSize.Width < pictureBox.Image.Width ||
                mainPanel.ClientSize.Height < pictureBox.Image.Height)
            {
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox.Size = new Size(mainPanel.Size.Width, mainPanel.Size.Height);
                pictureBox.Dock = DockStyle.Fill;
                zoomButton.Image = Resources.ZoomIn_img;
            }
        }

        private void InitTimers()
        {
            _slideshowTimer = new Timer();
            _slideshowTimer.Tick += SlideshowHandler;

            _allocatedMemoryTimer = new Timer();
            _allocatedMemoryTimer.Tick += CheckMemoryAllocated;
            _allocatedMemoryTimer.Interval = CHECK_MEMORY_MS;
            _allocatedMemoryTimer.Start();
        }

        private async Task CheckUpdates()
        {
            try
            {
                AppSettings.UpdatesLastChecked = DateTime.Now;

                var upd = new ProgramUpdater(Version.Parse(GitVersionInformation.SemVer),
                                             ApplicationInfo.BaseDirectory,
                                             ApplicationInfo.AppPath,
                                             ApplicationInfo.AppGUID);

                if (await upd.CheckUpdateIsAvailable())
                {
                    var dr = MessageBox.Show($"Newer program version available.\n" +
                        $"Current: {GitVersionInformation.SemVer}\n" +
                        $"Available: {upd.ProgramVerServer}\n\n" +
                        $"Update program?", "Program update",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dr == DialogResult.Yes)
                    {
                        await upd.Update();
                        Program.ProgramExit((int)Program.ExitCode.Success);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
        }

        private void OpenImageUsingCmdArgs()
        {
            // cmd provided filename/folder path
            string path = Path.GetFullPath(Program.CmdArgs[0]);

            if (File.Exists(path))
            {
                try
                {
                    UnsubscribeEvents();
                    Img?.Dispose();
                    Img = new ImagesModel(Path.GetDirectoryName(path), path, AppSettings.SearchInSubdirs);
                    SubscribeEvents();
                }
                catch (Exception ex)
                {
                    ShowExceptionMessage(ex);
                }
            }
            else if (Directory.Exists(path))
            {
                try
                {
                    UnsubscribeEvents();
                    Img?.Dispose();
                    Img = new ImagesModel(path, AppSettings.SearchInSubdirs);
                    SubscribeEvents();
                }
                catch (Exception ex)
                {
                    ShowExceptionMessage(ex);
                }
            }
            else
            {
                MessageBox.Show($"File/directory path '{path}' doesn't exist", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CheckMemoryAllocated(object sender, EventArgs e)
        {
            memoryAllocatedLabel.Text = $"Memory allocated: {GetTotalAllocatedMemoryInMBytes():0.00} MB";
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left || keyData == Keys.Down)
            {
                if (previousButton.Enabled) { PreviousButton_Click(this, null); }
            }
            else if (keyData == Keys.Right || keyData == Keys.Up)
            {
                if (nextButton.Enabled) { NextButton_Click(this, null); }
            }
            else if (keyData == Keys.R)
            {
                if (randomButton.Enabled) { RandomButton_Click(this, null); }
            }
            else if (keyData == Keys.Z)
            {
                if (zoomButton.Enabled) { ZoomButton_Click(this, null); }
            }
            else if (keyData == Keys.S)
            {
                if (settingsButton.Enabled) { SettingsButton_Click(this, null); }
            }
            else if (keyData == Keys.D)
            {
                if (rotateImageButton.Enabled) { RotateImageButton_Click(this, null); }
            }
            else if (keyData == Keys.Space)
            {
                if (slideshowButton.Enabled) { SlideshowButton_Click(this, null); }
            }
            else if (keyData == Keys.F)
            {
                if (fullscreenButton.Enabled) { FullscreenButton_Click(this, null); }
            }
            else if (keyData == Keys.Delete)
            {
                if (deleteFileButton.Enabled) { DeleteFileButton_Click(this, null); }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ShowExceptionMessage(Exception ex)
        {
            pictureBox.Image = Resources.Error_img;
            ZoomOut();
            ModifyPictureBoxSizeMode();
            MessageBox.Show(ex.Message, ex.GetType().ToString(),
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void CloseImage()
        {
            if (_slideshowTimer.Enabled)
            {
                _slideshowTimer.Stop();
                Screensaver.Reset();
            }

            if (pictureBox.Dock == DockStyle.None)
            {
                ZoomOut();
            }

            slideshowButton.Image = Resources.Play_img;
            SetControls(false);
            UnsubscribeEvents();
            Img.Dispose();
            Img = null;
            pictureBox.Image = null;
            Text = "Splash Image Viewer";
            imageDimensionsLabel.Text = string.Empty;
            UpdateTotalFilesLabel();
            GC.Collect();
        }

        private string GetFileSizeString(long fileSizeBytes)
        {
            if (fileSizeBytes < 1024)
            {
                return $"{fileSizeBytes} byte(s)";
            }
            else if (fileSizeBytes < 1048576)
            {
                return $"{(fileSizeBytes / 1024.0):0.0} KB";
            }
            else
            {
                return $"{(fileSizeBytes / 1048576.0):0.00} MB";
            }
        }

        private void UpdateFilePathText()
        {
            Text = Img.CurrentFilePath;
        }

        private void UpdateImageDimensionsLabel()
        {
            imageDimensionsLabel.Text = $"Dimensions: {Img.Image.Width}x{Img.Image.Height}  " +
                $"File size: {GetFileSizeString(new FileInfo(Img.CurrentFilePath).Length)}  " +
                $"Type: {Img.ImageFormatDescription}";
        }

        private void UpdateTotalFilesLabel()
        {
            if (Img == null)
            {
                totalFilesLabel.Text = "0 / 0";
            }
            else
            {
                totalFilesLabel.Text = $"{Img.CurrentFilePathIndex + 1} / {Img.FilePaths.Count}";
            }
        }

        private void SetControls(bool state)
        {
            slideshowButton.Enabled = state;
            previousButton.Enabled = state;
            nextButton.Enabled = state;
            randomButton.Enabled = state;
            fullscreenButton.Enabled = state;
            rotateImageButton.Enabled = state;
            deleteFileButton.Enabled = state;
            zoomButton.Enabled = state;
            closeImageMenuItem.Enabled = state;

            // enable/disable right click context menu
            rightClickMenuStrip.Enabled = state;
        }

        private void MainPanel_Resize(object sender, EventArgs e)
        {
            ModifyPictureBoxSizeMode();
        }

        private void ModifyPictureBoxSizeMode()
        {
            if (pictureBox.Image == null)
            {
                return;
            }

            if (mainPanel.ClientSize.Width < pictureBox.Image.Width ||
                mainPanel.ClientSize.Height < pictureBox.Image.Height)
            {
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            }
        }

        private void SlideshowHandler(object sender, EventArgs e)
        {
            if (AppSettings.SlideshowOrderIsRandom)
            {
                Img.SelectRandomImageIndex();
            }
            else
            {
                Img.SelectNextImageIndex();
            }
        }

        private void GoFullscreen()
        {
            _fullscreenFormIsActive = true;

            // get current display location/bounds, so that fullscreen form is displayed on the same display as the main form
            var screen = Screen.FromControl(this);

            try
            {
                using var fullscreenForm = new FullscreenForm(screen, _slideshowTimer.Enabled);
                fullscreenForm.ShowDialog();
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }

            _fullscreenFormIsActive = false;
        }

        private void CheckImageModified()
        {
            if (_imageIsModified)
            {
                OverwriteImage();
                UpdateFilePathText();
                _imageIsModified = false; // unset flag
            }
        }

        private void OverwriteImage()
        {
            if (AppSettings.ShowFileOverwritePrompt)
            {
                var dr = MessageBox.Show("Image modified. Overwrite current image?", "Image modified",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (dr != DialogResult.Yes) { return; }
            }

            try
            {
                Img.OverwriteImage();
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
        }

        #region Button Events

        private void OpenImage_Click(object sender, EventArgs e)
        {
            CheckImageModified();

            try
            {
                using var ofd = new OpenFileDialog()
                {
                    Filter = "Image Files|*.jpg; *.jpeg; *.jpe; *.jfif; *.bmp; *.png; *.gif; *.ico; *.tif|All Files|*.*"
                };

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    UnsubscribeEvents();
                    Img?.Dispose();
                    Img = new ImagesModel(Path.GetDirectoryName(ofd.FileName),
                        ofd.FileName, AppSettings.SearchInSubdirs);
                    SubscribeEvents();
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
        }

        private void OpenFolder_Click(object sender, EventArgs e)
        {
            CheckImageModified();

            try
            {
                using var fbd = new FolderBrowserDialog();

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    UnsubscribeEvents();
                    Img?.Dispose();
                    Img = new ImagesModel(fbd.SelectedPath, AppSettings.SearchInSubdirs);
                    SubscribeEvents();
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
        }

        private void CloseImage_Click(object sender, EventArgs e)
        {
            CheckImageModified();
            CloseImage();
        }

        private void OpenRecentItem_Click(object sender, EventArgs e)
        {
            CheckImageModified();

            if (sender is ToolStripMenuItem item)
            {
                try
                {
                    UnsubscribeEvents();
                    Img?.Dispose();
                    Img = new ImagesModel(Path.GetDirectoryName(item.Text),
                        item.Text, AppSettings.SearchInSubdirs);
                    SubscribeEvents();
                }
                catch (Exception ex)
                {
                    ShowExceptionMessage(ex);
                }
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            CheckImageModified();
            Close();
        }

        private void DeleteFileButton_Click(object sender, EventArgs e)
        {
            mainPanel.Focus();
            CheckImageModified();

            if (AppSettings.ShowFileDeletePrompt)
            {
                var dialogResult = MessageBox.Show("Are you sure you want to delete this file?", "Delete file",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialogResult != DialogResult.Yes) { return; }
            }

            try
            {
                Img.DeleteImage();
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
        }

        private void ZoomButton_Click(object sender, EventArgs e)
        {
            mainPanel.Focus();

            if (pictureBox.Image == null) { return; }

            if (pictureBox.Dock == DockStyle.None)
            {
                ZoomOut();
            }
            else
            {
                ZoomIn();
            }
        }

        private void FullscreenButton_Click(object sender, EventArgs e)
        {
            mainPanel.Focus();
            CheckImageModified();
            GoFullscreen();
            UpdatePictureBoxEvent(this);
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            mainPanel.Focus();
            CheckImageModified();
            Img.SelectPreviousImageIndex();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            mainPanel.Focus();
            CheckImageModified();
            Img.SelectNextImageIndex();
        }

        private void RandomButton_Click(object sender, EventArgs e)
        {
            mainPanel.Focus();
            CheckImageModified();
            Img.SelectRandomImageIndex();
        }

        private void SlideshowButton_Click(object sender, EventArgs e)
        {
            mainPanel.Focus();
            CheckImageModified();

            if (_slideshowTimer.Enabled)
            {
                _slideshowTimer.Stop();
                slideshowButton.Image = Resources.Play_img;
                Screensaver.Reset();
            }
            else
            {
                _slideshowTimer.Interval = AppSettings.SlideshowTransitionMs;
                slideshowButton.Image = Resources.Stop_img;
                _slideshowTimer.Start();
                Screensaver.Disable();
            }
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            mainPanel.Focus();
            SettingsToolStripMenuItem_Click(this, null);
        }

        private void RotateImageButton_Click(object sender, EventArgs e)
        {
            mainPanel.Focus();

            if (_slideshowTimer.Enabled)
            {
                MessageBox.Show("Slideshow mode is active. Stop slideshow first.", "Slideshow mode is active",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //pictureBox.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            Img.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pictureBox.Refresh();

            Text = $"{Img.CurrentFilePath} [MODIFIED]";
            _imageIsModified = true;
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();

            if (mainPanel.BackColor != AppSettings.ThemeColor)
            {
                mainPanel.BackColor = AppSettings.ThemeColor;
                totalFilesLabel.ForeColor = AppSettings.LabelsColor;
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (rightClickMenuStrip.Enabled)
                {
                    fileNameMenuItem.Text = $"--- {Path.GetFileName(Img.CurrentFilePath)} ---";
                    rightClickMenuStrip.Show(this, new Point(e.X, e.Y)); // places the menu at the pointer position
                }
            }
        }

        private void CopyPathMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetDataObject(Img.CurrentFilePath);
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
        }

        private void CopyFileMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetData(DataFormats.FileDrop, new string[] { Img.CurrentFilePath });
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
        }

        #endregion

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CheckImageModified();
        }

        private static long GetTotalAllocatedMemoryInBytes()
        {
            using var p = Process.GetCurrentProcess();

            return p.PrivateMemorySize64;
        }

        private static double GetTotalAllocatedMemoryInMBytes()
        {
            return GetTotalAllocatedMemoryInBytes() / 1048576.0;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            _slideshowTimer?.Dispose();
            _allocatedMemoryTimer?.Dispose();

            base.Dispose(disposing);
        }
    }
}
