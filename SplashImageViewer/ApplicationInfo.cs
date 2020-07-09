using System;
using System.Linq;
using System.Reflection;

namespace SplashImageViewer
{
    public static class ApplicationInfo
    {
        static readonly Assembly _ass = Assembly.GetExecutingAssembly();
        static readonly AssemblyTitleAttribute _title = _ass.GetCustomAttributes<AssemblyTitleAttribute>().FirstOrDefault();
        static readonly AssemblyDescriptionAttribute _description = _ass.GetCustomAttributes<AssemblyDescriptionAttribute>().FirstOrDefault();
        static readonly AssemblyCopyrightAttribute _copyright = _ass.GetCustomAttributes<AssemblyCopyrightAttribute>().FirstOrDefault();

        public static string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;
        public static string AppPath => _ass.Location;
        public static string AppTitle => _title.Title;
        public static string AppHeader => $"{_title.Title} v{GitVersionInformation.SemVer}";
        public static string AppAuthor => _copyright.Copyright;
        public static string AppDescription => _description.Description;
        public static Guid AppGUID => new Guid("8e3acb01-f0a7-4434-946f-de5e21f4c247");
        public static string GitHubUrl => "https://github.com/daniilshipilin/SplashImageViewer";

        /// <summary>
        /// Formatted application info string.
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
