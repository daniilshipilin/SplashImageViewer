using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Splash Image Viewer")]
[assembly: AssemblyDescription("A simple image viewer with slideshow feature.")]
[assembly: AssemblyProduct("Splash Image Viewer")]
[assembly: AssemblyCopyright("Daniil Shipilin (daniil.shipilin@gmail.com)")]
[assembly: AssemblyTrademark("Daniil Shipilin (daniil.shipilin@gmail.com)")]
[assembly: Guid("8e3acb01-f0a7-4434-946f-de5e21f4c247")]
[assembly: ComVisible(true)]

namespace SplashImageViewer
{
    public static class AssemblyInfo
    {
        static readonly Assembly _ass = Assembly.GetExecutingAssembly();
        static readonly AssemblyTitleAttribute _title = _ass.GetCustomAttributes<AssemblyTitleAttribute>().FirstOrDefault();
        static readonly AssemblyProductAttribute _product = _ass.GetCustomAttributes<AssemblyProductAttribute>().FirstOrDefault();
        static readonly AssemblyDescriptionAttribute _description = _ass.GetCustomAttributes<AssemblyDescriptionAttribute>().FirstOrDefault();
        static readonly AssemblyCopyrightAttribute _copyright = _ass.GetCustomAttributes<AssemblyCopyrightAttribute>().FirstOrDefault();
        static readonly AssemblyTrademarkAttribute _trademark = _ass.GetCustomAttributes<AssemblyTrademarkAttribute>().FirstOrDefault();
        static readonly GuidAttribute _guid = _ass.GetCustomAttributes<GuidAttribute>().FirstOrDefault();

        public static string BaseDirectory { get; } = AppDomain.CurrentDomain.BaseDirectory;
        public static string AppPath { get; } = _ass.Location;
        public static string AppTitle { get; } = _title.Title;
        public static string AppHeader { get; } = $"{_title.Title} v{GitVersionInformation.SemVer}";
        public static string AppAuthor { get; } = _copyright.Copyright;
        public static string AppDescription { get; } = _description.Description;
        public static string AppGUID { get; } = _guid.Value;
        public static string GitHubUrl { get; } = "https://github.com/daniilshipilin/SplashImageViewer";

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
