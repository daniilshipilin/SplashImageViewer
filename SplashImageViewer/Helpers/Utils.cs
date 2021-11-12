namespace SplashImageViewer.Helpers;

public static class Utils
{
    public static bool CheckAnotherInstanceIsRunning(string programName) => Process.GetProcessesByName(programName).Length > 1;

    public static void ProgramExit(ExitCode exitCode = ExitCode.Success) => Environment.Exit((int)exitCode);

    public static long GetTotalAllocatedMemoryInBytes()
    {
        using var p = Process.GetCurrentProcess();

        return p.PrivateMemorySize64;
    }

    public static void OpenLinkInBrowser(string link)
    {
        LaunchCmdProcess(link);
    }

    public static void OpenExplorer(string path)
    {
        LaunchCmdProcess($"explorer.exe /select,{path}");
    }

    private static void LaunchCmdProcess(string args)
    {
        using var proc = new Process
        {
            StartInfo = new ProcessStartInfo("cmd.exe")
            {
                Arguments = $"/c start {args}",
                CreateNoWindow = true,
            }
        };

        proc.Start();
    }
}
