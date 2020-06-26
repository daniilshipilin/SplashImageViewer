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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.aboutLabel = new System.Windows.Forms.Label();
            this.updatesInfoLabel = new System.Windows.Forms.Label();
            this.checkUpdatesButton = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.SuspendLayout();
            //
            // logoPictureBox
            //
            this.logoPictureBox.Image = global::SplashImageViewer.Properties.Resources.Splash_img;
            this.logoPictureBox.Location = new System.Drawing.Point(14, 14);
            this.logoPictureBox.Margin = new System.Windows.Forms.Padding(5);
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.Size = new System.Drawing.Size(100, 100);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoPictureBox.TabIndex = 0;
            this.logoPictureBox.TabStop = false;
            //
            // aboutLabel
            //
            this.aboutLabel.Location = new System.Drawing.Point(124, 14);
            this.aboutLabel.Margin = new System.Windows.Forms.Padding(5);
            this.aboutLabel.Name = "aboutLabel";
            this.aboutLabel.Size = new System.Drawing.Size(396, 195);
            this.aboutLabel.TabIndex = 0;
            //
            // updatesInfoLabel
            //
            this.updatesInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.updatesInfoLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.updatesInfoLabel.Location = new System.Drawing.Point(262, 219);
            this.updatesInfoLabel.Margin = new System.Windows.Forms.Padding(5);
            this.updatesInfoLabel.Name = "updatesInfoLabel";
            this.updatesInfoLabel.Size = new System.Drawing.Size(258, 30);
            this.updatesInfoLabel.TabIndex = 0;
            this.updatesInfoLabel.Text = "updatesInfoLabel";
            this.updatesInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.updatesInfoLabel.Click += new System.EventHandler(this.UpdatesInfoLabel_Click);
            //
            // checkUpdatesButton
            //
            this.checkUpdatesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkUpdatesButton.FlatAppearance.BorderSize = 0;
            this.checkUpdatesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkUpdatesButton.Image = ((System.Drawing.Image)(resources.GetObject("checkUpdatesButton.Image")));
            this.checkUpdatesButton.Location = new System.Drawing.Point(222, 217);
            this.checkUpdatesButton.Name = "checkUpdatesButton";
            this.checkUpdatesButton.Size = new System.Drawing.Size(32, 32);
            this.checkUpdatesButton.TabIndex = 2;
            this.checkUpdatesButton.TabStop = false;
            this.checkUpdatesButton.UseVisualStyleBackColor = true;
            this.checkUpdatesButton.Click += new System.EventHandler(this.CheckUpdatesButton_Click);
            //
            // AboutForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(534, 261);
            this.Controls.Add(this.checkUpdatesButton);
            this.Controls.Add(this.updatesInfoLabel);
            this.Controls.Add(this.aboutLabel);
            this.Controls.Add(this.logoPictureBox);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
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