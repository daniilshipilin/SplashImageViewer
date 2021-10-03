namespace SplashImageViewer.Helpers
{
    using System;
    using System.Diagnostics;

    public static class Utils
    {
        public static bool CheckAnotherInstanceIsRunning(string programName) => Process.GetProcessesByName(programName).Length > 1;

        public static void ProgramExit(ExitCode exitCode = ExitCode.Success) => Environment.Exit((int)exitCode);
    }
}
