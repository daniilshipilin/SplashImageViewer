namespace SplashImageViewer.Forms
{
    partial class FullscreenForm
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
            this.fullscreenPictureBox = new System.Windows.Forms.PictureBox();
            this.totalFilesLabel = new System.Windows.Forms.Label();
            this.infoLabel = new System.Windows.Forms.Label();
            this.filePathLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.fullscreenPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // fullscreenPictureBox
            // 
            this.fullscreenPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fullscreenPictureBox.Location = new System.Drawing.Point(0, 0);
            this.fullscreenPictureBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fullscreenPictureBox.Name = "fullscreenPictureBox";
            this.fullscreenPictureBox.Size = new System.Drawing.Size(400, 250);
            this.fullscreenPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.fullscreenPictureBox.TabIndex = 0;
            this.fullscreenPictureBox.TabStop = false;
            // 
            // totalFilesLabel
            // 
            this.totalFilesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.totalFilesLabel.BackColor = System.Drawing.Color.Transparent;
            this.totalFilesLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalFilesLabel.ForeColor = System.Drawing.Color.White;
            this.totalFilesLabel.Location = new System.Drawing.Point(250, 200);
            this.totalFilesLabel.Margin = new System.Windows.Forms.Padding(0);
            this.totalFilesLabel.Name = "totalFilesLabel";
            this.totalFilesLabel.Size = new System.Drawing.Size(150, 50);
            this.totalFilesLabel.TabIndex = 0;
            this.totalFilesLabel.Text = "totalFilesLabel";
            this.totalFilesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // infoLabel
            // 
            this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.infoLabel.BackColor = System.Drawing.Color.Transparent;
            this.infoLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoLabel.ForeColor = System.Drawing.Color.White;
            this.infoLabel.Location = new System.Drawing.Point(0, 0);
            this.infoLabel.Margin = new System.Windows.Forms.Padding(0);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(400, 50);
            this.infoLabel.TabIndex = 0;
            this.infoLabel.Text = "Press [F] or [ESC] to exit fullscreen mode";
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // filePathLabel
            // 
            this.filePathLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filePathLabel.BackColor = System.Drawing.Color.Transparent;
            this.filePathLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.filePathLabel.ForeColor = System.Drawing.Color.White;
            this.filePathLabel.Location = new System.Drawing.Point(0, 200);
            this.filePathLabel.Margin = new System.Windows.Forms.Padding(0);
            this.filePathLabel.Name = "filePathLabel";
            this.filePathLabel.Size = new System.Drawing.Size(400, 50);
            this.filePathLabel.TabIndex = 0;
            this.filePathLabel.Text = "filePathLabel";
            this.filePathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FullscreenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(400, 250);
            this.Controls.Add(this.totalFilesLabel);
            this.Controls.Add(this.filePathLabel);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.fullscreenPictureBox);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FullscreenForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FullscreenForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.fullscreenPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PictureBox fullscreenPictureBox;
        private System.Windows.Forms.Label totalFilesLabel;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Label filePathLabel;
    }
}
