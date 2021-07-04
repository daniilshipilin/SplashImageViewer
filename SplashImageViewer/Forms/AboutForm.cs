namespace SplashImageViewer.Forms
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;
    using ProgramUpdater;
    using SplashImageViewer.Helpers;
    using SplashImageViewer.Properties;

    public partial class AboutForm : Form
    {
        private readonly Updater updater;

        public AboutForm(Updater updater)
        {
            this.InitializeComponent();
            this.updater = updater;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void AboutForm_Load(object sender, EventArgs e) => this.LocalizeUIElements();

        private void LocalizeUIElements()
        {
            this.Text = Strings.About;
            this.aboutLabel.Text = ApplicationInfo.AppInfoFormatted;

            this.updatesInfoLabel.Text = !this.updater.CheckUpdateRequested
                ? $"{Strings.CheckForAvailableUpdates}. {Strings.LastCheck}: {AppSettings.UpdatesLastCheckedTimestamp}"
                : this.updater.ServerVersionIsGreater
                    ? $"{Strings.NewerProgramVersionAvailable}. {Strings.LastCheck}: {AppSettings.UpdatesLastCheckedTimestamp}"
                    : $"{Strings.ProgramIsUpToDate}. {Strings.LastCheck}: {AppSettings.UpdatesLastCheckedTimestamp}";

            this.toolTip.SetToolTip(this.checkUpdatesButton, Strings.CheckUpdatesButtonToolTip);
            this.toolTip.SetToolTip(this.updatesInfoLabel, Strings.ForceProgramUpdateToolTip);
            this.toolTip.SetToolTip(this.linkLabel, Strings.SourceCodeRepoToolTip);
        }

        private void ShowExceptionMessage(Exception ex)
        {
            this.updatesInfoLabel.Text = Strings.GeneralException;

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
                    this.updatesInfoLabel.Text = Strings.UpdateInProgress;
                    await this.updater.ForceUpdate();
                    Program.ProgramExit(ExitCode.Success);
                }
                catch (Exception ex)
                {
                    this.ShowExceptionMessage(ex);
                }
            }
        }

        private async void CheckUpdatesButton_Click(object sender, EventArgs e)
        {
            try
            {
                AppSettings.UpdateUpdatesLastCheckedTimestamp();
                this.updatesInfoLabel.Text = Strings.CheckingForUpdates;

                if (await this.updater.CheckUpdateIsAvailable())
                {
                    this.updatesInfoLabel.Text = $"{Strings.NewerProgramVersionAvailable}. {Strings.LastCheck}: {AppSettings.UpdatesLastCheckedTimestamp}";

                    var dr = MessageBox.Show(
                        new Form { TopMost = true },
                        this.updater.UpdatePromptFormatted,
                        Strings.ProgramUpdate,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (dr == DialogResult.Yes)
                    {
                        this.updatesInfoLabel.Text = Strings.UpdateInProgress;
                        await this.updater.Update();
                        Program.ProgramExit(ExitCode.Success);
                    }
                }
                else
                {
                    this.updatesInfoLabel.Text = $"{Strings.ProgramIsUpToDate}. {Strings.LastCheck}: {AppSettings.UpdatesLastCheckedTimestamp}";
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
                this.ShowExceptionMessage(ex);
            }
        }

        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // specify that the link was visited
            this.linkLabel.LinkVisited = true;

            try
            {
                // navigate to a URL
                Process.Start(ApplicationInfo.GitHubUrl);
            }
            catch (Exception ex)
            {
                this.ShowExceptionMessage(ex);
            }
        }
    }
}
