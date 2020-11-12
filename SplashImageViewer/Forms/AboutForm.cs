namespace SplashImageViewer.Forms
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;
    using SplashImageViewer.Helpers;

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
            aboutLabel.Text = ApplicationInfo.AppInfoFormatted;

            updatesInfoLabel.Text = ProgramUpdater.ServerVersionIsGreater ?
                $"Newer program version available: v{ProgramUpdater.ServerVersion}" :
                "Program is up to date";

            toolTip.SetToolTip(checkUpdatesButton, "Check updates");
            toolTip.SetToolTip(updatesInfoLabel, "Press, to force program update");
            toolTip.SetToolTip(linkLabel, "Visit project source code repository on github.com");
        }

        private void ShowExceptionMessage(Exception ex)
        {
            updatesInfoLabel.Text = "Exception encountered";
            MessageBox.Show(
                ex.Message,
                ex.GetType().ToString(),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private async void UpdatesInfoLabel_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show(
                $"Force program update?",
                "Program update",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                try
                {
                    updatesInfoLabel.Text = "Update in progress";
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
                updatesInfoLabel.Text = "Checking updates";

                if (await ProgramUpdater.CheckUpdateIsAvailable())
                {
                    updatesInfoLabel.Text = $"Newer program version available: v{ProgramUpdater.ServerVersion}";

                    var dr = MessageBox.Show(
                        $"Newer program version available.\n" +
                        $"Current: {GitVersionInformation.SemVer}\n" +
                        $"Available: {ProgramUpdater.ServerVersion}\n\n" +
                        $"Update program?",
                        "Program update",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (dr == DialogResult.Yes)
                    {
                        updatesInfoLabel.Text = "Update in progress";
                        await ProgramUpdater.Update();
                        Program.ProgramExit(ExitCode.Success);
                    }
                }
                else
                {
                    updatesInfoLabel.Text = "Program is up to date";
                    var dr = MessageBox.Show(
                        "Program is up to date",
                        "Program update",
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
