namespace SplashImageViewer.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using SplashImageViewer.Helpers;
    using SplashImageViewer.Models;
    using SplashImageViewer.Properties;
    using Updater;

    public partial class MainForm : Form
    {
        private const int CheckMemoryMs = 1000;

        private readonly string[] args;
        private readonly Timer slideshowTimer;
        private readonly Timer allocatedMemoryTimer;
        private bool fullscreenFormIsActive;
        private bool imageIsModified;
        private bool eventsSubscribed;

        public MainForm(string[] args)
        {
            InitializeComponent();
            this.args = args;
            slideshowTimer = new Timer();
            allocatedMemoryTimer = new Timer();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                case Keys.Down:
                    if (previousButton.Enabled)
                    {
                        PreviousButton_Click();
                    }

                    break;

                case Keys.Right:
                case Keys.Up:
                    if (nextButton.Enabled)
                    {
                        NextButton_Click();
                    }

                    break;

                case Keys.R:
                    if (randomButton.Enabled)
                    {
                        RandomButton_Click();
                    }

                    break;

                case Keys.Z:
                    if (zoomButton.Enabled)
                    {
                        ZoomButton_Click();
                    }

                    break;

                case Keys.S:
                    if (settingsButton.Enabled)
                    {
                        SettingsButton_Click();
                    }

                    break;

                case Keys.D:
                    if (rotateImageButton.Enabled)
                    {
                        RotateImageButton_Click();
                    }

                    break;

                case Keys.Space:
                    if (slideshowButton.Enabled)
                    {
                        SlideshowButton_Click();
                    }

                    break;

                case Keys.F:
                    if (fullscreenButton.Enabled)
                    {
                        FullscreenButton_Click();
                    }

                    break;

                case Keys.Delete:
                    if (deleteFileButton.Enabled)
                    {
                        DeleteFileButton_Click();
                    }

                    break;

                case Keys.Escape:
                    if (pictureBox.Image is object)
                    {
                        CloseImage();
                    }

                    break;

                default:
                    break;
            }

            pictureBox.Focus();

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
                components.Dispose();
            }

            slideshowTimer.Dispose();
            allocatedMemoryTimer.Dispose();

            base.Dispose(disposing);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CheckScreenDimensions();

            programInfoLabel.Text = GitVersionInformation.InformationalVersion;
            imageDimensionsLabel.Text = string.Empty;

            AppSettings.CheckSettings();

            if (AppSettings.ForceCheckUpdates ||
                (DateTime.UtcNow - AppSettings.UpdatesLastCheckedUtcTimestamp).Days >= 1)
            {
                // check for updates in the background
                Task.Run(async () => await CheckUpdates());
            }

            mainPanel.BackColor = AppSettings.ThemeColor;
            totalFilesLabel.ForeColor = AppSettings.LabelsColor;

            SetControls(false);

            // change totalFilesLabel parent
            totalFilesLabel.Parent = pictureBox;
            totalFilesLabel.Location = new Point(
                pictureBox.Size.Width - totalFilesLabel.Size.Width,
                pictureBox.Size.Height - totalFilesLabel.Size.Height);

            UpdateTotalFilesLabel();
            InitTimers();
            CheckMemoryAllocated();

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
            MouseWheel += PictureBox_MouseWheel;

            // add recent items
            PopulateRecentItemsList();

            // check, if args exist
            ProcessCmdArgs();
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
            // get saved values from registry and then populate ToolStripMenuItem
            foreach (var item in AppSettings.GetRecentItemsFromRegistry())
            {
                if (recentItemsMenuItem.DropDownItems.Count >= AppSettings.RecentItemsCapacity)
                {
                    break;
                }

                if (File.Exists(item))
                {
                    var menuItem = new ToolStripMenuItem { Text = item };
                    menuItem.Click += OpenRecentItem_Click;
                    recentItemsMenuItem.DropDownItems.Add(menuItem);
                }
            }

            recentItemsMenuItem.Enabled = recentItemsMenuItem.DropDownItems.Count > 0;
            WriteRecentItemsToRegistry();
        }

        private void ClearRecentItemsList()
        {
            for (int i = 0; i < recentItemsMenuItem.DropDownItems.Count; i++)
            {
                // unsubscribe event handlers first
                recentItemsMenuItem.DropDownItems[i].Click -= OpenRecentItem_Click;
            }

            recentItemsMenuItem.DropDownItems.Clear();
            recentItemsMenuItem.Enabled = false;
        }

        private void WriteRecentItemsToRegistry()
        {
            var items = new List<string>();

            for (int i = 0; i < recentItemsMenuItem.DropDownItems.Count; i++)
            {
                items.Add(recentItemsMenuItem.DropDownItems[i].Text);
            }

            AppSettings.WriteRecentItemsToRegistry(items);
        }

        private void UpdateRecentItemsEvent(object sender)
        {
            // check if caller is on a different thread (invoke required)
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(UpdateRecentItems));
            }
            else
            {
                UpdateRecentItems();
            }
        }

        private void UpdateRecentItems()
        {
            // check if file paths exist (remove, if don't)
            for (int i = 0; i < recentItemsMenuItem.DropDownItems.Count; i++)
            {
                if (!File.Exists(recentItemsMenuItem.DropDownItems[i].Text))
                {
                    // unsubscribe event handlers first
                    recentItemsMenuItem.DropDownItems[i].Click -= OpenRecentItem_Click;
                    recentItemsMenuItem.DropDownItems.RemoveAt(i);
                    --i;
                }
            }

            string path = ImagesModel.Singleton.CurrentFilePath;
            bool addItem = true;

            // add / remove items
            for (int i = 0; i < recentItemsMenuItem.DropDownItems.Count; i++)
            {
                if (recentItemsMenuItem.DropDownItems[i].Text.Equals(path))
                {
                    addItem = false;
                    break;
                }
            }

            if (addItem)
            {
                if (recentItemsMenuItem.DropDownItems.Count >= AppSettings.RecentItemsCapacity)
                {
                    recentItemsMenuItem.DropDownItems[0].Click -= OpenRecentItem_Click;
                    recentItemsMenuItem.DropDownItems.RemoveAt(0);
                }

                var menuItem = new ToolStripMenuItem { Text = path };
                menuItem.Click += OpenRecentItem_Click;
                recentItemsMenuItem.DropDownItems.Add(menuItem);
            }

            recentItemsMenuItem.Enabled = recentItemsMenuItem.DropDownItems.Count > 0;
            WriteRecentItemsToRegistry();
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
            // set picturebox image only, when fullscreen mode is not active
            if (!fullscreenFormIsActive)
            {
                if (ImagesModel.Singleton.FilePaths.Count == 0)
                {
                    CloseImage();
                    return;
                }

                try
                {
                    ImagesModel.Singleton.LoadImage();
                }
                catch (Exception ex)
                {
                    ShowExceptionMessage(ex);
                    return;
                }

                if (ImagesModel.Singleton.Image is object)
                {
                    pictureBox.Image = ImagesModel.Singleton.Image;
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
            if (!eventsSubscribed)
            {
                ImagesModel.Singleton.CurrentFilePathIndexChanged += UpdateRecentItemsEvent;
                ImagesModel.Singleton.CurrentFilePathIndexChanged += UpdatePictureBoxEvent;
                eventsSubscribed = true;
            }

            UpdateRecentItems();
            UpdatePictureBox();
        }

        private void UnsubscribeEvents()
        {
            if (eventsSubscribed)
            {
                ImagesModel.Singleton.CurrentFilePathIndexChanged -= UpdateRecentItemsEvent;
                ImagesModel.Singleton.CurrentFilePathIndexChanged -= UpdatePictureBoxEvent;
                eventsSubscribed = false;
            }
        }

        private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (pictureBox.Image is object)
            {
                if (e.Delta > 0)
                {
                    ZoomIn();
                }
                else if (e.Delta < 0)
                {
                    ZoomOut();
                }
            }
        }

        private void ZoomIn()
        {
            if (ImagesModel.Singleton.Image is object)
            {
                if (mainPanel.ClientSize.Width < pictureBox.Image.Width ||
                    mainPanel.ClientSize.Height < pictureBox.Image.Height)
                {
                    pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
                    pictureBox.Size = new Size(ImagesModel.Singleton.Image.Width, ImagesModel.Singleton.Image.Height);
                    pictureBox.Dock = DockStyle.None;
                    zoomButton.Image = Resources.ZoomOut_48x48;

                    // center panel scroll bars (centered zoom in effect)
                    mainPanel.AutoScrollPosition = new Point(
                        (pictureBox.Width / 2) - (mainPanel.ClientSize.Width / 2),
                        (pictureBox.Height / 2) - (mainPanel.ClientSize.Height / 2));
                }
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
                zoomButton.Image = Resources.ZoomIn_48x48;
            }
        }

        private void InitTimers()
        {
            slideshowTimer.Tick += SlideshowHandler;

            allocatedMemoryTimer.Tick += CheckMemoryAllocated;
            allocatedMemoryTimer.Interval = CheckMemoryMs;
            allocatedMemoryTimer.Start();
        }

        private async Task CheckUpdates()
        {
            try
            {
                AppSettings.UpdateUpdatesLastCheckedUtcTimestamp();

                var upd = new ProgramUpdater(
                    Version.Parse(GitVersionInformation.SemVer),
                    ApplicationInfo.BaseDirectory,
                    ApplicationInfo.ExePath,
                    ApplicationInfo.AppGUID);

                if (await upd.CheckUpdateIsAvailable())
                {
                    var dr = MessageBox.Show(
                        $"Newer program version available.\n" +
                        $"Current: {GitVersionInformation.SemVer}\n" +
                        $"Available: {upd.ProgramVerServer}\n\n" +
                        $"Update program?",
                        "Program update",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (dr == DialogResult.Yes)
                    {
                        await upd.Update();
                        Program.ProgramExit();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
        }

        private void ProcessCmdArgs()
        {
            if (args.Length > 0)
            {
                // cmd provided filename/folder path
                string path = Path.GetFullPath(args[0]);
                OpenImage(path);
            }
        }

        private void CheckMemoryAllocated(object? sender = null, EventArgs? e = null)
        {
            memoryAllocatedLabel.Text = $"Memory allocated: {GetTotalAllocatedMemoryInMBytes():0.00} MB";
        }

        private void ShowExceptionMessage(Exception ex)
        {
            pictureBox.Image = Resources.Error_64x64;
            ZoomOut();
            ModifyPictureBoxSizeMode();
            MessageBox.Show(
                ex.Message,
                ex.GetType().ToString(),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void OpenImage(string path)
        {
            try
            {
                UnsubscribeEvents();
                ImagesModel.Singleton.DisposeResources();
                ImagesModel.Singleton.Init(path, AppSettings.SearchInSubdirs);
                SubscribeEvents();
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
        }

        private void CloseImage()
        {
            if (slideshowTimer.Enabled)
            {
                slideshowTimer.Stop();
                Screensaver.Reset();
            }

            if (pictureBox.Dock == DockStyle.None)
            {
                ZoomOut();
            }

            slideshowButton.Image = Resources.Play_48x48;
            SetControls(false);
            UnsubscribeEvents();
            ImagesModel.Singleton.DisposeResources();
            pictureBox.Image = null;
            pictureBox.Focus();
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
                return $"{fileSizeBytes / 1024.0:0.0} KB";
            }
            else
            {
                return $"{fileSizeBytes / 1048576.0:0.00} MB";
            }
        }

        private void UpdateFilePathText()
        {
            Text = ImagesModel.Singleton.CurrentFilePath;
        }

        private void UpdateImageDimensionsLabel()
        {
            imageDimensionsLabel.Text = $"Dimensions: {ImagesModel.Singleton.Image?.Width}x{ImagesModel.Singleton.Image?.Height}  " +
                $"File size: {GetFileSizeString(new FileInfo(ImagesModel.Singleton.CurrentFilePath).Length)}  " +
                $"Type: {ImagesModel.Singleton.ImageFormatDescription}";
        }

        private void UpdateTotalFilesLabel()
        {
            if (pictureBox.Image is null)
            {
                totalFilesLabel.Text = "0 / 0";
            }
            else
            {
                totalFilesLabel.Text = $"{ImagesModel.Singleton.CurrentFilePathIndex + 1} / {ImagesModel.Singleton.FilePaths.Count}";
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

        private void SlideshowHandler(object? sender, EventArgs e)
        {
            if (AppSettings.SlideshowOrderIsRandom)
            {
                ImagesModel.Singleton.SelectRandomImageIndex();
            }
            else
            {
                ImagesModel.Singleton.SelectNextImageIndex();
            }
        }

        private void GoFullscreen()
        {
            fullscreenFormIsActive = true;
            Cursor.Hide();

            // get current display location/bounds, so that fullscreen form is displayed on the same display as the main form
            var screen = Screen.FromControl(this);

            try
            {
                using var fullscreenForm = new FullscreenForm(screen, slideshowTimer.Enabled);
                fullscreenForm.ShowDialog();
            }
            catch (Exception ex)
            {
                Cursor.Show();
                ShowExceptionMessage(ex);
            }

            fullscreenFormIsActive = false;
            Cursor.Show();
        }

        private void CheckImageModified()
        {
            if (imageIsModified)
            {
                OverwriteImage();
                UpdateFilePathText();
                imageIsModified = false;
            }
        }

        private void OverwriteImage()
        {
            if (AppSettings.ShowFileOverwritePrompt)
            {
                var dr = MessageBox.Show(
                    "Image modified. Overwrite current image?",
                    "Image modified",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (dr == DialogResult.Yes)
                {
                    try
                    {
                        ImagesModel.Singleton.OverwriteImage();
                    }
                    catch (Exception ex)
                    {
                        ShowExceptionMessage(ex);
                    }
                }
            }
        }

        private void OpenImage_Click(object? sender = null, EventArgs? e = null)
        {
            CheckImageModified();

            using var ofd = new OpenFileDialog() { Filter = "Image Files|*.jpg; *.jpeg; *.jpe; *.jfif; *.bmp; *.png; *.gif; *.ico; *.tif|All Files|*.*" };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                OpenImage(ofd.FileName);
            }
        }

        private void OpenFolder_Click(object? sender = null, EventArgs? e = null)
        {
            CheckImageModified();

            using var fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                OpenImage(fbd.SelectedPath);
            }
        }

        private void CloseImage_Click(object? sender = null, EventArgs? e = null)
        {
            CheckImageModified();
            CloseImage();
        }

        private void OpenRecentItem_Click(object? sender = null, EventArgs? e = null)
        {
            CheckImageModified();

            if (sender is ToolStripMenuItem item)
            {
                OpenImage(item.Text);
            }
        }

        private void Exit_Click(object? sender = null, EventArgs? e = null)
        {
            CheckImageModified();
            Close();
        }

        private void DeleteFileButton_Click(object? sender = null, EventArgs? e = null)
        {
            CheckImageModified();

            if (AppSettings.ShowFileDeletePrompt)
            {
                var dialogResult = MessageBox.Show(
                    "Are you sure you want to delete this file?",
                    "Delete file",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }

            try
            {
                ImagesModel.Singleton.DeleteImage();
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
        }

        private void ZoomButton_Click(object? sender = null, EventArgs? e = null)
        {
            if (pictureBox.Dock == DockStyle.None)
            {
                ZoomOut();
            }
            else
            {
                ZoomIn();
            }
        }

        private void FullscreenButton_Click(object? sender = null, EventArgs? e = null)
        {
            CheckImageModified();
            GoFullscreen();
            UpdatePictureBox();
        }

        private void PreviousButton_Click(object? sender = null, EventArgs? e = null)
        {
            CheckImageModified();
            ImagesModel.Singleton.SelectPreviousImageIndex();
        }

        private void NextButton_Click(object? sender = null, EventArgs? e = null)
        {
            CheckImageModified();
            ImagesModel.Singleton.SelectNextImageIndex();
        }

        private void RandomButton_Click(object? sender = null, EventArgs? e = null)
        {
            CheckImageModified();
            ImagesModel.Singleton.SelectRandomImageIndex();
        }

        private void SlideshowButton_Click(object? sender = null, EventArgs? e = null)
        {
            CheckImageModified();

            if (slideshowTimer.Enabled)
            {
                slideshowTimer.Stop();
                slideshowButton.Image = Resources.Play_48x48;
                Screensaver.Reset();
            }
            else
            {
                slideshowTimer.Interval = AppSettings.SlideshowTransitionMs;
                slideshowButton.Image = Resources.Stop_48x48;
                slideshowTimer.Start();
                Screensaver.Disable();
            }
        }

        private void SettingsButton_Click(object? sender = null, EventArgs? e = null)
        {
            SettingsToolStripMenuItem_Click();
        }

        private void RotateImageButton_Click(object? sender = null, EventArgs? e = null)
        {
            if (slideshowTimer.Enabled)
            {
                MessageBox.Show(
                    "Slideshow mode is active. Stop slideshow first.",
                    "Slideshow mode is active",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // pictureBox.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            ImagesModel.Singleton.Image?.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pictureBox.Refresh();

            Text = $"{ImagesModel.Singleton.CurrentFilePath} [MODIFIED]";
            imageIsModified = true;
        }

        private void SettingsToolStripMenuItem_Click(object? sender = null, EventArgs? e = null)
        {
            using var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();

            if (mainPanel.BackColor != AppSettings.ThemeColor)
            {
                mainPanel.BackColor = AppSettings.ThemeColor;
                totalFilesLabel.ForeColor = AppSettings.LabelsColor;
            }

            if (AppSettings.GetRecentItemsFromRegistry().Count == 0)
            {
                ClearRecentItemsList();
            }
        }

        private void AboutToolStripMenuItem_Click(object? sender = null, EventArgs? e = null)
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
                    fileNameMenuItem.Text = $"--- {Path.GetFileName(ImagesModel.Singleton.CurrentFilePath)} ---";
                    rightClickMenuStrip.Show(this, new Point(e.X, e.Y)); // places the menu at the pointer position
                }
            }
        }

        private void CopyPathMenuItem_Click(object? sender = null, EventArgs? e = null)
        {
            try
            {
                Clipboard.SetDataObject(ImagesModel.Singleton.CurrentFilePath);
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
        }

        private void CopyFileMenuItem_Click(object? sender = null, EventArgs? e = null)
        {
            try
            {
                Clipboard.SetData(DataFormats.FileDrop, new string[] { ImagesModel.Singleton.CurrentFilePath });
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
        }

        private void MainForm_FormClosing(object? sender = null, FormClosingEventArgs? e = null)
        {
            CheckImageModified();
        }

        private long GetTotalAllocatedMemoryInBytes()
        {
            using var p = Process.GetCurrentProcess();
            return p.PrivateMemorySize64;
        }

        private double GetTotalAllocatedMemoryInMBytes()
        {
            return GetTotalAllocatedMemoryInBytes() / 1048576.0;
        }
    }
}
