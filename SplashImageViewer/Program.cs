namespace SplashImageViewer
{
    using System;
    using System.Windows.Forms;
    using SplashImageViewer.Forms;
    using SplashImageViewer.Helpers;

    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // handle special case, when we pass single argument
            if (args.Length == 1 && args[0].Equals("/?"))
            {
                MessageBox.Show(
                    new Form { TopMost = true },
                    ApplicationInfo.AppInfoFormatted,
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Utils.ProgramExit(ExitCode.Success);
            }

            // check, if there is another instance running
            if (Utils.CheckAnotherInstanceIsRunning(ApplicationInfo.AppTitle))
            {
                MessageBox.Show(
                    new Form { TopMost = true },
                    $"Another instance of '{ApplicationInfo.AppTitle}' is running",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                Utils.ProgramExit(ExitCode.AnotherInstanceRunning);
            }

            ApplicationInfo.SetArgs(args);
            Application.Run(new MainForm());
            Utils.ProgramExit(ExitCode.Success);
        }
    }
}
