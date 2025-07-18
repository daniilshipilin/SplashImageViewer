namespace SplashImageViewer.Forms
{
    using System.Drawing;
    using System.Windows.Forms;

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
            // AboutForm
            // 
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(584, 261);
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
        private System.Windows.Forms.ToolTip toolTip;
    }
}
