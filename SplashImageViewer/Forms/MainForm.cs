namespace SplashImageViewer.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using SplashImageViewer.Helpers;
    using SplashImageViewer.Models;
    using SplashImageViewer.Properties;

    public partial class MainForm : Form
    {
        private readonly Timer slideshowTimer = new Timer();
        private readonly Timer allocatedMemoryTimer = new Timer();
        private bool fullscreenFormIsActive;
        private bool imageIsModified;
        private bool eventsSubscribed;

        public MainForm()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                case Keys.Down:
                    if (previousButton.Enabled)
                    {
                        SelectPreviousImage();
                    }

                    break;

                case Keys.Right:
                case Keys.Up:
                    if (nextButton.Enabled)
                    {
                        SelectNextImage();
                    }

                    break;

                case Keys.R:
                    if (randomButton.Enabled)
                    {
                        SelectRandomImage();
                    }

                    break;

                case Keys.Z:
                    if (zoomButton.Enabled)
                    {
                        ZoomingMode();
                    }

                    break;

                case Keys.S:
                    if (settingsButton.Enabled)
                    {
                        OpenSettings();
                    }

                    break;

                case Keys.D:
                    if (rotateImageButton.Enabled)
                    {
                        RotateImage();
                    }

                    break;

                case Keys.Space:
                    if (slideshowButton.Enabled)
                    {
                        StartStopSlideshow();
                    }

                    break;

                case Keys.F:
                    if (fullscreenButton.Enabled)
                    {
                        GoFullscreen();
                    }

                    break;

                case Keys.Delete:
                    if (deleteFileButton.Enabled)
                    {
                        DeleteImage();
                    }

                    break;

                case Keys.Escape:
                    if (pictureBox.Image is not null)
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
            if (disposing && (components is not null))
            {
                components.Dispose();
            }

            slideshowTimer.Dispose();
            allocatedMemoryTimer.Dispose();

            base.Dispose(disposing);
        }

        private static string GetFileSizeString(long bytes)
        {
            if (bytes < 1024)
            {
                return $"{bytes} {Strings.Byte}";
            }
            else if (bytes < 1048576)
            {
                return $"{bytes / 1024D:0.00} {Strings.KByte}";
            }
            else
            {
                return $"{bytes / 1048576D:0.00} {Strings.MByte}";
            }
        }

        private static long GetTotalAllocatedMemoryInBytes()
        {
            using var p = Process.GetCurrentProcess();
            return p.PrivateMemorySize64;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            AppSettings.CheckSettings();

            // set current language culture
            System.Threading.Thread.CurrentThread.CurrentUICulture = AppSettings.CurrentUICulture;

            // ui controls translate
            LocalizeUIElements();

            CheckScreenDimensions();

            programInfoLabel.Text = ApplicationInfo.AppHeader;
            imageDimensionsLabel.Text = string.Empty;

            // check for updates in the background
            Task.Run(async () => await CheckUpdates());

            mainPanel.BackColor = Color.FromArgb(AppSettings.ThemeColorArgb);
            totalFilesLabel.ForeColor = Color.FromArgb(AppSettings.LabelsColorArgb);

            SetControls(false);

            // change totalFilesLabel parent
            totalFilesLabel.Parent = pictureBox;
            totalFilesLabel.Location = new Point(
                pictureBox.Size.Width - totalFilesLabel.Size.Width,
                pictureBox.Size.Height - totalFilesLabel.Size.Height);

            UpdateTotalFilesLabel();
            InitTimers();

            // mouse wheel event handler
            MouseWheel += PictureBox_MouseWheel;

            // add recent items
            PopulateRecentItemsList();

            // check, if args exist
            if (ApplicationInfo.Args.Count > 0)
            {
                // cmd provided filename/folder path
                OpenImage(Path.GetFullPath(ApplicationInfo.Args.First()));
            }
        }

        private void LocalizeUIElements()
        {
            // set ui text
            fileToolStripMenuItem.Text = Strings.FileToolStripMenuItem;
            openImageMenuItem.Text = Strings.OpenImageMenuItem;
            openFolderMenuItem.Text = Strings.OpenFolderMenuItem;
            closeImageMenuItem.Text = Strings.CloseImageMenuItem;
            recentItemsMenuItem.Text = Strings.RecentItemsMenuItem;
            exitMenuItem.Text = Strings.ExitMenuItem;
            settingsToolStripMenuItem.Text = Strings.SettingsToolStripMenuItem;
            aboutToolStripMenuItem.Text = Strings.AboutToolStripMenuItem;

            // set tooltips
            toolTip.SetToolTip(previousButton, Strings.PreviousButtonToolTip);
            toolTip.SetToolTip(nextButton, Strings.NextButtonToolTip);
            toolTip.SetToolTip(slideshowButton, Strings.SlideshowButtonToolTip);
            toolTip.SetToolTip(randomButton, Strings.RandomButtonToolTip);
            toolTip.SetToolTip(deleteFileButton, Strings.DeleteButtonToolTip);
            toolTip.SetToolTip(fullscreenButton, Strings.FullscreenButtonToolTip);
            toolTip.SetToolTip(zoomButton, Strings.ZoomButtonToolTip);
            toolTip.SetToolTip(rotateImageButton, Strings.RotateImageButtonToolTip);
            toolTip.SetToolTip(settingsButton, Strings.OpenSettingsButtonToolTip);
        }

        private void CheckScreenDimensions()
        {
            // get current screen size
            var screen = Screen.FromControl(this).Bounds;

            if (screen.Width < AppSettings.MinScreenSizeWidth || screen.Height < AppSettings.MinScreenSizeHeight)
            {
                MessageBox.Show(
                    new Form { TopMost = true },
                    Strings.CheckScreenDimensionsWarning,
                    Strings.GeneralWarning,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

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

                if (ImagesModel.Singleton.Image is not null)
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

        private void PictureBox_MouseWheel(object? sender, MouseEventArgs e)
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

        private void ZoomIn()
        {
            if (pictureBox.Image is not null)
            {
                if (mainPanel.ClientSize.Width < pictureBox.Image.Width ||
                    mainPanel.ClientSize.Height < pictureBox.Image.Height)
                {
                    pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
                    pictureBox.Size = new Size(pictureBox.Image.Width, pictureBox.Image.Height);
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
            if (pictureBox.Image is not null)
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
        }

        private void InitTimers()
        {
            slideshowTimer.Tick += SlideshowHandler;

            allocatedMemoryTimer.Tick += CheckMemoryAllocated;
            allocatedMemoryTimer.Interval = AppSettings.MainFormCheckMemoryMs;
            allocatedMemoryTimer.Start();
        }

        private async Task CheckUpdates()
        {
            if ((DateTime.UtcNow - AppSettings.UpdatesLastCheckedUtcTimestamp).Days >= 1 ||
                AppSettings.ForceCheckUpdates)
            {
                try
                {
                    AppSettings.UpdateUpdatesLastCheckedUtcTimestamp();

                    if (await ProgramUpdater.CheckUpdateIsAvailable())
                    {
                        var dr = MessageBox.Show(
                            new Form { TopMost = true },
                            ProgramUpdater.UpdatePromptFormatted,
                            Strings.ProgramUpdate,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (dr == DialogResult.Yes)
                        {
                            await ProgramUpdater.Update();
                            Program.ProgramExit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowExceptionMessage(ex);
                }
            }
        }

        private void CheckMemoryAllocated(object? sender, EventArgs e)
        {
            memoryAllocatedLabel.Text = $"{Strings.MemoryAllocated}: {GetTotalAllocatedMemoryInBytes() / 1048576D:0.00} {Strings.MByte}";
        }

        private void ShowExceptionMessage(Exception ex)
        {
            pictureBox.Image = Resources.Error_64x64;
            ZoomOut();
            ModifyPictureBoxSizeMode();

            MessageBox.Show(
                new Form { TopMost = true },
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
            Text = ApplicationInfo.AppProduct;
            imageDimensionsLabel.Text = string.Empty;
            UpdateTotalFilesLabel();
            GC.Collect();
        }

        private void UpdateFilePathText()
        {
            Text = ImagesModel.Singleton.CurrentFilePath;
        }

        private void UpdateImageDimensionsLabel()
        {
            imageDimensionsLabel.Text = $"{Strings.Dimensions}: {ImagesModel.Singleton.Image?.Width}x{ImagesModel.Singleton.Image?.Height}  " +
                $"{Strings.FileSize}: {GetFileSizeString(new FileInfo(ImagesModel.Singleton.CurrentFilePath).Length)}  " +
                $"{Strings.Type}: {ImagesModel.Singleton.ImageFormatDescription}";
        }

        private void UpdateTotalFilesLabel()
        {
            totalFilesLabel.Text = pictureBox.Image is not null ?
                $"{ImagesModel.Singleton.CurrentFilePathIndex + 1} / {ImagesModel.Singleton.FilePaths.Count}" :
                "0 / 0";
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
            if (pictureBox.Image is not null)
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
            CheckImageModified();
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
            UpdatePictureBox();
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
                    new Form { TopMost = true },
                    Strings.OverwriteImagePrompt,
                    Strings.ImageModified,
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

        private void OpenImage_Click(object sender, EventArgs e)
        {
            CheckImageModified();

            using var ofd = new OpenFileDialog() { Filter = "Image Files|*.jpg; *.jpeg; *.jpe; *.jfif; *.bmp; *.png; *.gif; *.ico; *.tif|All Files|*.*" };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                OpenImage(ofd.FileName);
            }
        }

        private void OpenFolder_Click(object sender, EventArgs e)
        {
            CheckImageModified();

            using var fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                OpenImage(fbd.SelectedPath);
            }
        }

        private void CloseImage_Click(object sender, EventArgs e)
        {
            CheckImageModified();
            CloseImage();
        }

        private void OpenRecentItem_Click(object? sender, EventArgs e)
        {
            CheckImageModified();

            if (sender is ToolStripMenuItem item)
            {
                OpenImage(item.Text);
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            CheckImageModified();
            Close();
        }

        private void DeleteFileButton_Click(object sender, EventArgs e)
        {
            DeleteImage();
        }

        private void DeleteImage()
        {
            CheckImageModified();

            if (AppSettings.ShowFileDeletePrompt)
            {
                var dialogResult = MessageBox.Show(
                    new Form { TopMost = true },
                    Strings.FileDeletePrompt,
                    Strings.DeleteFile,
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

        private void ZoomButton_Click(object sender, EventArgs e)
        {
            ZoomingMode();
        }

        private void ZoomingMode()
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

        private void FullscreenButton_Click(object sender, EventArgs e)
        {
            GoFullscreen();
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            SelectPreviousImage();
        }

        private void SelectPreviousImage()
        {
            CheckImageModified();
            ImagesModel.Singleton.SelectPreviousImageIndex();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            SelectNextImage();
        }

        private void SelectNextImage()
        {
            CheckImageModified();
            ImagesModel.Singleton.SelectNextImageIndex();
        }

        private void RandomButton_Click(object sender, EventArgs e)
        {
            SelectRandomImage();
        }

        private void SelectRandomImage()
        {
            CheckImageModified();
            ImagesModel.Singleton.SelectRandomImageIndex();
        }

        private void SlideshowButton_Click(object sender, EventArgs e)
        {
            StartStopSlideshow();
        }

        private void StartStopSlideshow()
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
                slideshowTimer.Interval = AppSettings.SlideshowTransitionSec * 1000;
                slideshowButton.Image = Resources.Stop_48x48;
                slideshowTimer.Start();
                Screensaver.Disable();
            }
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            OpenSettings();
        }

        private void RotateImageButton_Click(object sender, EventArgs e)
        {
            RotateImage();
        }

        private void RotateImage()
        {
            if (slideshowTimer.Enabled)
            {
                MessageBox.Show(
                    new Form { TopMost = true },
                    Strings.StopSlideshowFirstPrompt,
                    Strings.SlideshowModeIsActive,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            // pictureBox.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            ImagesModel.Singleton.Image?.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pictureBox.Refresh();

            Text = $"{ImagesModel.Singleton.CurrentFilePath} [{Strings.ModifiedCaps}]";
            imageIsModified = true;
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSettings();
        }

        private void OpenSettings()
        {
            using var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();

            var color = Color.FromArgb(AppSettings.ThemeColorArgb);

            if (mainPanel.BackColor != color)
            {
                mainPanel.BackColor = color;
                totalFilesLabel.ForeColor = Color.FromArgb(AppSettings.LabelsColorArgb);
            }

            if (AppSettings.GetRecentItemsFromRegistry().Count == 0)
            {
                ClearRecentItemsList();
            }

            if (System.Threading.Thread.CurrentThread.CurrentUICulture != AppSettings.CurrentUICulture)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = AppSettings.CurrentUICulture;
                LocalizeUIElements();
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
                    fileNameMenuItem.Text = $"--- {ImagesModel.Singleton.CurrentFileName} ---";
                    rightClickMenuStrip.Show(this, new Point(e.X, e.Y)); // places the menu at the pointer position
                }
            }
        }

        private void CopyPathMenuItem_Click(object sender, EventArgs e)
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

        private void CopyFileMenuItem_Click(object sender, EventArgs e)
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

        private void SetDesktopBackgroundMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (ImagesModel.Singleton.Image is not null)
                {
                    Wallpaper.SetDesktopBackground(ImagesModel.Singleton.Image);
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CheckImageModified();
        }
    }
}
