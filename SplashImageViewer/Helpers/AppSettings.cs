namespace SplashImageViewer.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Microsoft.Win32;

    public static class AppSettings
    {
        public const string RegistryBaseKey =
#if DEBUG
            @"SOFTWARE\Illuminati Software Inc.\Splash Image Viewer [Debug]";

#else
            @"SOFTWARE\Illuminati Software Inc.\Splash Image Viewer";
#endif

        public const int ConfigVersion = 18;
        public const int RecentItemsCapacity = 10;
        public const string RegistryRecentItemsKey = RegistryBaseKey + "\\Recent Items";
        public const string RegistryProgramUpdaterKey = RegistryBaseKey + "\\Program Updater";
        public const int MinScreenSizeWidth = 1024;
        public const int MinScreenSizeHeight = 768;
        public const int FullscreenFormHideInfoTimerIntervalMs = 10000;
        public const int MainFormCheckMemoryMs = 1000;
        public const int MainFormSlideshowProgressBarUpdateMs = 10;

        private static readonly RegistryKey RegKeyRoot = Registry.CurrentUser.CreateSubKey(RegistryBaseKey);
        private static readonly RegistryKey RegKeyRecentItems = Registry.CurrentUser.CreateSubKey(RegistryRecentItemsKey);
        private static readonly RegistryKey RegKeyProgramUpdater = Registry.CurrentUser.CreateSubKey(RegistryProgramUpdaterKey);

        private static readonly IReadOnlyDictionary<string, object> DefaultSettingsDict = new Dictionary<string, object>()
        {
            { nameof(ConfigVersion), ConfigVersion },
            { nameof(ThemeColorArgb), unchecked((int)0xFF000000) }, // black
            { nameof(SlideshowTransitionSec), 10 },
            { nameof(SlideshowOrderIsRandom), false },
            { nameof(SearchInSubdirs), false },
            { nameof(ShowFileDeletePrompt), true },
            { nameof(ShowFileOverwritePrompt), true },
            { nameof(ForceCheckUpdates), false },
            { nameof(UpdatesLastCheckedTimestamp), default(DateTime).ToString("s") }, // assign default datetime struct value
            { nameof(CurrentUICulture), CultureInfo.GetCultureInfo("en") },
            { nameof(ScreenSizeWidth), MinScreenSizeWidth },
            { nameof(ScreenSizeHeight), MinScreenSizeHeight },
            { nameof(ScreenIsMaximized), false },
        };

        public static IReadOnlyList<CultureInfo> CultureInfos { get; } = new List<CultureInfo>()
        {
            CultureInfo.GetCultureInfo("en"),
            CultureInfo.GetCultureInfo("ru"),
        };

        public static IReadOnlyList<int> SlideshowTransitionsSec { get; } = new List<int>()
        {
            1, 2, 5, 10, 30, 60, 300, 600, 3600,
        };

        public static string RegistryBaseKeyFull => RegKeyRoot.Name;

        public static int LabelsColorArgb => (ThemeColorArgb > unchecked((int)0xFF808080)) ? unchecked((int)0xFF000000) : unchecked((int)0xFFFFFFFF);

        public static int ThemeColorArgb
        {
            get => (int?)RegKeyRoot.GetValue(nameof(ThemeColorArgb)) ?? 0;

            set
            {
                RegKeyRoot.SetValue(nameof(ThemeColorArgb), value);
            }
        }

        public static int SlideshowTransitionSec
        {
            get => (int?)RegKeyRoot.GetValue(nameof(SlideshowTransitionSec)) ?? 0;

            set
            {
                RegKeyRoot.SetValue(nameof(SlideshowTransitionSec), value);
            }
        }

        public static bool SlideshowOrderIsRandom
        {
            get => bool.Parse((string?)RegKeyRoot.GetValue(nameof(SlideshowOrderIsRandom)) ?? string.Empty);

            set
            {
                RegKeyRoot.SetValue(nameof(SlideshowOrderIsRandom), value);
            }
        }

        public static SearchOption SearchInSubdirs
        {
            get => bool.Parse((string?)RegKeyRoot.GetValue(nameof(SearchInSubdirs)) ?? string.Empty) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            set
            {
                RegKeyRoot.SetValue(nameof(SearchInSubdirs), value == SearchOption.AllDirectories);
            }
        }

        public static bool ShowFileDeletePrompt
        {
            get => bool.Parse((string?)RegKeyRoot.GetValue(nameof(ShowFileDeletePrompt)) ?? string.Empty);

            set
            {
                RegKeyRoot.SetValue(nameof(ShowFileDeletePrompt), value);
            }
        }

        public static bool ShowFileOverwritePrompt
        {
            get => bool.Parse((string?)RegKeyRoot.GetValue(nameof(ShowFileOverwritePrompt)) ?? string.Empty);

            set
            {
                RegKeyRoot.SetValue(nameof(ShowFileOverwritePrompt), value);
            }
        }

        public static bool ForceCheckUpdates
        {
            get => bool.Parse((string?)RegKeyRoot.GetValue(nameof(ForceCheckUpdates)) ?? string.Empty);

            set
            {
                RegKeyRoot.SetValue(nameof(ForceCheckUpdates), value);
            }
        }

        public static DateTime UpdatesLastCheckedTimestamp
        {
            get => DateTime.ParseExact((string?)RegKeyRoot.GetValue(nameof(UpdatesLastCheckedTimestamp)) ?? string.Empty, "s", CultureInfo.InvariantCulture);

            private set
            {
                RegKeyRoot.SetValue(nameof(UpdatesLastCheckedTimestamp), value.ToString("s", CultureInfo.InvariantCulture));
            }
        }

        public static CultureInfo CurrentUICulture
        {
            get => CultureInfo.GetCultureInfo((string?)RegKeyRoot.GetValue(nameof(CurrentUICulture)) ?? string.Empty);

            set
            {
                RegKeyRoot.SetValue(nameof(CurrentUICulture), value.Name);
            }
        }

        public static int ScreenSizeWidth
        {
            get => (int?)RegKeyRoot.GetValue(nameof(ScreenSizeWidth)) ?? 0;

            set
            {
                RegKeyRoot.SetValue(nameof(ScreenSizeWidth), value);
            }
        }

        public static int ScreenSizeHeight
        {
            get => (int?)RegKeyRoot.GetValue(nameof(ScreenSizeHeight)) ?? 0;

            set
            {
                RegKeyRoot.SetValue(nameof(ScreenSizeHeight), value);
            }
        }

        public static bool ScreenIsMaximized
        {
            get => bool.Parse((string?)RegKeyRoot.GetValue(nameof(ScreenIsMaximized)) ?? string.Empty);

            set
            {
                RegKeyRoot.SetValue(nameof(ScreenIsMaximized), value);
            }
        }

        public static string AppVersionsUrl
        {
            get => (string?)RegKeyProgramUpdater.GetValue(nameof(AppVersionsUrl)) ?? string.Empty;

            private set
            {
                RegKeyProgramUpdater.SetValue(nameof(AppVersionsUrl), value ?? string.Empty);
            }
        }

        public static void UpdateUpdatesLastCheckedTimestamp()
        {
            UpdatesLastCheckedTimestamp = DateTime.Now;
        }

        public static void CheckSettings()
        {
            object? version = RegKeyRoot.GetValue(nameof(ConfigVersion));

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
            if (AppVersionsUrl is null)
            {
                // create key with default value
                AppVersionsUrl = string.Empty;
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
