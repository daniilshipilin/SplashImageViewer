namespace SplashImageViewer.Forms;

using System;
using System.Windows.Forms;
using SplashImageViewer.Properties;

public partial class AboutForm : Form
{
    public AboutForm()
    {
        this.InitializeComponent();
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
    }
}
