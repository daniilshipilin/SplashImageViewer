namespace SplashImageViewer.Forms
{
    partial class AboutForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.logoPictureBox = new PictureBox();
            this.aboutLabel = new Label();
            this.updatesInfoLabel = new Label();
            this.checkUpdatesButton = new Button();
            this.toolTip = new ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)this.logoPictureBox).BeginInit();
            this.SuspendLayout();
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Image = (Image)resources.GetObject("logoPictureBox.Image");
            this.logoPictureBox.Location = new Point(14, 14);
            this.logoPictureBox.Margin = new Padding(5);
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.Size = new Size(100, 100);
            this.logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.logoPictureBox.TabIndex = 0;
            this.logoPictureBox.TabStop = false;
            // 
            // aboutLabel
            // 
            this.aboutLabel.Location = new Point(124, 14);
            this.aboutLabel.Margin = new Padding(5);
            this.aboutLabel.Name = "aboutLabel";
            this.aboutLabel.Size = new Size(446, 155);
            this.aboutLabel.TabIndex = 0;
            this.aboutLabel.Text = "aboutLabel";
            // 
            // updatesInfoLabel
            // 
            this.updatesInfoLabel.Font = new Font("Segoe UI", 9.75F, FontStyle.Italic);
            this.updatesInfoLabel.Location = new Point(14, 217);
            this.updatesInfoLabel.Margin = new Padding(5);
            this.updatesInfoLabel.Name = "updatesInfoLabel";
            this.updatesInfoLabel.Size = new Size(556, 30);
            this.updatesInfoLabel.TabIndex = 0;
            this.updatesInfoLabel.Text = "updatesInfoLabel";
            this.updatesInfoLabel.TextAlign = ContentAlignment.MiddleRight;
            this.updatesInfoLabel.Click += this.UpdatesInfoLabel_Click;
            // 
            // checkUpdatesButton
            // 
            this.checkUpdatesButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.checkUpdatesButton.FlatAppearance.BorderSize = 0;
            this.checkUpdatesButton.FlatStyle = FlatStyle.Flat;
            this.checkUpdatesButton.Image = (Image)resources.GetObject("checkUpdatesButton.Image");
            this.checkUpdatesButton.Location = new Point(276, 177);
            this.checkUpdatesButton.Name = "checkUpdatesButton";
            this.checkUpdatesButton.Size = new Size(32, 32);
            this.checkUpdatesButton.TabIndex = 2;
            this.checkUpdatesButton.TabStop = false;
            this.checkUpdatesButton.UseVisualStyleBackColor = true;
            this.checkUpdatesButton.Click += this.CheckUpdatesButton_Click;
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(584, 261);
            this.Controls.Add(this.checkUpdatesButton);
            this.Controls.Add(this.updatesInfoLabel);
            this.Controls.Add(this.aboutLabel);
            this.Controls.Add(this.logoPictureBox);
            this.Font = new Font("Segoe UI", 9.75F);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Margin = new Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.ShowIcon = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "About";
            this.Load += this.AboutForm_Load;
            ((System.ComponentModel.ISupportInitialize)this.logoPictureBox).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Label aboutLabel;
        private System.Windows.Forms.Label updatesInfoLabel;
        private System.Windows.Forms.Button checkUpdatesButton;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
