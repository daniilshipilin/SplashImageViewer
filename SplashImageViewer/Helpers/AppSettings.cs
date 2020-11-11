namespace SplashImageViewer.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Microsoft.Win32;

    public static class AppSettings
    {
        public const string RegistryBaseKey =
#if DEBUG
            @"SOFTWARE\Splash Image Viewer [Debug]";
#else
            @"SOFTWARE\Splash Image Viewer";
#endif

        public const int ConfigVersion = 12;
        public const int RecentItemsCapacity = 10;
        public const string RegistryRecentItemsKey = RegistryBaseKey + "\\Recent Items";
        public const string RegistryProgramUpdaterKey = RegistryBaseKey + "\\Program Updater";
        public const int MinScreenSizeWidth = 1024;
        public const int MinScreenSizeHeight = 768;

        private static readonly RegistryKey RegKeyRoot = Registry.CurrentUser.CreateSubKey(RegistryBaseKey);
        private static readonly RegistryKey RegKeyRecentItems = Registry.CurrentUser.CreateSubKey(RegistryRecentItemsKey);
        private static readonly RegistryKey RegKeyProgramUpdater = Registry.CurrentUser.CreateSubKey(RegistryProgramUpdaterKey);

        private static readonly IReadOnlyDictionary<string, object> DefaultSettingsDict = new Dictionary<string, object>()
        {
            { nameof(ConfigVersion), ConfigVersion },
            { nameof(ThemeColor), "FF000000" },
            { nameof(SlideshowTransitionMs), 10000 },
            { nameof(SlideshowOrderIsRandom), false },
            { nameof(SearchInSubdirs), false },
            { nameof(ShowFileDeletePrompt), true },
            { nameof(ShowFileOverwritePrompt), true },
            { nameof(ForceCheckUpdates), false },
            { nameof(UpdatesLastCheckedUtcTimestamp), default(DateTime).ToString("u", CultureInfo.InvariantCulture) }, // assign default datetime struct value
        };

        public static string RegistryBaseKeyFull => RegKeyRoot.Name;

        public static Color LabelsColor => ((uint)ThemeColor.ToArgb() > 0xFF808080) ? Color.Black : Color.White;

        public static Color ThemeColor
        {
            get => Color.FromArgb(Convert.ToInt32((string)RegKeyRoot.GetValue(nameof(ThemeColor)), 16));

            set
            {
                RegKeyRoot.SetValue(nameof(ThemeColor), Convert.ToString(value.ToArgb(), 16).ToUpper());
            }
        }

        public static int SlideshowTransitionMs
        {
            get => (int)RegKeyRoot.GetValue(nameof(SlideshowTransitionMs));

            set
            {
                RegKeyRoot.SetValue(nameof(SlideshowTransitionMs), value);
            }
        }

        public static bool SlideshowOrderIsRandom
        {
            get => bool.Parse((string)RegKeyRoot.GetValue(nameof(SlideshowOrderIsRandom)));

            set
            {
                RegKeyRoot.SetValue(nameof(SlideshowOrderIsRandom), value);
            }
        }

        public static SearchOption SearchInSubdirs
        {
            get => bool.Parse((string)RegKeyRoot.GetValue(nameof(SearchInSubdirs))) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            set
            {
                RegKeyRoot.SetValue(nameof(SearchInSubdirs), value == SearchOption.AllDirectories);
            }
        }

        public static bool ShowFileDeletePrompt
        {
            get => bool.Parse((string)RegKeyRoot.GetValue(nameof(ShowFileDeletePrompt)));

            set
            {
                RegKeyRoot.SetValue(nameof(ShowFileDeletePrompt), value);
            }
        }

        public static bool ShowFileOverwritePrompt
        {
            get => bool.Parse((string)RegKeyRoot.GetValue(nameof(ShowFileOverwritePrompt)));

            set
            {
                RegKeyRoot.SetValue(nameof(ShowFileOverwritePrompt), value);
            }
        }

        public static bool ForceCheckUpdates
        {
            get => bool.Parse((string)RegKeyRoot.GetValue(nameof(ForceCheckUpdates)));

            set
            {
                RegKeyRoot.SetValue(nameof(ForceCheckUpdates), value);
            }
        }

        public static DateTime UpdatesLastCheckedUtcTimestamp => DateTime.ParseExact((string)RegKeyRoot.GetValue(nameof(UpdatesLastCheckedUtcTimestamp)), "u", CultureInfo.InvariantCulture);

        public static string AppVersionsXmlUrl
        {
            get => (string)RegKeyProgramUpdater.GetValue(nameof(AppVersionsXmlUrl));

            private set
            {
                RegKeyProgramUpdater.SetValue(nameof(AppVersionsXmlUrl), value);
            }
        }

        public static void UpdateUpdatesLastCheckedUtcTimestamp()
        {
            RegKeyRoot.SetValue(nameof(UpdatesLastCheckedUtcTimestamp), DateTime.UtcNow.ToString("u", CultureInfo.InvariantCulture));
        }

        public static void CheckSettings()
        {
            object version = RegKeyRoot.GetValue(nameof(ConfigVersion));

            if (version is null || (int)version != ConfigVersion)
            {
                ResetSettings();
            }
        }

        public static void ResetSettings()
        {
            // clear root config reg keys
            ClearRegistryKey(RegKeyRoot);

            // clear recent items reg keys
            ClearRegistryKey(RegKeyRecentItems);

            // set default values
            foreach (var pair in DefaultSettingsDict)
            {
                RegKeyRoot.SetValue(pair.Key, pair.Value);
            }

            // check, whether required key for the Program Updater exists
            if (AppVersionsXmlUrl is null)
            {
                // create key with default value
                AppVersionsXmlUrl = string.Empty;
            }
        }

        public static IList<string> GetRecentItemsFromRegistry()
        {
            return RegKeyRecentItems.GetValueNames().ToList();
        }

        public static void WriteRecentItemsToRegistry(IList<string> items)
        {
            // clear recent items reg keys
            ClearRegistryKey(RegKeyRecentItems);

            // write a list of items to the registry
            foreach (var item in items)
            {
                RegKeyRecentItems.SetValue(item, string.Empty);
            }
        }

        private static void ClearRegistryKey(RegistryKey regKey)
        {
            foreach (var key in regKey.GetValueNames())
            {
                regKey.DeleteValue(key);
            }
        }
    }
}
