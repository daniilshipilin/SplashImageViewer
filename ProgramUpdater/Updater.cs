namespace ProgramUpdater
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using ProgramUpdater.Models;

    public class Updater
    {
        private static readonly HttpClient Client = new HttpClient()
        {
            Timeout = new TimeSpan(0, 0, 60),
        };

        private readonly string baseDir;
        private readonly string tmpDir;
        private readonly string archivesDir;
        private readonly string extractedDir;
        private readonly string exeLaunchPath;
        private readonly string appVersionsUrl = string.Empty;

        private AppVersionsJson.Entry? appEntry;

        public Updater(string baseDir, Version clientVersion, Guid appGuid, string exeLaunchPath, string? appVersionsUrl)
        {
            this.baseDir = Path.GetFullPath(baseDir);
            tmpDir = Path.Combine(this.baseDir, "update");
            archivesDir = Path.Combine(tmpDir, "Archives");
            extractedDir = Path.Combine(tmpDir, "Extracted");
            ClientVersion = clientVersion;
            ServerVersion = clientVersion;
            AppGUID = appGuid;
            this.exeLaunchPath = Path.GetFullPath(exeLaunchPath);

            if (appVersionsUrl is not null)
            {
                this.appVersionsUrl = appVersionsUrl;
            }
        }

        /// <summary>
        /// Gets current program version.
        /// </summary>
        public Version ClientVersion { get; }

        /// <summary>
        /// Gets latest program version, that resides on the server.
        /// </summary>
        public Version ServerVersion { get; private set; }

        /// <summary>
        /// Gets application GUID.
        /// </summary>
        public Guid AppGUID { get; }

        /// <summary>
        /// Gets a value indicating whether server program version is greater, than the current program version.
        /// </summary>
        public bool ServerVersionIsGreater => ServerVersion.CompareTo(ClientVersion) > 0;

        /// <summary>
        /// Gets a value indicating whether check update was performed.
        /// </summary>
        public bool CheckUpdateRequested => appEntry is not null;

        /// <summary>
        /// Gets a pre-formatted update prompt string.
        /// </summary>
        public string UpdatePromptFormatted =>
            $"Newer program version available.{Environment.NewLine}" +
            $"Current: {ClientVersion}{Environment.NewLine}" +
            $"Available: {ServerVersion}{Environment.NewLine}{Environment.NewLine}" +
            "Update program?";

        /// <summary>
        /// Checks, whether update is available.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> CheckUpdateIsAvailable()
        {
            using var response = await Client.GetAsync(appVersionsUrl);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();

                // deserialize received json
                var obj = JsonConvert.DeserializeObject<AppVersionsJson>(data);

                // find specific record, based on GUID
                var entry = obj?.AppEntries?.FirstOrDefault(x => x.GUID.Equals(AppGUID));

                if (entry is null || entry.SemVer is null)
                {
                    // guid was not found - throw exception
                    throw new Exception($"'{AppGUID}' GUID was not found in resulting json");
                }

                appEntry = entry;
                ServerVersion = Version.Parse(entry.SemVer);
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }

            return ServerVersionIsGreater;
        }

        /// <summary>
        /// Initiates program update without comparing server and client versions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task ForceUpdate()
        {
            await CheckUpdateIsAvailable();
            await Update();
        }

        /// <summary>
        /// Launches program update procedure.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Update()
        {
            if (appEntry is not null)
            {
                CleanTmpDirectory();
                CreateDirectoryStructure();
                string filePath;

                using var response = await Client.GetAsync(appEntry.DownloadUrl);

                if (response.IsSuccessStatusCode)
                {
                    string? fileName = response.Content.Headers.ContentDisposition?.FileName?.Replace("\"", string.Empty);

                    if (fileName is null)
                    {
                        throw new NullReferenceException(nameof(fileName));
                    }

                    filePath = Path.Combine(archivesDir, fileName);
                    byte[] data = await response.Content.ReadAsByteArrayAsync();

                    // save received file
                    File.WriteAllBytes(filePath, data);

                    // check downloaded package hash
                    using var sha256 = SHA256.Create();
                    string hash = BitConverter.ToString(sha256.ComputeHash(data)).Replace("-", string.Empty).ToLower();

                    if (!hash.Equals(appEntry.PackageSha256?.ToLower()))
                    {
                        throw new Exception("Downloaded package hash sum mismatch");
                    }
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }

                // extract downloaded zip
                ZipFile.ExtractToDirectory(filePath, extractedDir);

                // start file copy/replace
                LaunchUpdateProcess();
            }
            else
            {
                throw new Exception($"'{nameof(CheckUpdateIsAvailable)}' Method needs to be called first");
            }
        }

        /// <summary>
        /// Launches a background process, that finishes update procedure.
        /// </summary>
        private void LaunchUpdateProcess()
        {
            var psi = new ProcessStartInfo
            {
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = $"/C timeout 5 &&" +
                $"xcopy /E /Y \"{extractedDir}\\*\" \"{baseDir}\" &&" +
                $"rmdir /S /Q \"{tmpDir}\" &&" +
                $"\"{exeLaunchPath}\"",
            };

            var p = new Process { StartInfo = psi };
            p.Start();
        }

        /// <summary>
        /// Deletes temporary directory and all of its contents.
        /// </summary>
        private void CleanTmpDirectory()
        {
            if (Directory.Exists(tmpDir))
            {
                Directory.Delete(tmpDir, true);
            }
        }

        /// <summary>
        /// Creates temporary directory.
        /// </summary>
        private void CreateDirectoryStructure()
        {
            if (!Directory.Exists(tmpDir))
            {
                var di = Directory.CreateDirectory(tmpDir);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            if (!Directory.Exists(archivesDir))
            {
                Directory.CreateDirectory(archivesDir);
            }

            if (!Directory.Exists(extractedDir))
            {
                Directory.CreateDirectory(extractedDir);
            }
        }
    }
}
