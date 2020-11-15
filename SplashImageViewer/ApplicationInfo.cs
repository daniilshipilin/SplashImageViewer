namespace SplashImageViewer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using SplashImageViewer.Properties;

    public static class ApplicationInfo
    {
        private static readonly Assembly Ass = Assembly.GetExecutingAssembly();
        private static readonly AssemblyTitleAttribute? Title = Ass.GetCustomAttributes<AssemblyTitleAttribute>().FirstOrDefault();
        private static readonly AssemblyProductAttribute? Product = Ass.GetCustomAttributes<AssemblyProductAttribute>().FirstOrDefault();
        private static readonly AssemblyDescriptionAttribute? Description = Ass.GetCustomAttributes<AssemblyDescriptionAttribute>().FirstOrDefault();
        private static readonly AssemblyCopyrightAttribute? Copyright = Ass.GetCustomAttributes<AssemblyCopyrightAttribute>().FirstOrDefault();

        public static IList<string> Args { get; private set; } = new List<string>();

        public static string BaseDirectory { get; } = Environment.CurrentDirectory;

        public static string ExePath { get; } = Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;

        public static string AppTitle { get; } = Title?.Title ?? string.Empty;

        public static string AppProduct { get; } = Product?.Product ?? string.Empty;

        public static string AppHeader { get; } = $"{Title?.Title} v{GitVersionInformation.SemVer}";

        public static string AppAuthor { get; } = Copyright?.Copyright ?? string.Empty;

        public static string AppDescription { get; } = Description?.Description ?? string.Empty;

        public static Guid AppGUID { get; } = new Guid("8e3acb01-f0a7-4434-946f-de5e21f4c247");

        public static string GitHubUrl { get; } = "https://github.com/daniilshipilin/SplashImageViewer";

        /// <summary>
        /// Gets application info formatted string.
        /// </summary>
        public static string AppInfoFormatted =>
            $"{AppHeader}{Environment.NewLine}" +
            $"{GitVersionInformation.InformationalVersion}{Environment.NewLine}" +
            $"{Strings.Author}: {AppAuthor}{Environment.NewLine}{Environment.NewLine}" +
            $"{Strings.Description}:{Environment.NewLine}" +
            $"  {Strings.AppDescription}";

        /// <summary>
        /// Sets application command line arguments.
        /// </summary>
        public static void SetArgs(string[] args)
        {
            Args = args.ToList();
        }
    }
}
