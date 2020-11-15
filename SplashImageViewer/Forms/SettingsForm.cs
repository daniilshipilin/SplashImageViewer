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
            InitializeComponent();
        }

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
            LocalizeUIElements();

            uiLanguageComboBox.DataSource = AppSettings.CultureInfos;
            uiLanguageComboBox.SelectedItem = AppSettings.CurrentUICulture;
            slideshowTransitionSecComboBox.DataSource = AppSettings.SlideshowTransitionsSec;
            slideshowTransitionSecComboBox.SelectedItem = AppSettings.SlideshowTransitionSec;

            Text = AppSettings.RegistryBaseKeyFull;

            colorDialog.Color = Color.FromArgb(AppSettings.ThemeColorArgb);
            colorSelectLabel.BackColor = colorDialog.Color;
            colorLabel.Text = colorDialog.Color.Name.ToUpper();

            randomizeCheckBox.Checked = AppSettings.SlideshowOrderIsRandom;
            searchOptionCheckBox.Checked = AppSettings.SearchInSubdirs == SearchOption.AllDirectories;
            showFileDeletePromptCheckBox.Checked = AppSettings.ShowFileDeletePrompt;
            showFileOverwritePromptCheckBox.Checked = AppSettings.ShowFileOverwritePrompt;
            forceCheckUpdatesCheckBox.Checked = AppSettings.ForceCheckUpdates;

            SetChangesPending(false);
            formIsLoaded = true;
        }

        private void LocalizeUIElements()
        {
            Text = Strings.Settings;
            label1.Text = Strings.SlideshowTransition;
            randomizeCheckBox.Text = Strings.RandomizeSlideshowOrder;
            searchOptionCheckBox.Text = Strings.SearchImagesInSubdirectories;
            showFileDeletePromptCheckBox.Text = Strings.FileDeleteConfirmationRequired;
            defaultSettingsButton.Text = Strings.Reset;
            label2.Text = Strings.ThemeColor;
            forceCheckUpdatesCheckBox.Text = Strings.ForceCheckUpdates;
            showFileOverwritePromptCheckBox.Text = Strings.ModifiedFileOverwriteConfirmationRequired;
            label3.Text = Strings.UILanguage;

            toolTip.SetToolTip(okButton, Strings.SettingsCommitToolTip);
        }

        private void SlideshowTransitionMSComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (formIsLoaded)
            {
                SetChangesPending(true);
            }
        }

        private void UiLanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (formIsLoaded)
            {
                SetChangesPending(true);

                MessageBox.Show(
                        new Form { TopMost = true },
                        Strings.ChangingUILanguageRequiresAppRestart,
                        Strings.UILanguageChanged,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
            }
        }

        private void RandomizeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetChangesPending(true);
        }

        private void SearcOptionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetChangesPending(true);
        }

        private void ShowFileDeletePromptCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetChangesPending(true);
        }

        private void ShowFileOverwritePromptCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetChangesPending(true);
        }

        private void ForceCheckUpdatesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetChangesPending(true);
        }

        private void SetChangesPending(bool pending)
        {
            okButton.Enabled = pending;
        }

        private void SettingsForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            try
            {
                AppSettings.ThemeColorArgb = colorDialog.Color.ToArgb();
                AppSettings.SlideshowTransitionSec = (int)slideshowTransitionSecComboBox.SelectedItem;
                AppSettings.CurrentUICulture = (CultureInfo)uiLanguageComboBox.SelectedItem;
                AppSettings.SlideshowOrderIsRandom = randomizeCheckBox.Checked;
                AppSettings.SearchInSubdirs = searchOptionCheckBox.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                AppSettings.ShowFileDeletePrompt = showFileDeletePromptCheckBox.Checked;
                AppSettings.ShowFileOverwritePrompt = showFileOverwritePromptCheckBox.Checked;
                AppSettings.ForceCheckUpdates = forceCheckUpdatesCheckBox.Checked;
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }

            Close();
        }

        private void DefaultSettingsButton_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show(
                new Form { TopMost = true },
                Strings.ResetSettingsPrompt,
                Strings.DefaultSettings,
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

                Close();
            }
        }

        private void ColorLabel_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                colorSelectLabel.BackColor = colorDialog.Color;
                colorLabel.Text = colorDialog.Color.Name.ToUpper();
                SetChangesPending(true);
            }
        }
    }
}
