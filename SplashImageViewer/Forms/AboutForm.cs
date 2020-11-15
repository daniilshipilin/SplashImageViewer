namespace SplashImageViewer.Forms
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;
    using SplashImageViewer.Helpers;
    using SplashImageViewer.Properties;

    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Close();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            LocalizeUIElements();
        }

        private void LocalizeUIElements()
        {
            Text = Strings.About;
            aboutLabel.Text = ApplicationInfo.AppInfoFormatted;

            if (ProgramUpdater.ServerVersion is null)
            {
                updatesInfoLabel.Text = Strings.CheckForAvailableUpdates;
            }
            else if (ProgramUpdater.ServerVersionIsGreater)
            {
                updatesInfoLabel.Text = Strings.NewerProgramVersionAvailable;
            }
            else
            {
                updatesInfoLabel.Text = Strings.ProgramIsUpToDate;
            }

            toolTip.SetToolTip(checkUpdatesButton, Strings.CheckUpdatesButtonToolTip);
            toolTip.SetToolTip(updatesInfoLabel, Strings.ForceProgramUpdateToolTip);
            toolTip.SetToolTip(linkLabel, Strings.SourceCodeRepoToolTip);
        }

        private void ShowExceptionMessage(Exception ex)
        {
            updatesInfoLabel.Text = Strings.GeneralException;

            MessageBox.Show(
                new Form { TopMost = true },
                ex.Message,
                ex.GetType().ToString(),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private async void UpdatesInfoLabel_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show(
                new Form { TopMost = true },
                Strings.ForceProgramUpdatePrompt,
                Strings.ProgramUpdate,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                try
                {
                    updatesInfoLabel.Text = Strings.UpdateInProgress;
                    await ProgramUpdater.ForceUpdate();
                    Program.ProgramExit(ExitCode.Success);
                }
                catch (Exception ex)
                {
                    ShowExceptionMessage(ex);
                }
            }
        }

        private async void CheckUpdatesButton_Click(object sender, EventArgs e)
        {
            try
            {
                AppSettings.UpdateUpdatesLastCheckedUtcTimestamp();
                updatesInfoLabel.Text = Strings.CheckingForUpdates;

                if (await ProgramUpdater.CheckUpdateIsAvailable())
                {
                    updatesInfoLabel.Text = Strings.NewerProgramVersionAvailable;

                    var dr = MessageBox.Show(
                        new Form { TopMost = true },
                        ProgramUpdater.UpdatePromptFormatted,
                        Strings.ProgramUpdate,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (dr == DialogResult.Yes)
                    {
                        updatesInfoLabel.Text = Strings.UpdateInProgress;
                        await ProgramUpdater.Update();
                        Program.ProgramExit(ExitCode.Success);
                    }
                }
                else
                {
                    updatesInfoLabel.Text = Strings.ProgramIsUpToDate;
                    var dr = MessageBox.Show(
                        new Form { TopMost = true },
                        Strings.ProgramIsUpToDate,
                        Strings.ProgramUpdate,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
        }

        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // specify that the link was visited
            linkLabel.LinkVisited = true;

            try
            {
                // navigate to a URL
                Process.Start(ApplicationInfo.GitHubUrl);
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
        }
    }
}
