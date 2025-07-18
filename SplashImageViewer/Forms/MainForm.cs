namespace SplashImageViewer.Forms;

public partial class MainForm : Form
{
    private readonly System.Windows.Forms.Timer slideshowTimer = new();
    private readonly System.Windows.Forms.Timer allocatedMemoryTimer = new();
    private readonly System.Windows.Forms.Timer slideshowProgressBarTimer = new();
    private IUpdater? updater;
    private DateTime nextSlideshowTransitionDate;
    private bool fullscreenFormIsActive;
    private bool imageIsModified;
    private bool eventsSubscribed;

    public MainForm()
    {
        this.InitializeComponent();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        switch (keyData)
        {
            case Keys.Left:
            case Keys.Down:
                if (this.previousButton.Enabled)
                {
                    this.SelectPreviousImage();
                }

                break;

            case Keys.Right:
            case Keys.Up:
                if (this.nextButton.Enabled)
                {
                    this.SelectNextImage();
                }

                break;

            case Keys.R:
                if (this.randomButton.Enabled)
                {
                    this.SelectRandomImage();
                }

                break;

            case Keys.Z:
                if (this.zoomButton.Enabled)
                {
                    this.ZoomingMode();
                }

                break;

            case Keys.S:
                if (this.settingsButton.Enabled)
                {
                    this.OpenSettings();
                }

                break;

            case Keys.D:
                if (this.rotateImageButton.Enabled)
                {
                    this.RotateImage();
                }

                break;

            case Keys.Space:
                if (this.slideshowButton.Enabled)
                {
                    this.StartStopSlideshow();
                }

                break;

            case Keys.F:
                if (this.fullscreenButton.Enabled)
                {
                    this.GoFullscreen();
                }

                break;

            case Keys.Delete:
                if (this.deleteFileButton.Enabled)
                {
                    this.DeleteImage();
                }

                break;

            case Keys.Escape:
                if (ImagesModel.Singleton.Image is not null)
                {
                    this.CloseImage();
                }

                break;

            default:
                break;
        }

        this.pictureBox.Focus();

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
            this.components.Dispose();
        }

        this.slideshowTimer.Dispose();
        this.allocatedMemoryTimer.Dispose();

        base.Dispose(disposing);
    }

    private static string GetFileSizeString(long bytes)
    {
        return bytes < 1024
            ? $"{bytes} {Resources.Byte}"
            : bytes < 1048576
                ? $"{bytes / 1024D:0.00} {Resources.KByte}"
                : $"{bytes / 1048576D:0.00} {Resources.MByte}";
    }

    private async void MainForm_Load(object sender, EventArgs e)
    {
        AppSettings.CheckSettings();

        // set current language culture
        System.Threading.Thread.CurrentThread.CurrentUICulture = AppSettings.CurrentUICulture;

        // ui controls translate
        this.LocalizeUIElements();

        if (!this.ScreenDimensionsIsValid())
        {
            this.Close();
        }

        this.RestoreScreenDimensions();

        this.programInfoLabel.Text = ApplicationInfo.AppHeader;

        // hide progress bar
        this.slideshowProgressBar.Visible = false;

        this.ClearImageValueLabels();

        this.mainPanel.BackColor = Color.FromArgb(AppSettings.ThemeColorArgb);
        this.totalFilesLabel.ForeColor = Color.FromArgb(AppSettings.LabelsColorArgb);

        this.SetControls(false);

        // change totalFilesLabel parent
        this.totalFilesLabel.Parent = this.pictureBox;
        this.totalFilesLabel.Location = new Point(
            this.pictureBox.Size.Width - this.totalFilesLabel.Size.Width,
            this.pictureBox.Size.Height - this.totalFilesLabel.Size.Height);

        this.UpdateTotalFilesLabel();
        this.InitTimers();

        // mouse wheel event handler
        MouseWheel += this.PictureBox_MouseWheel;

        // add recent items
        this.PopulateRecentItemsList();

        // check, if args exist
        if (ApplicationInfo.Args.Count > 0)
        {
            // cmd provided filename/folder path
            this.OpenImage(Path.GetFullPath(ApplicationInfo.Args[0]));
        }

        // init program updater
        this.InitUpdater();

        // check for updates
        await this.CheckUpdates();
    }

    private void LocalizeUIElements()
    {
        // set ui text
        this.fileToolStripMenuItem.Text = Resources.FileToolStripMenuItem;
        this.openImageMenuItem.Text = Resources.OpenImageMenuItem;
        this.openFolderMenuItem.Text = Resources.OpenFolderMenuItem;
        this.closeImageMenuItem.Text = Resources.CloseImageMenuItem;
        this.recentItemsMenuItem.Text = Resources.RecentItemsMenuItem;
        this.exitMenuItem.Text = Resources.ExitMenuItem;
        this.settingsToolStripMenuItem.Text = Resources.SettingsToolStripMenuItem;
        this.aboutToolStripMenuItem.Text = Resources.AboutToolStripMenuItem;

        this.showFileInFileExplorerMenuItem.Text = Resources.ShowFileInFileExplorer;
        this.copyPathMenuItem.Text = Resources.CopyFilePath;
        this.copyFileMenuItem.Text = Resources.CopyFileToClipboard;
        this.setDesktopBackgroundMenuItem.Text = Resources.SetAsDesktopBackground;

        this.imageDimensionsLabel.Text = $"{Resources.Dimensions}:";
        this.imageSizeLabel.Text = $"{Resources.FileSize}:";
        this.imageTypeLabel.Text = $"{Resources.Type}:";
        this.memoryAllocatedLabel.Text = $"{Resources.MemoryAllocated}:";

        // set tooltips
        this.toolTip.SetToolTip(this.previousButton, Resources.PreviousButtonToolTip);
        this.toolTip.SetToolTip(this.nextButton, Resources.NextButtonToolTip);
        this.toolTip.SetToolTip(this.slideshowButton, Resources.SlideshowButtonToolTip);
        this.toolTip.SetToolTip(this.randomButton, Resources.RandomButtonToolTip);
        this.toolTip.SetToolTip(this.deleteFileButton, Resources.DeleteButtonToolTip);
        this.toolTip.SetToolTip(this.fullscreenButton, Resources.FullscreenButtonToolTip);
        this.toolTip.SetToolTip(this.zoomButton, Resources.ZoomButtonToolTip);
        this.toolTip.SetToolTip(this.rotateImageButton, Resources.RotateImageButtonToolTip);
        this.toolTip.SetToolTip(this.settingsButton, Resources.OpenSettingsButtonToolTip);
    }

    private bool ScreenDimensionsIsValid()
    {
        // get current screen size
        var screen = Screen.FromControl(this).Bounds;

        if (screen.Width < AppSettings.MinScreenSizeWidth ||
            screen.Height < AppSettings.MinScreenSizeHeight)
        {
            MessageBox.Show(
                new Form { TopMost = true },
                Resources.CheckScreenDimensionsWarning,
                Resources.GeneralWarning,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }

        return true;
    }

    private void RestoreScreenDimensions()
    {
        if (AppSettings.ScreenIsMaximized)
        {
            this.WindowState = FormWindowState.Maximized;
            return;
        }

        this.WindowState = FormWindowState.Normal;
        this.Width = AppSettings.ScreenSizeWidth;
        this.Height = AppSettings.ScreenSizeHeight;

        // center form
        this.Location = new Point(
            (Screen.PrimaryScreen!.WorkingArea.Width - this.Width) / 2,
            (Screen.PrimaryScreen!.WorkingArea.Height - this.Height) / 2);
    }

    private void SaveScreenDimensions()
    {
        if (this.WindowState == FormWindowState.Maximized)
        {
            AppSettings.ScreenIsMaximized = true;
            return;
        }

        AppSettings.ScreenSizeWidth = this.Width;
        AppSettings.ScreenSizeHeight = this.Height;
        AppSettings.ScreenIsMaximized = false;
    }

    private void PopulateRecentItemsList()
    {
        // get saved values from registry and then populate ToolStripMenuItem
        foreach (string item in AppSettings.GetRecentItemsFromRegistry())
        {
            if (this.recentItemsMenuItem.DropDownItems.Count >= AppSettings.RecentItemsCapacity)
            {
                break;
            }

            if (File.Exists(item))
            {
                var menuItem = new ToolStripMenuItem { Text = item };
                menuItem.Click += this.OpenRecentItem_Click;
                this.recentItemsMenuItem.DropDownItems.Add(menuItem);
            }
        }

        this.recentItemsMenuItem.Enabled = this.recentItemsMenuItem.DropDownItems.Count > 0;
        this.WriteRecentItemsToRegistry();
    }

    private void ClearRecentItemsList()
    {
        for (int i = 0; i < this.recentItemsMenuItem.DropDownItems.Count; i++)
        {
            // unsubscribe event handlers first
            this.recentItemsMenuItem.DropDownItems[i].Click -= this.OpenRecentItem_Click;
        }

        this.recentItemsMenuItem.DropDownItems.Clear();
        this.recentItemsMenuItem.Enabled = false;
    }

    private void WriteRecentItemsToRegistry()
    {
        var items = new List<string>();

        for (int i = 0; i < this.recentItemsMenuItem.DropDownItems.Count; i++)
        {
            items.Add(this.recentItemsMenuItem.DropDownItems[i].Text!);
        }

        AppSettings.WriteRecentItemsToRegistry(items);
    }

    private void UpdateRecentItemsEvent(object sender)
    {
        // check if caller is on a different thread (invoke required)
        if (this.InvokeRequired)
        {
            this.Invoke(new System.Windows.Forms.MethodInvoker(this.UpdateRecentItems));
        }
        else
        {
            this.UpdateRecentItems();
        }
    }

    private void UpdateRecentItems()
    {
        // check if file paths exist (remove, if don't)
        for (int i = 0; i < this.recentItemsMenuItem.DropDownItems.Count; i++)
        {
            if (!File.Exists(this.recentItemsMenuItem.DropDownItems[i].Text))
            {
                // unsubscribe event handlers first
                this.recentItemsMenuItem.DropDownItems[i].Click -= this.OpenRecentItem_Click;
                this.recentItemsMenuItem.DropDownItems.RemoveAt(i);
                --i;
            }
        }

        string path = ImagesModel.Singleton.CurrentFilePath;
        bool addItem = true;

        // add / remove items
        for (int i = 0; i < this.recentItemsMenuItem.DropDownItems.Count; i++)
        {
            if (this.recentItemsMenuItem.DropDownItems[i].Text!.Equals(path))
            {
                addItem = false;
                break;
            }
        }

        if (addItem)
        {
            if (this.recentItemsMenuItem.DropDownItems.Count >= AppSettings.RecentItemsCapacity)
            {
                this.recentItemsMenuItem.DropDownItems[0].Click -= this.OpenRecentItem_Click;
                this.recentItemsMenuItem.DropDownItems.RemoveAt(0);
            }

            var menuItem = new ToolStripMenuItem { Text = path };
            menuItem.Click += this.OpenRecentItem_Click;
            this.recentItemsMenuItem.DropDownItems.Add(menuItem);
        }

        this.recentItemsMenuItem.Enabled = this.recentItemsMenuItem.DropDownItems.Count > 0;
        this.WriteRecentItemsToRegistry();
    }

    private void UpdatePictureBoxEvent(object sender)
    {
        // check if caller is on a different thread (invoke required)
        if (this.InvokeRequired)
        {
            this.Invoke(new System.Windows.Forms.MethodInvoker(this.UpdatePictureBox));
        }
        else
        {
            this.UpdatePictureBox();
        }
    }

    private void UpdatePictureBox()
    {
        // set picturebox image only, when fullscreen mode is not active
        if (!this.fullscreenFormIsActive)
        {
            if (ImagesModel.Singleton.FilePaths.Count == 0)
            {
                this.CloseImage();
                return;
            }

            try
            {
                // load image async in background
                //await Task.Run(() => ImagesModel.Singleton.LoadImage());
                // loading image asynchronously can lead to a racing condition
                ImagesModel.Singleton.LoadImage();
            }
            catch (Exception ex)
            {
                this.ShowExceptionMessage(ex);
                return;
            }

            if (ImagesModel.Singleton.Image is not null)
            {
                this.pictureBox.Image = ImagesModel.Singleton.Image;
                this.UpdateFilePathText();
                this.UpdateImageLabels();
                this.ZoomOut();
                this.ModifyPictureBoxSizeMode();
                this.UpdateTotalFilesLabel();
                this.SetControls(true);
            }
        }
    }

    private void SubscribeEvents()
    {
        if (!this.eventsSubscribed)
        {
            ImagesModel.Singleton.CurrentFilePathIndexChanged += this.UpdateRecentItemsEvent;
            ImagesModel.Singleton.CurrentFilePathIndexChanged += this.UpdatePictureBoxEvent;
            this.eventsSubscribed = true;
        }

        this.UpdateRecentItems();
        this.UpdatePictureBox();
    }

    private void UnsubscribeEvents()
    {
        if (this.eventsSubscribed)
        {
            ImagesModel.Singleton.CurrentFilePathIndexChanged -= this.UpdateRecentItemsEvent;
            ImagesModel.Singleton.CurrentFilePathIndexChanged -= this.UpdatePictureBoxEvent;
            this.eventsSubscribed = false;
        }
    }

    private void PictureBox_MouseWheel(object? sender, MouseEventArgs e)
    {
        if (e.Delta > 0)
        {
            this.ZoomIn();
        }
        else if (e.Delta < 0)
        {
            this.ZoomOut();
        }
    }

    private void ZoomIn()
    {
        if (this.pictureBox.Image is not null)
        {
            if (this.mainPanel.ClientSize.Width < this.pictureBox.Image.Width ||
                this.mainPanel.ClientSize.Height < this.pictureBox.Image.Height)
            {
                this.pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
                this.pictureBox.Size = new Size(this.pictureBox.Image.Width, this.pictureBox.Image.Height);
                this.pictureBox.Dock = DockStyle.None;
                this.zoomButton.Image = Resources.ZoomOut_48x48;

                // center panel scroll bars (centered zoom in effect)
                this.mainPanel.AutoScrollPosition = new Point(
                    (this.pictureBox.Width / 2) - (this.mainPanel.ClientSize.Width / 2),
                    (this.pictureBox.Height / 2) - (this.mainPanel.ClientSize.Height / 2));
            }
        }
    }

    private void ZoomOut()
    {
        if (this.pictureBox.Image is not null)
        {
            if (this.mainPanel.ClientSize.Width < this.pictureBox.Image.Width ||
            this.mainPanel.ClientSize.Height < this.pictureBox.Image.Height)
            {
                this.pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                this.pictureBox.Size = new Size(this.mainPanel.Size.Width, this.mainPanel.Size.Height);
                this.pictureBox.Dock = DockStyle.Fill;
                this.zoomButton.Image = Resources.ZoomIn_48x48;
            }
        }
    }

    private void InitTimers()
    {
        this.slideshowTimer.Tick += this.SlideshowHandler;

        this.allocatedMemoryTimer.Tick += this.CheckMemoryAllocated;
        this.allocatedMemoryTimer.Interval = AppSettings.MainFormCheckMemoryMs;
        this.allocatedMemoryTimer.Start();

        this.slideshowProgressBarTimer.Tick += this.SlideshowProgressBarHandler;
        this.slideshowProgressBarTimer.Interval = AppSettings.MainFormSlideshowProgressBarUpdateMs;
    }

    private void InitUpdater()
    {
        try
        {
            this.updater = new Updater(
                ApplicationInfo.BaseDirectory,
                Version.Parse(""),
                ApplicationInfo.AppGUID,
                ApplicationInfo.ExePath);
        }
        catch (Exception ex)
        {
            this.ShowExceptionMessage(ex);
        }
    }

    private async Task CheckUpdates()
    {
        if (this.updater is null)
        {
            return;
        }

        try
        {
            this.aboutToolStripMenuItem.Enabled = false;

            if ((DateTime.Now - AppSettings.UpdatesLastCheckedTimestamp).Days >= 1 ||
                AppSettings.ForceCheckUpdates)
            {
                AppSettings.UpdateUpdatesLastCheckedTimestamp();

                if (await this.updater.CheckUpdateIsAvailable())
                {
                    var dr = MessageBox.Show(
                        new Form { TopMost = true },
                        $"{Resources.NewerProgramVersionAvailable}{Environment.NewLine}" +
                        $"{Resources.Current}: {this.updater.ClientVersion}{Environment.NewLine}" +
                        $"{Resources.Available}: {this.updater.ServerVersion}",
                        Resources.ProgramUpdate,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }
        catch (Exception ex)
        {
            this.ShowExceptionMessage(ex);
        }
        finally
        {
            this.aboutToolStripMenuItem.Enabled = true;
        }
    }

    private void CheckMemoryAllocated(object? sender, EventArgs e) => this.memoryAllocatedValueLabel.Text = $"{Utils.GetTotalAllocatedMemoryInBytes() / 1048576D:0.00} {Resources.MByte}";

    private void ShowExceptionMessage(Exception ex)
    {
        this.pictureBox.Image = Resources.Error_64x64;
        this.ZoomOut();
        this.ModifyPictureBoxSizeMode();

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
            this.UnsubscribeEvents();
            ImagesModel.Singleton.DisposeResources();
            ImagesModel.Singleton.Init(path, AppSettings.SearchInSubdirs);
            this.SubscribeEvents();
        }
        catch (Exception ex)
        {
            this.ShowExceptionMessage(ex);
        }
    }

    private void CloseImage()
    {
        this.CheckImageModified();

        if (this.slideshowTimer.Enabled)
        {
            this.slideshowProgressBar.Visible = false;
            this.slideshowTimer.Stop();
            this.slideshowProgressBarTimer.Stop();
            Screensaver.Reset();
        }

        if (this.pictureBox.Dock == DockStyle.None)
        {
            this.ZoomOut();
        }

        this.slideshowButton.Image = Resources.Play_48x48;
        this.SetControls(false);
        this.UnsubscribeEvents();
        ImagesModel.Singleton.DisposeResources();
        this.pictureBox.Image = null;
        this.pictureBox.Focus();
        this.UpdateFilePathText();
        this.ClearImageValueLabels();
        this.UpdateTotalFilesLabel();
        GC.Collect();
    }

    private void UpdateFilePathText()
    {
        if (this.slideshowTimer.Enabled)
        {
            this.Text = $"{ImagesModel.Singleton.CurrentFilePath} - {Resources.SlideshowEnabled}";
        }
        else if (this.imageIsModified)
        {
            this.Text = $"{ImagesModel.Singleton.CurrentFilePath} - {Resources.ImageModifiedCaps}";
        }
        else if (ImagesModel.Singleton.Image is not null)
        {
            this.Text = ImagesModel.Singleton.CurrentFilePath;
        }
        else
        {
            // default text
            this.Text = ApplicationInfo.AppProduct;
        }
    }

    private void ClearImageValueLabels()
    {
        this.imageDimensionsValueLabel.Text = string.Empty;
        this.imageSizeValueLabel.Text = string.Empty;
        this.imageTypeValueLabel.Text = string.Empty;
    }

    private void UpdateImageLabels()
    {
        if (ImagesModel.Singleton.Image is not null)
        {
            this.imageDimensionsValueLabel.Text = $"{ImagesModel.Singleton.Image.Width}x{ImagesModel.Singleton.Image.Height}";
            this.imageSizeValueLabel.Text = $"{GetFileSizeString(new FileInfo(ImagesModel.Singleton.CurrentFilePath).Length)}";
            this.imageTypeValueLabel.Text = $"{ImagesModel.Singleton.ImageFormatDescription}";
        }
    }

    private void UpdateTotalFilesLabel()
    {
        this.totalFilesLabel.Text = ImagesModel.Singleton.Image is not null ?
            $"{ImagesModel.Singleton.CurrentFilePathIndex + 1} / {ImagesModel.Singleton.FilePaths.Count}" :
            string.Empty;
    }

    private void SetControls(bool state)
    {
        this.slideshowButton.Enabled = state;
        this.previousButton.Enabled = state;
        this.nextButton.Enabled = state;
        this.randomButton.Enabled = state;
        this.fullscreenButton.Enabled = state;
        this.rotateImageButton.Enabled = state;
        this.deleteFileButton.Enabled = state;
        this.zoomButton.Enabled = state;
        this.closeImageMenuItem.Enabled = state;

        // enable/disable right click context menu
        this.rightClickMenuStrip.Enabled = state;
    }

    private void MainPanel_Resize(object sender, EventArgs e) => this.ModifyPictureBoxSizeMode();

    private void ModifyPictureBoxSizeMode()
    {
        if (this.pictureBox.Image is not null)
        {
            this.pictureBox.SizeMode = this.mainPanel.ClientSize.Width < this.pictureBox.Image.Width ||
            this.mainPanel.ClientSize.Height < this.pictureBox.Image.Height
                ? PictureBoxSizeMode.Zoom
                : PictureBoxSizeMode.CenterImage;
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

        this.SetNextSlideshowTransitionDate();
    }

    private void SetNextSlideshowTransitionDate() => this.nextSlideshowTransitionDate = DateTime.Now.AddMilliseconds(this.slideshowTimer.Interval);

    private void SlideshowProgressBarHandler(object? sender, EventArgs e)
    {
        int progress = (int)((this.slideshowTimer.Interval - (this.nextSlideshowTransitionDate - DateTime.Now).TotalMilliseconds) / this.slideshowTimer.Interval * this.slideshowProgressBar.Maximum);

        // progress bar value synchronization hack
        if (progress < this.slideshowProgressBar.Maximum - this.slideshowProgressBar.Step)
        {
            this.slideshowProgressBar.Value = progress + this.slideshowProgressBar.Step;
            this.slideshowProgressBar.Value = progress;
        }
        else
        {
            this.slideshowProgressBar.Value = this.slideshowProgressBar.Maximum - this.slideshowProgressBar.Step;
            this.slideshowProgressBar.Value = this.slideshowProgressBar.Maximum;
        }
    }

    private void GoFullscreen()
    {
        this.CheckImageModified();
        this.fullscreenFormIsActive = true;
        Cursor.Hide();

        // get current display location/bounds, so that fullscreen form is displayed on the same display as the main form
        var screen = Screen.FromControl(this);

        try
        {
            using var fullscreenForm = new FullscreenForm(screen, this.slideshowTimer.Enabled);
            fullscreenForm.ShowDialog();
        }
        catch (Exception ex)
        {
            Cursor.Show();
            this.ShowExceptionMessage(ex);
        }

        this.fullscreenFormIsActive = false;
        Cursor.Show();
        this.UpdatePictureBox();
    }

    private void CheckImageModified()
    {
        if (this.imageIsModified)
        {
            this.OverwriteImage();
            this.imageIsModified = false;
            this.UpdateFilePathText();
        }
    }

    private void OverwriteImage()
    {
        if (AppSettings.ShowFileOverwritePrompt)
        {
            var dr = MessageBox.Show(
                new Form { TopMost = true },
                Resources.OverwriteImagePrompt,
                Resources.ImageModified,
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
                    this.ShowExceptionMessage(ex);
                }
            }
        }
    }

    private void OpenImage_Click(object sender, EventArgs e)
    {
        this.CheckImageModified();

        using var ofd = new OpenFileDialog() { Filter = "Image Files|*.jpg; *.jpeg; *.jpe; *.jfif; *.bmp; *.png; *.gif; *.ico; *.tif|All Files|*.*" };

        if (ofd.ShowDialog() == DialogResult.OK)
        {
            this.OpenImage(ofd.FileName);
        }
    }

    private void OpenFolder_Click(object sender, EventArgs e)
    {
        this.CheckImageModified();

        using var fbd = new FolderBrowserDialog();

        if (fbd.ShowDialog() == DialogResult.OK)
        {
            this.OpenImage(fbd.SelectedPath);
        }
    }

    private void CloseImage_Click(object sender, EventArgs e) => this.CloseImage();

    private void OpenRecentItem_Click(object? sender, EventArgs e)
    {
        this.CheckImageModified();

        if (sender is ToolStripMenuItem item)
        {
            this.OpenImage(item.Text!);
        }
    }

    private void Exit_Click(object sender, EventArgs e)
    {
        this.CheckImageModified();
        this.Close();
    }

    private void DeleteFileButton_Click(object sender, EventArgs e) => this.DeleteImage();

    private void DeleteImage()
    {
        this.CheckImageModified();

        if (AppSettings.ShowFileDeletePrompt)
        {
            var dialogResult = MessageBox.Show(
                new Form { TopMost = true },
                Resources.FileDeletePrompt,
                Resources.DeleteFile,
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
            this.ShowExceptionMessage(ex);
        }
    }

    private void ZoomButton_Click(object sender, EventArgs e) => this.ZoomingMode();

    private void ZoomingMode()
    {
        if (this.pictureBox.Dock == DockStyle.None)
        {
            this.ZoomOut();
        }
        else
        {
            this.ZoomIn();
        }
    }

    private void FullscreenButton_Click(object sender, EventArgs e) => this.GoFullscreen();

    private void PreviousButton_Click(object sender, EventArgs e) => this.SelectPreviousImage();

    private void SelectPreviousImage()
    {
        this.CheckImageModified();
        ImagesModel.Singleton.SelectPreviousImageIndex();
    }

    private void NextButton_Click(object sender, EventArgs e) => this.SelectNextImage();

    private void SelectNextImage()
    {
        this.CheckImageModified();
        ImagesModel.Singleton.SelectNextImageIndex();
    }

    private void RandomButton_Click(object sender, EventArgs e) => this.SelectRandomImage();

    private void SelectRandomImage()
    {
        this.CheckImageModified();
        ImagesModel.Singleton.SelectRandomImageIndex();
    }

    private void SlideshowButton_Click(object sender, EventArgs e) => this.StartStopSlideshow();

    private void StartStopSlideshow()
    {
        this.CheckImageModified();

        if (this.slideshowTimer.Enabled)
        {
            this.slideshowProgressBar.Visible = false;
            this.slideshowTimer.Stop();
            this.slideshowProgressBarTimer.Stop();
            this.slideshowButton.Image = Resources.Play_48x48;
            Screensaver.Reset();
        }
        else
        {
            if (ImagesModel.Singleton.FilePaths.Count > 1)
            {
                this.slideshowTimer.Interval = AppSettings.SlideshowTransitionSec * 1000;
                this.slideshowButton.Image = Resources.Stop_48x48;
                this.slideshowProgressBar.Value = this.slideshowProgressBar.Minimum;
                this.slideshowProgressBar.Visible = true;
                this.SetNextSlideshowTransitionDate();
                this.slideshowTimer.Start();
                this.slideshowProgressBarTimer.Start();
                Screensaver.Disable();
            }
        }

        this.UpdateFilePathText();
    }

    private void SettingsButton_Click(object sender, EventArgs e) => this.OpenSettings();

    private void RotateImageButton_Click(object sender, EventArgs e) => this.RotateImage();

    private void RotateImage()
    {
        if (this.slideshowTimer.Enabled)
        {
            MessageBox.Show(
                new Form { TopMost = true },
                Resources.StopSlideshowFirstPrompt,
                Resources.SlideshowModeIsActive,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        if (ImagesModel.Singleton.Image is not null)
        {
            // pictureBox.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            ImagesModel.Singleton.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            this.pictureBox.Refresh();
            this.imageIsModified = true;
            this.UpdateFilePathText();
        }
    }

    private void SettingsToolStripMenuItem_Click(object sender, EventArgs e) => this.OpenSettings();

    private void OpenSettings()
    {
        using var settingsForm = new SettingsForm();
        settingsForm.ShowDialog();

        if (settingsForm.DefaultSettingsRestored)
        {
            this.RestoreScreenDimensions();
        }

        var color = Color.FromArgb(AppSettings.ThemeColorArgb);

        if (this.mainPanel.BackColor != color)
        {
            this.mainPanel.BackColor = color;
            this.totalFilesLabel.ForeColor = Color.FromArgb(AppSettings.LabelsColorArgb);
        }

        if (AppSettings.GetRecentItemsFromRegistry().Count == 0)
        {
            this.ClearRecentItemsList();
        }

        if (System.Threading.Thread.CurrentThread.CurrentUICulture != AppSettings.CurrentUICulture)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = AppSettings.CurrentUICulture;
            this.LocalizeUIElements();
        }
    }

    private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var aboutForm = new AboutForm(this.updater);
        aboutForm.ShowDialog();
    }

    private void PictureBox_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            if (this.rightClickMenuStrip.Enabled)
            {
                this.fileNameMenuItem.Text = $"--- {ImagesModel.Singleton.CurrentFileName} ---";
                this.rightClickMenuStrip.Show(this, new Point(e.X, e.Y)); // places the menu at the pointer position
            }
        }
    }

    private void ShowFileInFileExplorerMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            Utils.OpenExplorer(ImagesModel.Singleton.CurrentFilePath);
        }
        catch (Exception ex)
        {
            this.ShowExceptionMessage(ex);
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
            this.ShowExceptionMessage(ex);
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
            this.ShowExceptionMessage(ex);
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
            this.ShowExceptionMessage(ex);
        }
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        this.CheckImageModified();
        this.SaveScreenDimensions();
    }
}
