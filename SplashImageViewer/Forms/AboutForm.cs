namespace SplashImageViewer.Forms;

public partial class AboutForm : Form
{
    private readonly IUpdater? updater;

    public AboutForm(IUpdater? updater)
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
        this.Text = Resources.About;
        this.aboutLabel.Text = ApplicationInfo.AppInfoFormatted;

        this.updatesInfoLabel.Text = this.updater is null
            ? Resources.ApplicationUpdaterNotAvailable
            : !this.updater.CheckUpdateRequested
                ? $"{Resources.CheckForAvailableUpdates}. {Resources.LastCheck}: {AppSettings.UpdatesLastCheckedTimestamp}"
                : this.updater.ServerVersionIsGreater
                    ? $"{Resources.NewerProgramVersionAvailable}. {Resources.LastCheck}: {AppSettings.UpdatesLastCheckedTimestamp}"
                    : $"{Resources.ProgramIsUpToDate}. {Resources.LastCheck}: {AppSettings.UpdatesLastCheckedTimestamp}";

        this.toolTip.SetToolTip(this.checkUpdatesButton, Resources.CheckUpdatesButtonToolTip);
        this.toolTip.SetToolTip(this.updatesInfoLabel, Resources.ForceProgramUpdateToolTip);
        this.toolTip.SetToolTip(this.linkLabel, Resources.SourceCodeRepoToolTip);
    }

    private void ShowExceptionMessage(Exception ex)
    {
        this.updatesInfoLabel.Text = Resources.GeneralException;

        MessageBox.Show(
            new Form { TopMost = true },
            ex.Message,
            ex.GetType().ToString(),
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    private async void UpdatesInfoLabel_Click(object sender, EventArgs e)
    {
        if (this.updater is null)
        {
            return;
        }

        var dr = MessageBox.Show(
        new Form { TopMost = true },
        Resources.ForceProgramUpdatePrompt,
        Resources.ProgramUpdate,
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

        if (dr == DialogResult.Yes)
        {
            try
            {
                this.ShowUpdateForm();
                this.updatesInfoLabel.Text = Resources.UpdateInProgress;
                await this.updater.ForceUpdate();
                Utils.ProgramExit(ExitCode.Success);
            }
            catch (Exception ex)
            {
                this.ShowExceptionMessage(ex);
            }
        }
    }

    private async void CheckUpdatesButton_Click(object sender, EventArgs e)
    {
        if (this.updater is null)
        {
            return;
        }

        try
        {
            AppSettings.UpdateUpdatesLastCheckedTimestamp();
            this.updatesInfoLabel.Text = Resources.CheckingForUpdates;

            if (await this.updater.CheckUpdateIsAvailable())
            {
                this.updatesInfoLabel.Text = $"{Resources.NewerProgramVersionAvailable}. {Resources.LastCheck}: {AppSettings.UpdatesLastCheckedTimestamp}";

                var dr = MessageBox.Show(
                    new Form { TopMost = true },
                    $"{Resources.NewerProgramVersionAvailable}{Environment.NewLine}" +
                    $"{Resources.Current}: {this.updater.ClientVersion}{Environment.NewLine}" +
                    $"{Resources.Available}: {this.updater.ServerVersion}{Environment.NewLine}{Environment.NewLine}" +
                    $"{Resources.UpdateProgramPrompt}",
                    Resources.ProgramUpdate,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dr == DialogResult.Yes)
                {
                    this.ShowUpdateForm();
                    this.updatesInfoLabel.Text = Resources.UpdateInProgress;
                    await this.updater.Update();
                    Utils.ProgramExit(ExitCode.Success);
                }
            }
            else
            {
                this.updatesInfoLabel.Text = $"{Resources.ProgramIsUpToDate}. {Resources.LastCheck}: {AppSettings.UpdatesLastCheckedTimestamp}";
                var dr = MessageBox.Show(
                    new Form { TopMost = true },
                    Resources.ProgramIsUpToDate,
                    Resources.ProgramUpdate,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            this.ShowExceptionMessage(ex);
        }
    }

    private void ShowUpdateForm()
    {
        var updateForm = new Form
        {
            Text = Resources.ProgramUpdate,
            Width = 300,
            Height = 150,
            FormBorderStyle = FormBorderStyle.FixedSingle,
            StartPosition = FormStartPosition.CenterScreen,
            ShowInTaskbar = false,
            ShowIcon = false,
            MinimizeBox = false,
            MaximizeBox = false,
            TopMost = true,
        };

        var label = new Label
        {
            Size = new Size(300, 100),
            TextAlign = ContentAlignment.MiddleCenter,
            Text = $"{Resources.UpdateInProgress}. {ApplicationInfo.AppTitle} v{GitVersionInformation.SemVer} => v{this.updater?.ServerVersion}"
        };

        updateForm.Controls.Add(label);
        updateForm.Show();
    }

    private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(ApplicationInfo.GitHubUrl))
            {
                throw new ArgumentException(nameof(ApplicationInfo.GitHubUrl));
            }

            // use cmd to launch system default browser to navigate to a URL
            Utils.OpenLinkInBrowser(ApplicationInfo.GitHubUrl);

            // specify that the link was visited
            this.linkLabel.LinkVisited = true;
        }
        catch (Exception ex)
        {
            this.ShowExceptionMessage(ex);
        }
    }
}
