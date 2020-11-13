namespace SplashImageViewer.Forms
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using SplashImageViewer.Helpers;

    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            Text = $"[{AppSettings.RegistryBaseKeyFull}]";

            colorDialog.Color = AppSettings.ThemeColor;
            colorSelectLabel.BackColor = colorDialog.Color;
            colorLabel.Text = colorDialog.Color.ToArgb().ToString("X");

            slideshowTransitionSecComboBox.SelectedIndex = slideshowTransitionSecComboBox.FindStringExact((AppSettings.SlideshowTransitionMs / 1000).ToString());
            randomizeCheckBox.Checked = AppSettings.SlideshowOrderIsRandom;
            searchOptionCheckBox.Checked = AppSettings.SearchInSubdirs == SearchOption.AllDirectories;
            showFileDeletePromptCheckBox.Checked = AppSettings.ShowFileDeletePrompt;
            showFileOverwritePromptCheckBox.Checked = AppSettings.ShowFileOverwritePrompt;
            forceCheckUpdatesCheckBox.Checked = AppSettings.ForceCheckUpdates;

            toolTip.SetToolTip(okButton, "Commit changes");

            SetChangesPending(false);
        }

        private void SlideshowTransitionMSComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetChangesPending(true);
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
                AppSettings.ThemeColor = colorDialog.Color;
                AppSettings.SlideshowTransitionMs = int.Parse((string)slideshowTransitionSecComboBox.SelectedItem) * 1000;
                AppSettings.SlideshowOrderIsRandom = randomizeCheckBox.Checked;
                AppSettings.SearchInSubdirs = searchOptionCheckBox.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                AppSettings.ShowFileDeletePrompt = showFileDeletePromptCheckBox.Checked;
                AppSettings.ShowFileOverwritePrompt = showFileOverwritePromptCheckBox.Checked;
                AppSettings.ForceCheckUpdates = forceCheckUpdatesCheckBox.Checked;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    new Form { TopMost = true },
                    ex.Message,
                    ex.GetType().ToString(),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            Close();
        }

        private void DefaultSettingsButton_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show(
                new Form { TopMost = true },
                "Reset settings?",
                "Default settings",
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
                    MessageBox.Show(
                        new Form { TopMost = true },
                        ex.Message,
                        ex.GetType().ToString(),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                Close();
            }
        }

        private void ColorLabel_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() != DialogResult.Cancel)
            {
                colorSelectLabel.BackColor = colorDialog.Color;
                colorLabel.Text = colorDialog.Color.ToArgb().ToString("X");
                SetChangesPending(true);
            }
        }
    }
}
