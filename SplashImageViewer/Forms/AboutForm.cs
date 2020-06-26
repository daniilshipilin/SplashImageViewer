using SplashImageViewer.Helpers;
using System;
using System.Windows.Forms;
using Updater;

namespace SplashImageViewer.Forms
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            aboutLabel.Text = AssemblyInfo.AppInfoFormatted;
            updatesInfoLabel.Text = "Need to check for available updates";

            toolTip.SetToolTip(checkUpdatesButton, "Check updates");
            toolTip.SetToolTip(updatesInfoLabel, "Press, to force program update");
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Close();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private async void UpdatesInfoLabel_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show($"Force program update?", "Program update",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                try
                {
                    var upd = new ProgramUpdater(Version.Parse(GitVersionInformation.SemVer),
                                                 AssemblyInfo.BaseDirectory,
                                                 AssemblyInfo.AppPath,
                                                 Guid.Parse(AssemblyInfo.AppGUID));

                    updatesInfoLabel.Text = "Update in progress";
                    await upd.ForceUpdate();
                    Program.ProgramExit((int)Program.ExitCode.Success);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().ToString(),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void CheckUpdatesButton_Click(object sender, EventArgs e)
        {
            try
            {
                AppSettings.UpdatesLastChecked = DateTime.Now;
                updatesInfoLabel.Text = "Checking updates";

                var upd = new ProgramUpdater(Version.Parse(GitVersionInformation.SemVer),
                                             AssemblyInfo.BaseDirectory,
                                             AssemblyInfo.AppPath,
                                             Guid.Parse(AssemblyInfo.AppGUID));

                if (await upd.CheckUpdateIsAvailable())
                {
                    updatesInfoLabel.Text = $"Newer program version available: v{upd.ProgramVerServer}";

                    var dr = MessageBox.Show($"Newer program version available.\n" +
                        $"Current: {GitVersionInformation.SemVer}\n" +
                        $"Available: {upd.ProgramVerServer}\n\n" +
                        $"Update program?", "Program update",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dr == DialogResult.Yes)
                    {
                        updatesInfoLabel.Text = "Update in progress";
                        await upd.Update();
                        Program.ProgramExit((int)Program.ExitCode.Success);
                    }
                }
                else
                {
                    updatesInfoLabel.Text = "Program is up to date";
                }
            }
            catch (Exception ex)
            {
                updatesInfoLabel.Text = "Exception encountered";
                MessageBox.Show(ex.Message, ex.GetType().ToString(),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
