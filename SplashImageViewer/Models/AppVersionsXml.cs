namespace SplashImageViewer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot("root")]
    public class AppVersionsXml
    {
        [XmlElement("record")]
        public List<Record>? AppRecords { get; set; }

        public class Record
        {
            [XmlElement("guid")]
            public Guid GUID { get; set; }

            [XmlElement("info")]
            public string AppInfo { get; set; } = string.Empty;

            [XmlElement("semver")]
            public string SemVer { get; set; } = string.Empty;

            [XmlElement("downloadurl")]
            public string DownloadUrl { get; set; } = string.Empty;
        }
    }
}

// <?xml version = "1.0" encoding = "UTF-8"?>
// <root>
//     <record>
//         <guid></guid>
//         <info></info>
//         <semver></semver>
//         <downloadurl></downloadurl>
//     </record>
// </root>
