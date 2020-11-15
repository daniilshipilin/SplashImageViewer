namespace SplashImageViewer.Helpers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using SplashImageViewer.Models;
    using SplashImageViewer.Properties;

    public static class ProgramUpdater
    {
        private static readonly HttpClient Client = new HttpClient();
        private static readonly string TmpDir = Path.Combine(ApplicationInfo.BaseDirectory, "update");
        private static readonly string ArchivesDir = Path.Combine(TmpDir, "Archives");
        private static readonly string ExtractedDir = Path.Combine(TmpDir, "Extracted");

        private static AppVersionsXml.Record? appRecord;

        /// <summary>
        /// Gets current program version.
        /// </summary>
        public static Version ClientVersion { get; } = Version.Parse(GitVersionInformation.SemVer);

        /// <summary>
        /// Gets latest program version, that resides on the server.
        /// </summary>
        public static Version? ServerVersion { get; private set; }

        /// <summary>
        /// Gets a value indicating whether server program version is greater, than the current program version.
        /// </summary>
        public static bool ServerVersionIsGreater { get; private set; } = false;

        /// <summary>
        /// Gets a pre-formatted update prompt string.
        /// </summary>
        public static string UpdatePromptFormatted =>
            $"{Strings.NewerProgramVersionAvailable}.{Environment.NewLine}" +
            $"{Strings.Current}: {ClientVersion}{Environment.NewLine}" +
            $"{Strings.Available}: {ServerVersion}{Environment.NewLine}{Environment.NewLine}" +
            $"{Strings.UpdateProgramPrompt}";

        /// <summary>
        /// Checks, whether update is available.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task<bool> CheckUpdateIsAvailable()
        {
            if (string.IsNullOrEmpty(AppSettings.AppVersionsXmlUrl))
            {
                throw new ArgumentNullException(nameof(AppSettings.AppVersionsXmlUrl));
            }

            using var response = await Client.GetAsync(AppSettings.AppVersionsXmlUrl);

            if (response.IsSuccessStatusCode)
            {
                string? fileName = response.Content.Headers.ContentDisposition?.FileName?.Replace("\"", string.Empty);
                using var sr = new StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.UTF8);
                string xml = sr.ReadToEnd();

                // deserialize received xml
                var xmlObj = XmlUtils.XmlDeserializeFromString<AppVersionsXml>(xml);

                // find specific record, based on GUID
                appRecord = xmlObj?.AppRecords?.FirstOrDefault(x => x.GUID.Equals(ApplicationInfo.AppGUID));

                if (appRecord is null)
                {
                    // guid was not found - throw exception
                    throw new Exception($"'{ApplicationInfo.AppGUID}' {Strings.GuidWasNotFound} '{fileName}'");
                }

                ServerVersion = Version.Parse(appRecord.SemVer);
                ServerVersionIsGreater = ServerVersion.CompareTo(ClientVersion) > 0;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }

            return ServerVersion.CompareTo(ClientVersion) > 0;
        }

        /// <summary>
        /// Initiates program update without comparing server and client versions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task ForceUpdate()
        {
            await CheckUpdateIsAvailable();
            await Update();
        }

        /// <summary>
        /// Launches program update procedure.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Update()
        {
            if (ServerVersion is object && appRecord is object)
            {
                CleanTmpDirectory();
                CreateDirectoryStructure();
                string filePath;

                using var response = await Client.GetAsync(appRecord.DownloadUrl);

                if (response.IsSuccessStatusCode)
                {
                    string? fileName = response.Content.Headers.ContentDisposition?.FileName?.Replace("\"", string.Empty);

                    if (fileName is null)
                    {
                        throw new NullReferenceException(nameof(fileName));
                    }

                    filePath = Path.Combine(ArchivesDir, fileName);
                    using var sr = await response.Content.ReadAsStreamAsync();

                    // save received file
                    using var fs = File.Create(filePath);
                    sr.Seek(0, SeekOrigin.Begin);
                    sr.CopyTo(fs);
                    sr.Position = 0;

                    // check downloaded package hash
                    using var sha256 = SHA256.Create();
                    string hash = BitConverter.ToString(sha256.ComputeHash(sr)).Replace("-", string.Empty).ToLower();

                    if (!hash.Equals(appRecord.PackageSha256.ToLower()))
                    {
                        throw new Exception(Strings.HashSumMismatch);
                    }
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }

                // extract downloaded zip
                ZipFile.ExtractToDirectory(filePath, ExtractedDir);

                // start file copy/replace
                LaunchUpdateProcess();
            }
            else
            {
                throw new Exception($"'{nameof(CheckUpdateIsAvailable)}' {Strings.MethodNeedsToBeCalledFirst}");
            }
        }

        /// <summary>
        /// Launches a background process, that finishes update procedure.
        /// </summary>
        private static void LaunchUpdateProcess()
        {
            var psi = new ProcessStartInfo
            {
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = $"/C timeout 5 &&" +
                $"copy /Y \"{ExtractedDir}\\*\" \"{ApplicationInfo.BaseDirectory}\" &&" +
                $"rmdir /S /Q \"{TmpDir}\" &&" +
                $"\"{ApplicationInfo.ExePath}\"",
            };

            var p = new Process { StartInfo = psi };
            p.Start();
        }

        /// <summary>
        /// Deletes temporary directory and all of its contents.
        /// </summary>
        private static void CleanTmpDirectory()
        {
            if (Directory.Exists(TmpDir))
            {
                Directory.Delete(TmpDir, true);
            }
        }

        /// <summary>
        /// Creates temporary directory.
        /// </summary>
        private static void CreateDirectoryStructure()
        {
            if (!Directory.Exists(TmpDir))
            {
                var di = Directory.CreateDirectory(TmpDir);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            if (!Directory.Exists(ArchivesDir))
            {
                Directory.CreateDirectory(ArchivesDir);
            }

            if (!Directory.Exists(ExtractedDir))
            {
                Directory.CreateDirectory(ExtractedDir);
            }
        }
    }
}
