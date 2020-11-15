namespace SplashImageViewer.Forms
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.label1 = new System.Windows.Forms.Label();
            this.slideshowTransitionSecComboBox = new System.Windows.Forms.ComboBox();
            this.randomizeCheckBox = new System.Windows.Forms.CheckBox();
            this.searchOptionCheckBox = new System.Windows.Forms.CheckBox();
            this.showFileDeletePromptCheckBox = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.defaultSettingsButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.forceCheckUpdatesCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.colorSelectLabel = new System.Windows.Forms.Label();
            this.colorLabel = new System.Windows.Forms.Label();
            this.showFileOverwritePromptCheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.uiLanguageComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 46);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Slideshow transition (s):";
            // 
            // slideshowTransitionSecComboBox
            // 
            this.slideshowTransitionSecComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.slideshowTransitionSecComboBox.FormattingEnabled = true;
            this.slideshowTransitionSecComboBox.Items.AddRange(new object[] {
            "1",
            "2",
            "5",
            "10",
            "30",
            "60",
            "300",
            "600",
            "3600"});
            this.slideshowTransitionSecComboBox.Location = new System.Drawing.Point(216, 43);
            this.slideshowTransitionSecComboBox.Name = "slideshowTransitionSecComboBox";
            this.slideshowTransitionSecComboBox.Size = new System.Drawing.Size(60, 25);
            this.slideshowTransitionSecComboBox.TabIndex = 2;
            this.slideshowTransitionSecComboBox.SelectedIndexChanged += new System.EventHandler(this.SlideshowTransitionMSComboBox_SelectedIndexChanged);
            // 
            // randomizeCheckBox
            // 
            this.randomizeCheckBox.AutoSize = true;
            this.randomizeCheckBox.Location = new System.Drawing.Point(12, 74);
            this.randomizeCheckBox.Name = "randomizeCheckBox";
            this.randomizeCheckBox.Size = new System.Drawing.Size(190, 21);
            this.randomizeCheckBox.TabIndex = 3;
            this.randomizeCheckBox.Text = "Randomize slideshow order";
            this.randomizeCheckBox.UseVisualStyleBackColor = true;
            this.randomizeCheckBox.CheckedChanged += new System.EventHandler(this.RandomizeCheckBox_CheckedChanged);
            // 
            // searchOptionCheckBox
            // 
            this.searchOptionCheckBox.AutoSize = true;
            this.searchOptionCheckBox.Location = new System.Drawing.Point(12, 101);
            this.searchOptionCheckBox.Name = "searchOptionCheckBox";
            this.searchOptionCheckBox.Size = new System.Drawing.Size(213, 21);
            this.searchOptionCheckBox.TabIndex = 4;
            this.searchOptionCheckBox.Text = "Search images in subdirectories";
            this.searchOptionCheckBox.UseVisualStyleBackColor = true;
            this.searchOptionCheckBox.CheckedChanged += new System.EventHandler(this.SearcOptionCheckBox_CheckedChanged);
            // 
            // showFileDeletePromptCheckBox
            // 
            this.showFileDeletePromptCheckBox.AutoSize = true;
            this.showFileDeletePromptCheckBox.Location = new System.Drawing.Point(12, 128);
            this.showFileDeletePromptCheckBox.Name = "showFileDeletePromptCheckBox";
            this.showFileDeletePromptCheckBox.Size = new System.Drawing.Size(217, 21);
            this.showFileDeletePromptCheckBox.TabIndex = 6;
            this.showFileDeletePromptCheckBox.Text = "File delete confirmation required";
            this.showFileDeletePromptCheckBox.UseVisualStyleBackColor = true;
            this.showFileDeletePromptCheckBox.CheckedChanged += new System.EventHandler(this.ShowFileDeletePromptCheckBox_CheckedChanged);
            // 
            // defaultSettingsButton
            // 
            this.defaultSettingsButton.Location = new System.Drawing.Point(322, 209);
            this.defaultSettingsButton.Name = "defaultSettingsButton";
            this.defaultSettingsButton.Size = new System.Drawing.Size(100, 30);
            this.defaultSettingsButton.TabIndex = 7;
            this.defaultSettingsButton.Text = "Reset";
            this.defaultSettingsButton.UseVisualStyleBackColor = true;
            this.defaultSettingsButton.Click += new System.EventHandler(this.DefaultSettingsButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 15);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Theme color:";
            // 
            // forceCheckUpdatesCheckBox
            // 
            this.forceCheckUpdatesCheckBox.AutoSize = true;
            this.forceCheckUpdatesCheckBox.Location = new System.Drawing.Point(12, 182);
            this.forceCheckUpdatesCheckBox.Name = "forceCheckUpdatesCheckBox";
            this.forceCheckUpdatesCheckBox.Size = new System.Drawing.Size(146, 21);
            this.forceCheckUpdatesCheckBox.TabIndex = 11;
            this.forceCheckUpdatesCheckBox.Text = "Force check updates";
            this.forceCheckUpdatesCheckBox.UseVisualStyleBackColor = true;
            this.forceCheckUpdatesCheckBox.CheckedChanged += new System.EventHandler(this.ForceCheckUpdatesCheckBox_CheckedChanged);
            // 
            // okButton
            // 
            this.okButton.FlatAppearance.BorderSize = 0;
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.okButton.Image = ((System.Drawing.Image)(resources.GetObject("okButton.Image")));
            this.okButton.Location = new System.Drawing.Point(206, 207);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(32, 32);
            this.okButton.TabIndex = 0;
            this.okButton.TabStop = false;
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // colorSelectLabel
            // 
            this.colorSelectLabel.BackColor = System.Drawing.Color.Black;
            this.colorSelectLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorSelectLabel.Location = new System.Drawing.Point(216, 12);
            this.colorSelectLabel.Margin = new System.Windows.Forms.Padding(3);
            this.colorSelectLabel.Name = "colorSelectLabel";
            this.colorSelectLabel.Size = new System.Drawing.Size(60, 25);
            this.colorSelectLabel.TabIndex = 0;
            this.colorSelectLabel.Click += new System.EventHandler(this.ColorLabel_Click);
            // 
            // colorLabel
            // 
            this.colorLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.colorLabel.Location = new System.Drawing.Point(122, 15);
            this.colorLabel.Margin = new System.Windows.Forms.Padding(3);
            this.colorLabel.Name = "colorLabel";
            this.colorLabel.Size = new System.Drawing.Size(80, 17);
            this.colorLabel.TabIndex = 12;
            this.colorLabel.Text = "colorLabel";
            this.colorLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // showFileOverwritePromptCheckBox
            // 
            this.showFileOverwritePromptCheckBox.AutoSize = true;
            this.showFileOverwritePromptCheckBox.Location = new System.Drawing.Point(12, 155);
            this.showFileOverwritePromptCheckBox.Name = "showFileOverwritePromptCheckBox";
            this.showFileOverwritePromptCheckBox.Size = new System.Drawing.Size(290, 21);
            this.showFileOverwritePromptCheckBox.TabIndex = 13;
            this.showFileOverwritePromptCheckBox.Text = "Modified file overwrite confirmation required";
            this.showFileOverwritePromptCheckBox.UseVisualStyleBackColor = true;
            this.showFileOverwritePromptCheckBox.CheckedChanged += new System.EventHandler(this.ShowFileOverwritePromptCheckBox_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(298, 15);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 17);
            this.label3.TabIndex = 14;
            this.label3.Text = "UI language:";
            // 
            // uiLanguageComboBox
            // 
            this.uiLanguageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uiLanguageComboBox.FormattingEnabled = true;
            this.uiLanguageComboBox.Location = new System.Drawing.Point(322, 43);
            this.uiLanguageComboBox.Name = "uiLanguageComboBox";
            this.uiLanguageComboBox.Size = new System.Drawing.Size(60, 25);
            this.uiLanguageComboBox.TabIndex = 15;
            this.uiLanguageComboBox.SelectedIndexChanged += new System.EventHandler(this.UiLanguageComboBox_SelectedIndexChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(434, 251);
            this.Controls.Add(this.uiLanguageComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.showFileOverwritePromptCheckBox);
            this.Controls.Add(this.colorLabel);
            this.Controls.Add(this.colorSelectLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.forceCheckUpdatesCheckBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.defaultSettingsButton);
            this.Controls.Add(this.showFileDeletePromptCheckBox);
            this.Controls.Add(this.searchOptionCheckBox);
            this.Controls.Add(this.randomizeCheckBox);
            this.Controls.Add(this.slideshowTransitionSecComboBox);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SettingsForm_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox slideshowTransitionSecComboBox;
        private System.Windows.Forms.CheckBox randomizeCheckBox;
        private System.Windows.Forms.CheckBox searchOptionCheckBox;
        private System.Windows.Forms.CheckBox showFileDeletePromptCheckBox;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button defaultSettingsButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox forceCheckUpdatesCheckBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Label colorSelectLabel;
        private System.Windows.Forms.Label colorLabel;
        private System.Windows.Forms.CheckBox showFileOverwritePromptCheckBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox uiLanguageComboBox;
    }
}
