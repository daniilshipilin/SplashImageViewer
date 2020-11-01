namespace SplashImageViewer
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    public static class ApplicationInfo
    {
        private static readonly Assembly Ass = Assembly.GetExecutingAssembly();
        private static readonly AssemblyTitleAttribute Title = Ass.GetCustomAttributes<AssemblyTitleAttribute>().FirstOrDefault();
        private static readonly AssemblyDescriptionAttribute Description = Ass.GetCustomAttributes<AssemblyDescriptionAttribute>().FirstOrDefault();
        private static readonly AssemblyCopyrightAttribute Copyright = Ass.GetCustomAttributes<AssemblyCopyrightAttribute>().FirstOrDefault();

        public static string BaseDirectory { get; } = Environment.CurrentDirectory;

        public static string ExePath { get; } = Process.GetCurrentProcess().MainModule.FileName;

        public static string AppTitle { get; } = Title.Title;

        public static string AppHeader { get; } = $"{Title.Title} v{GitVersionInformation.SemVer}";

        public static string AppAuthor { get; } = Copyright.Copyright;

        public static string AppDescription { get; } = Description.Description;

        public static Guid AppGUID { get; } = new Guid("8e3acb01-f0a7-4434-946f-de5e21f4c247");

        public static string GitHubUrl { get; } = "https://github.com/daniilshipilin/SplashImageViewer";

        /// <summary>
        /// Gets application info formatted string.
        /// </summary>
        public static string AppInfoFormatted { get; } =
            $"{AppHeader}{Environment.NewLine}" +
            $"{GitVersionInformation.InformationalVersion}{Environment.NewLine}" +
            $"Author: {AppAuthor}{Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"Description:{Environment.NewLine}" +
            $"  {AppDescription}{Environment.NewLine}";
    }
}
