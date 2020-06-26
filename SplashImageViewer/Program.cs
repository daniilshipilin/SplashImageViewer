using SplashImageViewer.Forms;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SplashImageViewer
{
    static class Program
    {
        /// <summary>
        /// Cmd arguments.
        /// </summary>
        public static string[] CmdArgs { get; private set; }

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

        [STAThread]
        static void Main(string[] args)
        {
            // check, if there is another instance running
            if (CheckAnotherInstanceIsRunning(AssemblyInfo.AppTitle))
            {
                Console.WriteLine($"Another instance of '{AssemblyInfo.AppTitle}' is running");
                Environment.Exit((int)ExitCode.AnotherInstanceRunning);
            }

            // check if arguments provided
            if (args.Length > 0)
            {
                // handle special case, when we pass single argument
                if (args.Length == 1 && args[0] == "/?")
                {
                    Console.WriteLine(AssemblyInfo.AppInfoFormatted);
                    ProgramExit((int)ExitCode.Success);
                }

                CmdArgs = args;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            ProgramExit((int)ExitCode.Success);
        }

        private static bool CheckAnotherInstanceIsRunning(string programName)
        {
            return Process.GetProcessesByName(programName).Length > 1;
        }

        public static void ProgramExit(int exitCode)
        {
            Environment.Exit(exitCode);
        }

        public static void ExceptionHandler(Exception ex, string infoMessage = null, ExitCode exitCode = ExitCode.Error)
        {
            DialogResult dr;

            if (!string.IsNullOrEmpty(infoMessage))
            {
                dr = MessageBox.Show($"{infoMessage}{Environment.NewLine}{ex.Message}{Environment.NewLine}Exit program?", "Exception handler", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            }
            else
            {
                dr = MessageBox.Show($"{ex.Message}{Environment.NewLine}Exit program?", "Exception handler", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            }

            if (dr == DialogResult.Yes)
            {
                ProgramExit((int)exitCode);
            }
        }
    }
}
