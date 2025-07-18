namespace SplashImageViewer;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using SplashImageViewer.Properties;

public static class ApplicationInfo
{
    private static readonly Assembly Ass = Assembly.GetExecutingAssembly();
    private static readonly AssemblyTitleAttribute? Title = Ass.GetCustomAttributes<AssemblyTitleAttribute>().FirstOrDefault();
    private static readonly AssemblyProductAttribute? Product = Ass.GetCustomAttributes<AssemblyProductAttribute>().FirstOrDefault();
    private static readonly AssemblyDescriptionAttribute? Description = Ass.GetCustomAttributes<AssemblyDescriptionAttribute>().FirstOrDefault();

    public const string AppBuild =
#if DEBUG
            " [Debug]";
#else
            "";
#endif

    public static IReadOnlyList<string> Args { get; private set; } = new List<string>();

    public static string BaseDirectory => Path.GetDirectoryName(ExePath) ?? string.Empty;

    public static string ExePath { get; } = Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;

    public static string AppTitle { get; } = Title?.Title ?? string.Empty;

    public static string AppProduct { get; } = Product?.Product ?? string.Empty;

    public static string AppHeader { get; } = $"{AppTitle} v{Ass.GetName().Version}{AppBuild}";

    public static string AppDescription { get; } = Description?.Description ?? string.Empty;

    public static Guid AppGUID { get; } = new Guid("8e3acb01-f0a7-4434-946f-de5e21f4c247");

    public const string GlobalMutexName = "Global\\Splash_6EC07BB4D546FF848DAE296A150DFCB0F968EAE2";

    /// <summary>
    /// Gets application info formatted string.
    /// </summary>
    public static string AppInfoFormatted =>
        $"{AppHeader}{Environment.NewLine}{Environment.NewLine}" +
        $"{Resources.Description}:{Environment.NewLine}" +
        $"  {Resources.AppDescription}";

    /// <summary>
    /// Sets application command line arguments.
    /// </summary>
    public static void SetArgs(string[] args) => Args = args.ToList();
}
