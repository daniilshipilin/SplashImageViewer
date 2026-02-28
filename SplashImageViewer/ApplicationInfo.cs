namespace SplashImageViewer;

using System;
using System.Linq;
using System.Reflection;
using SplashImageViewer.Properties;

public static class ApplicationInfo
{
    private static readonly Assembly Ass = Assembly.GetExecutingAssembly();
    private static readonly AssemblyTitleAttribute? Title = Ass.GetCustomAttributes<AssemblyTitleAttribute>().FirstOrDefault();
    private static readonly AssemblyProductAttribute? Product = Ass.GetCustomAttributes<AssemblyProductAttribute>().FirstOrDefault();

    public const string AppBuild =
#if DEBUG
            " [Debug]";
#else
            "";
#endif

    public static string AppTitle { get; } = Title?.Title ?? string.Empty;

    public static string AppProduct { get; } = Product?.Product ?? string.Empty;

    public static string AppHeader { get; } = $"{AppTitle} v{Ass.GetName().Version}{AppBuild}";

    public const string GlobalMutexName = "Global\\Splash_6EC07BB4D546FF848DAE296A150DFCB0F968EAE2";

    public static string AppInfoFormatted =>
        $"{AppHeader}{Environment.NewLine}{Environment.NewLine}" +
        $"{Resources.Description}:{Environment.NewLine}" +
        $"  {Resources.AppDescription}";
}
