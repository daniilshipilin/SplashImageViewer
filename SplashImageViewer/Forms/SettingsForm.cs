namespace SplashImageViewer.Forms
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;
    using SplashImageViewer.Helpers;
    using SplashImageViewer.Properties;

    public partial class SettingsForm : Form
    {
        // flag, that indicates whether SettingsForm_Load method has finished its
        // initialization, so that combo boxes SelectedIndexChanged event is
        // processed only afer form is loaded
        private bool formIsLoaded;

        public SettingsForm()
        {
            this.InitializeComponent();
        }

        public bool DefaultSettingsRestored { get; private set; }

        private static void ShowExceptionMessage(Exception ex)
        {
            MessageBox.Show(
                new Form { TopMost = true },
                ex.Message,
                ex.GetType().ToString(),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            this.LocalizeUIElements();

            this.uiLanguageComboBox.DataSource = AppSettings.CultureInfos;
            this.uiLanguageComboBox.SelectedItem = AppSettings.CurrentUICulture;
            this.slideshowTransitionSecComboBox.DataSource = AppSettings.SlideshowTransitionsSec;
            this.slideshowTransitionSecComboBox.SelectedItem = AppSettings.SlideshowTransitionSec;

            this.colorDialog.Color = Color.FromArgb(AppSettings.ThemeColorArgb);
            this.colorSelectLabel.BackColor = this.colorDialog.Color;
            this.colorLabel.Text = this.colorDialog.Color.Name.ToUpper();

            this.randomizeCheckBox.Checked = AppSettings.SlideshowOrderIsRandom;
            this.searchOptionCheckBox.Checked = AppSettings.SearchInSubdirs == SearchOption.AllDirectories;
            this.showFileDeletePromptCheckBox.Checked = AppSettings.ShowFileDeletePrompt;
            this.showFileOverwritePromptCheckBox.Checked = AppSettings.ShowFileOverwritePrompt;
            this.forceCheckUpdatesCheckBox.Checked = AppSettings.ForceCheckUpdates;

            this.SetChangesPending(false);
            this.formIsLoaded = true;
        }

        private void LocalizeUIElements()
        {
            this.Text = Resources.Settings;
            this.label1.Text = Resources.SlideshowTransition;
            this.randomizeCheckBox.Text = Resources.RandomizeSlideshowOrder;
            this.searchOptionCheckBox.Text = Resources.SearchImagesInSubdirectories;
            this.showFileDeletePromptCheckBox.Text = Resources.FileDeleteConfirmationRequired;
            this.defaultSettingsButton.Text = Resources.Reset;
            this.label2.Text = Resources.ThemeColor;
            this.forceCheckUpdatesCheckBox.Text = Resources.ForceCheckUpdates;
            this.showFileOverwritePromptCheckBox.Text = Resources.ModifiedFileOverwriteConfirmationRequired;
            this.label3.Text = Resources.UILanguage;

            this.toolTip.SetToolTip(this.okButton, Resources.SettingsCommitToolTip);
        }

        private void SlideshowTransitionMSComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.formIsLoaded)
            {
                this.SetChangesPending(true);
            }
        }

        private void UiLanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.formIsLoaded)
            {
                this.SetChangesPending(true);

                MessageBox.Show(
                        new Form { TopMost = true },
                        Resources.ChangingUILanguageRequiresAppRestart,
                        Resources.UILanguageChanged,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
            }
        }

        private void RandomizeCheckBox_CheckedChanged(object sender, EventArgs e) => this.SetChangesPending(true);

        private void SearcOptionCheckBox_CheckedChanged(object sender, EventArgs e) => this.SetChangesPending(true);

        private void ShowFileDeletePromptCheckBox_CheckedChanged(object sender, EventArgs e) => this.SetChangesPending(true);

        private void ShowFileOverwritePromptCheckBox_CheckedChanged(object sender, EventArgs e) => this.SetChangesPending(true);

        private void ForceCheckUpdatesCheckBox_CheckedChanged(object sender, EventArgs e) => this.SetChangesPending(true);

        private void SetChangesPending(bool pending) => this.okButton.Enabled = pending;

        private void SettingsForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            try
            {
                AppSettings.ThemeColorArgb = this.colorDialog.Color.ToArgb();
                AppSettings.SlideshowTransitionSec = (int)this.slideshowTransitionSecComboBox.SelectedItem;
                AppSettings.CurrentUICulture = (CultureInfo)this.uiLanguageComboBox.SelectedItem;
                AppSettings.SlideshowOrderIsRandom = this.randomizeCheckBox.Checked;
                AppSettings.SearchInSubdirs = this.searchOptionCheckBox.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                AppSettings.ShowFileDeletePrompt = this.showFileDeletePromptCheckBox.Checked;
                AppSettings.ShowFileOverwritePrompt = this.showFileOverwritePromptCheckBox.Checked;
                AppSettings.ForceCheckUpdates = this.forceCheckUpdatesCheckBox.Checked;
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }

            this.Close();
        }

        private void DefaultSettingsButton_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show(
                new Form { TopMost = true },
                Resources.ResetSettingsPrompt,
                Resources.DefaultSettings,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                try
                {
                    AppSettings.ResetSettings();
                }
                catch (Exception ex)
                {
                    ShowExceptionMessage(ex);
                }

                this.DefaultSettingsRestored = true;
                this.Close();
            }
        }

        private void ColorLabel_Click(object sender, EventArgs e)
        {
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.colorSelectLabel.BackColor = this.colorDialog.Color;
                this.colorLabel.Text = this.colorDialog.Color.Name.ToUpper();
                this.SetChangesPending(true);
            }
        }
    }
}
