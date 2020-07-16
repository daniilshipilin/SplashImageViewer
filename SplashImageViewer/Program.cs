using SplashImageViewer.Forms;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SplashImageViewer
{
    /// <summary>
    /// Exitcode enum.
    /// </summary>
    public enum ExitCode
    {
        Success = 0,
        AnotherInstanceRunning = 1,
        IncorrectArgs = 2,
        Error = 4
    }

    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // check, if there is another instance running
            if (CheckAnotherInstanceIsRunning(ApplicationInfo.AppTitle))
            {
                MessageBox.Show($"Another instance of '{ApplicationInfo.AppTitle}' is running",
                                "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                ProgramExit(ExitCode.AnotherInstanceRunning);
            }

            // handle special case, when we pass single argument
            if (args.Length == 1 && args[0].Equals("/?"))
            {
                MessageBox.Show(ApplicationInfo.AppInfoFormatted, "Information",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Application.Run(new MainForm(args));
            }

            ProgramExit(ExitCode.Success);
        }

        private static bool CheckAnotherInstanceIsRunning(string programName)
        {
            return Process.GetProcessesByName(programName).Length > 1;
        }

        public static void ProgramExit(ExitCode exitCode = ExitCode.Success)
        {
            Environment.Exit((int)exitCode);
        }
    }
}
