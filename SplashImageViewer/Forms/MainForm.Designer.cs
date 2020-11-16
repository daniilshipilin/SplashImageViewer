using System.Windows.Forms;

namespace SplashImageViewer.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }

        //    base.Dispose(disposing);
        //}

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFolderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.recentItemsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.imageDimensionsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.imageSizeLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.imageTypeLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.memoryAllocatedLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.programInfoLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.totalFilesLabel = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.nextButton = new System.Windows.Forms.Button();
            this.slideshowButton = new System.Windows.Forms.Button();
            this.previousButton = new System.Windows.Forms.Button();
            this.deleteFileButton = new System.Windows.Forms.Button();
            this.fullscreenButton = new System.Windows.Forms.Button();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.zoomButton = new System.Windows.Forms.Button();
            this.randomButton = new System.Windows.Forms.Button();
            this.settingsButton = new System.Windows.Forms.Button();
            this.rotateImageButton = new System.Windows.Forms.Button();
            this.rightClickMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fileNameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.copyPathMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setDesktopBackgroundMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.rightClickMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(5);
            this.menuStrip.Size = new System.Drawing.Size(1008, 29);
            this.menuStrip.TabIndex = 0;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openImageMenuItem,
            this.openFolderMenuItem,
            this.toolStripSeparator1,
            this.closeImageMenuItem,
            this.toolStripSeparator2,
            this.recentItemsMenuItem,
            this.toolStripSeparator3,
            this.exitMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 19);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openImageMenuItem
            // 
            this.openImageMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openImageMenuItem.Image")));
            this.openImageMenuItem.Name = "openImageMenuItem";
            this.openImageMenuItem.Size = new System.Drawing.Size(146, 26);
            this.openImageMenuItem.Text = "&Open Image";
            this.openImageMenuItem.Click += new System.EventHandler(this.OpenImage_Click);
            // 
            // openFolderMenuItem
            // 
            this.openFolderMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openFolderMenuItem.Image")));
            this.openFolderMenuItem.Name = "openFolderMenuItem";
            this.openFolderMenuItem.Size = new System.Drawing.Size(146, 26);
            this.openFolderMenuItem.Text = "Open &Folder";
            this.openFolderMenuItem.Click += new System.EventHandler(this.OpenFolder_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(143, 6);
            // 
            // closeImageMenuItem
            // 
            this.closeImageMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("closeImageMenuItem.Image")));
            this.closeImageMenuItem.Name = "closeImageMenuItem";
            this.closeImageMenuItem.Size = new System.Drawing.Size(146, 26);
            this.closeImageMenuItem.Text = "&Close Image";
            this.closeImageMenuItem.Click += new System.EventHandler(this.CloseImage_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(143, 6);
            // 
            // recentItemsMenuItem
            // 
            this.recentItemsMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("recentItemsMenuItem.Image")));
            this.recentItemsMenuItem.Name = "recentItemsMenuItem";
            this.recentItemsMenuItem.Size = new System.Drawing.Size(146, 26);
            this.recentItemsMenuItem.Text = "&Recent Items";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(143, 6);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exitMenuItem.Image")));
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(146, 26);
            this.exitMenuItem.Text = "&Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.Exit_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 19);
            this.settingsToolStripMenuItem.Text = "&Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.SettingsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 19);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(1008, 610);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            this.pictureBox.WaitOnLoad = true;
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox_MouseDown);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.imageDimensionsLabel,
            this.imageSizeLabel,
            this.imageTypeLabel,
            this.memoryAllocatedLabel,
            this.programInfoLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 707);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1008, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip";
            // 
            // imageDimensionsLabel
            // 
            this.imageDimensionsLabel.Name = "imageDimensionsLabel";
            this.imageDimensionsLabel.Size = new System.Drawing.Size(198, 17);
            this.imageDimensionsLabel.Spring = true;
            this.imageDimensionsLabel.Text = "imageDimensionsLabel";
            this.imageDimensionsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // imageSizeLabel
            // 
            this.imageSizeLabel.Name = "imageSizeLabel";
            this.imageSizeLabel.Size = new System.Drawing.Size(198, 17);
            this.imageSizeLabel.Spring = true;
            this.imageSizeLabel.Text = "imageSizeLabel";
            this.imageSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // imageTypeLabel
            // 
            this.imageTypeLabel.Name = "imageTypeLabel";
            this.imageTypeLabel.Size = new System.Drawing.Size(198, 17);
            this.imageTypeLabel.Spring = true;
            this.imageTypeLabel.Text = "imageTypeLabel";
            this.imageTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // memoryAllocatedLabel
            // 
            this.memoryAllocatedLabel.Name = "memoryAllocatedLabel";
            this.memoryAllocatedLabel.Size = new System.Drawing.Size(198, 17);
            this.memoryAllocatedLabel.Spring = true;
            this.memoryAllocatedLabel.Text = "memoryAllocatedLabel";
            this.memoryAllocatedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // programInfoLabel
            // 
            this.programInfoLabel.Name = "programInfoLabel";
            this.programInfoLabel.Size = new System.Drawing.Size(198, 17);
            this.programInfoLabel.Spring = true;
            this.programInfoLabel.Text = "programInfoLabel";
            this.programInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // totalFilesLabel
            // 
            this.totalFilesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.totalFilesLabel.BackColor = System.Drawing.Color.Transparent;
            this.totalFilesLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.totalFilesLabel.ForeColor = System.Drawing.Color.White;
            this.totalFilesLabel.Location = new System.Drawing.Point(858, 560);
            this.totalFilesLabel.Margin = new System.Windows.Forms.Padding(0);
            this.totalFilesLabel.Name = "totalFilesLabel";
            this.totalFilesLabel.Size = new System.Drawing.Size(150, 50);
            this.totalFilesLabel.TabIndex = 0;
            this.totalFilesLabel.Text = "totalFilesLabel";
            this.totalFilesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // nextButton
            // 
            this.nextButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.nextButton.FlatAppearance.BorderSize = 0;
            this.nextButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.nextButton.Image = ((System.Drawing.Image)(resources.GetObject("nextButton.Image")));
            this.nextButton.Location = new System.Drawing.Point(534, 648);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(48, 48);
            this.nextButton.TabIndex = 0;
            this.nextButton.TabStop = false;
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // slideshowButton
            // 
            this.slideshowButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.slideshowButton.FlatAppearance.BorderSize = 0;
            this.slideshowButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.slideshowButton.Image = ((System.Drawing.Image)(resources.GetObject("slideshowButton.Image")));
            this.slideshowButton.Location = new System.Drawing.Point(480, 648);
            this.slideshowButton.Name = "slideshowButton";
            this.slideshowButton.Size = new System.Drawing.Size(48, 48);
            this.slideshowButton.TabIndex = 0;
            this.slideshowButton.TabStop = false;
            this.slideshowButton.UseVisualStyleBackColor = true;
            this.slideshowButton.Click += new System.EventHandler(this.SlideshowButton_Click);
            // 
            // previousButton
            // 
            this.previousButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.previousButton.FlatAppearance.BorderSize = 0;
            this.previousButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.previousButton.Image = ((System.Drawing.Image)(resources.GetObject("previousButton.Image")));
            this.previousButton.Location = new System.Drawing.Point(426, 648);
            this.previousButton.Name = "previousButton";
            this.previousButton.Size = new System.Drawing.Size(48, 48);
            this.previousButton.TabIndex = 0;
            this.previousButton.TabStop = false;
            this.previousButton.UseVisualStyleBackColor = true;
            this.previousButton.Click += new System.EventHandler(this.PreviousButton_Click);
            // 
            // deleteFileButton
            // 
            this.deleteFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteFileButton.FlatAppearance.BorderSize = 0;
            this.deleteFileButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteFileButton.Image = ((System.Drawing.Image)(resources.GetObject("deleteFileButton.Image")));
            this.deleteFileButton.Location = new System.Drawing.Point(894, 648);
            this.deleteFileButton.Name = "deleteFileButton";
            this.deleteFileButton.Size = new System.Drawing.Size(48, 48);
            this.deleteFileButton.TabIndex = 0;
            this.deleteFileButton.TabStop = false;
            this.deleteFileButton.UseVisualStyleBackColor = true;
            this.deleteFileButton.Click += new System.EventHandler(this.DeleteFileButton_Click);
            // 
            // fullscreenButton
            // 
            this.fullscreenButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.fullscreenButton.FlatAppearance.BorderSize = 0;
            this.fullscreenButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fullscreenButton.Image = ((System.Drawing.Image)(resources.GetObject("fullscreenButton.Image")));
            this.fullscreenButton.Location = new System.Drawing.Point(948, 648);
            this.fullscreenButton.Name = "fullscreenButton";
            this.fullscreenButton.Size = new System.Drawing.Size(48, 48);
            this.fullscreenButton.TabIndex = 0;
            this.fullscreenButton.TabStop = false;
            this.fullscreenButton.UseVisualStyleBackColor = true;
            this.fullscreenButton.Click += new System.EventHandler(this.FullscreenButton_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPanel.AutoScroll = true;
            this.mainPanel.Controls.Add(this.totalFilesLabel);
            this.mainPanel.Controls.Add(this.pictureBox);
            this.mainPanel.Location = new System.Drawing.Point(0, 30);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(1008, 610);
            this.mainPanel.TabIndex = 0;
            this.mainPanel.Resize += new System.EventHandler(this.MainPanel_Resize);
            // 
            // zoomButton
            // 
            this.zoomButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.zoomButton.FlatAppearance.BorderSize = 0;
            this.zoomButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.zoomButton.Image = ((System.Drawing.Image)(resources.GetObject("zoomButton.Image")));
            this.zoomButton.Location = new System.Drawing.Point(372, 648);
            this.zoomButton.Name = "zoomButton";
            this.zoomButton.Size = new System.Drawing.Size(48, 48);
            this.zoomButton.TabIndex = 0;
            this.zoomButton.TabStop = false;
            this.zoomButton.UseVisualStyleBackColor = true;
            this.zoomButton.Click += new System.EventHandler(this.ZoomButton_Click);
            // 
            // randomButton
            // 
            this.randomButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.randomButton.FlatAppearance.BorderSize = 0;
            this.randomButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.randomButton.Image = ((System.Drawing.Image)(resources.GetObject("randomButton.Image")));
            this.randomButton.Location = new System.Drawing.Point(588, 648);
            this.randomButton.Name = "randomButton";
            this.randomButton.Size = new System.Drawing.Size(48, 48);
            this.randomButton.TabIndex = 0;
            this.randomButton.TabStop = false;
            this.randomButton.UseVisualStyleBackColor = false;
            this.randomButton.Click += new System.EventHandler(this.RandomButton_Click);
            // 
            // settingsButton
            // 
            this.settingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.settingsButton.FlatAppearance.BorderSize = 0;
            this.settingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsButton.Image = ((System.Drawing.Image)(resources.GetObject("settingsButton.Image")));
            this.settingsButton.Location = new System.Drawing.Point(12, 648);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(48, 48);
            this.settingsButton.TabIndex = 0;
            this.settingsButton.TabStop = false;
            this.settingsButton.UseVisualStyleBackColor = true;
            this.settingsButton.Click += new System.EventHandler(this.SettingsButton_Click);
            // 
            // rotateImageButton
            // 
            this.rotateImageButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.rotateImageButton.FlatAppearance.BorderSize = 0;
            this.rotateImageButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rotateImageButton.Image = ((System.Drawing.Image)(resources.GetObject("rotateImageButton.Image")));
            this.rotateImageButton.Location = new System.Drawing.Point(318, 648);
            this.rotateImageButton.Name = "rotateImageButton";
            this.rotateImageButton.Size = new System.Drawing.Size(48, 48);
            this.rotateImageButton.TabIndex = 0;
            this.rotateImageButton.TabStop = false;
            this.rotateImageButton.UseVisualStyleBackColor = true;
            this.rotateImageButton.Click += new System.EventHandler(this.RotateImageButton_Click);
            // 
            // rightClickMenuStrip
            // 
            this.rightClickMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileNameMenuItem,
            this.toolStripSeparator4,
            this.copyPathMenuItem,
            this.copyFileMenuItem,
            this.setDesktopBackgroundMenuItem});
            this.rightClickMenuStrip.Name = "rightClickContextMenuStrip";
            this.rightClickMenuStrip.Size = new System.Drawing.Size(220, 98);
            // 
            // fileNameMenuItem
            // 
            this.fileNameMenuItem.Enabled = false;
            this.fileNameMenuItem.Name = "fileNameMenuItem";
            this.fileNameMenuItem.Size = new System.Drawing.Size(219, 22);
            this.fileNameMenuItem.Text = "fileNameMenuItem";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(216, 6);
            // 
            // copyPathMenuItem
            // 
            this.copyPathMenuItem.Name = "copyPathMenuItem";
            this.copyPathMenuItem.Size = new System.Drawing.Size(219, 22);
            this.copyPathMenuItem.Text = "Copy File Path";
            this.copyPathMenuItem.Click += new System.EventHandler(this.CopyPathMenuItem_Click);
            // 
            // copyFileMenuItem
            // 
            this.copyFileMenuItem.Name = "copyFileMenuItem";
            this.copyFileMenuItem.Size = new System.Drawing.Size(219, 22);
            this.copyFileMenuItem.Text = "Copy File To Clipboard";
            this.copyFileMenuItem.Click += new System.EventHandler(this.CopyFileMenuItem_Click);
            // 
            // setDesktopBackgroundMenuItem
            // 
            this.setDesktopBackgroundMenuItem.Name = "setDesktopBackgroundMenuItem";
            this.setDesktopBackgroundMenuItem.Size = new System.Drawing.Size(219, 22);
            this.setDesktopBackgroundMenuItem.Text = "Set As Desktop Background";
            this.setDesktopBackgroundMenuItem.Click += new System.EventHandler(this.SetDesktopBackgroundMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.rotateImageButton);
            this.Controls.Add(this.settingsButton);
            this.Controls.Add(this.randomButton);
            this.Controls.Add(this.zoomButton);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.fullscreenButton);
            this.Controls.Add(this.deleteFileButton);
            this.Controls.Add(this.previousButton);
            this.Controls.Add(this.slideshowButton);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(1024, 768);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Splash Image Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            this.rightClickMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel imageDimensionsLabel;
        private ToolStripStatusLabel programInfoLabel;
        private ToolStripStatusLabel memoryAllocatedLabel;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private Label totalFilesLabel;
        private ToolTip toolTip;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openImageMenuItem;
        private ToolStripMenuItem openFolderMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem closeImageMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitMenuItem;
        private Button nextButton;
        private Button slideshowButton;
        private Button previousButton;
        private Button deleteFileButton;
        private Button fullscreenButton;
        private Panel mainPanel;
        private Button zoomButton;
        private Button randomButton;
        private Button settingsButton;
        private Button rotateImageButton;
        private ToolStripMenuItem recentItemsMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ContextMenuStrip rightClickMenuStrip;
        private ToolStripMenuItem copyPathMenuItem;
        private ToolStripMenuItem setDesktopBackgroundMenuItem;
        private ToolStripMenuItem copyFileMenuItem;
        private ToolStripMenuItem fileNameMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripStatusLabel imageSizeLabel;
        private ToolStripStatusLabel imageTypeLabel;
    }
}
