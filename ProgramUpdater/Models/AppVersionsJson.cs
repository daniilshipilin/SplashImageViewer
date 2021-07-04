namespace ProgramUpdater.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class AppVersionsJson
    {
        [JsonProperty("entries")]
        public IList<Entry>? AppEntries { get; set; }

        public class Entry
        {
            [JsonProperty("guid")]
            public Guid GUID { get; set; }

            [JsonProperty("info")]
            public string? AppInfo { get; set; }

            [JsonProperty("semver")]
            public string? SemVer { get; set; }

            [JsonProperty("downloadurl")]
            public string? DownloadUrl { get; set; }

            [JsonProperty("packagesha256")]
            public string? PackageSha256 { get; set; }

            public override string ToString() =>
                $"GUID: {this.GUID}{Environment.NewLine}" +
                $"Info: {this.AppInfo}{Environment.NewLine}" +
                $"Version: {this.SemVer}{Environment.NewLine}" +
                $"DownloadUrl: {this.DownloadUrl}{Environment.NewLine}" +
                $"Sha256: {this.PackageSha256}{Environment.NewLine}";
        }
    }
}
