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
        public const string RegistryBaseKey = @"SOFTWARE\Illuminati Software Inc.";

        public const string RegistrySplashImageViewerKey =
#if DEBUG
            RegistryBaseKey + "\\Splash Image Viewer [Debug]";
#else
            RegistryBaseKey + "\\Splash Image Viewer";
#endif

        public const string RegistryRecentItemsKey = RegistrySplashImageViewerKey + "\\Recent Items";

        public const int CurrentConfigVersion = 21;
        public const int RecentItemsCapacity = 10;
        public const int MinScreenSizeWidth = 1024;
        public const int MinScreenSizeHeight = 768;
        public const int FullscreenFormHideInfoTimerIntervalMs = 10000;
        public const int MainFormCheckMemoryMs = 1000;
        public const int MainFormSlideshowProgressBarUpdateMs = 10;

        private static readonly RegistryKey RegKeySplashImageViewer = Registry.CurrentUser.CreateSubKey(RegistrySplashImageViewerKey);
        private static readonly RegistryKey RegKeyRecentItems = Registry.CurrentUser.CreateSubKey(RegistryRecentItemsKey);

        private static readonly IReadOnlyDictionary<string, object> DefaultSettingsDict = new Dictionary<string, object>()
        {
            { nameof(ConfigVersion), CurrentConfigVersion },
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

        public static int LabelsColorArgb => (ThemeColorArgb > unchecked((int)0xFF808080)) ? unchecked((int)0xFF000000) : unchecked((int)0xFFFFFFFF);

        public static int? ConfigVersion
        {
            get => (int?)RegKeySplashImageViewer.GetValue(nameof(ConfigVersion));

            set => RegKeySplashImageViewer.SetValue(nameof(ConfigVersion), value ?? 0);
        }

        public static int ThemeColorArgb
        {
            get => (int?)RegKeySplashImageViewer.GetValue(nameof(ThemeColorArgb)) ?? 0;

            set => RegKeySplashImageViewer.SetValue(nameof(ThemeColorArgb), value);
        }

        public static int SlideshowTransitionSec
        {
            get => (int?)RegKeySplashImageViewer.GetValue(nameof(SlideshowTransitionSec)) ?? 0;

            set => RegKeySplashImageViewer.SetValue(nameof(SlideshowTransitionSec), value);
        }

        public static bool SlideshowOrderIsRandom
        {
            get => bool.Parse((string?)RegKeySplashImageViewer.GetValue(nameof(SlideshowOrderIsRandom)) ?? string.Empty);

            set => RegKeySplashImageViewer.SetValue(nameof(SlideshowOrderIsRandom), value);
        }

        public static SearchOption SearchInSubdirs
        {
            get => bool.Parse((string?)RegKeySplashImageViewer.GetValue(nameof(SearchInSubdirs)) ?? string.Empty) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            set => RegKeySplashImageViewer.SetValue(nameof(SearchInSubdirs), value == SearchOption.AllDirectories);
        }

        public static bool ShowFileDeletePrompt
        {
            get => bool.Parse((string?)RegKeySplashImageViewer.GetValue(nameof(ShowFileDeletePrompt)) ?? string.Empty);

            set => RegKeySplashImageViewer.SetValue(nameof(ShowFileDeletePrompt), value);
        }

        public static bool ShowFileOverwritePrompt
        {
            get => bool.Parse((string?)RegKeySplashImageViewer.GetValue(nameof(ShowFileOverwritePrompt)) ?? string.Empty);

            set => RegKeySplashImageViewer.SetValue(nameof(ShowFileOverwritePrompt), value);
        }

        public static bool ForceCheckUpdates
        {
            get => bool.Parse((string?)RegKeySplashImageViewer.GetValue(nameof(ForceCheckUpdates)) ?? string.Empty);

            set => RegKeySplashImageViewer.SetValue(nameof(ForceCheckUpdates), value);
        }

        public static DateTime UpdatesLastCheckedTimestamp
        {
            get => DateTime.ParseExact((string?)RegKeySplashImageViewer.GetValue(nameof(UpdatesLastCheckedTimestamp)) ?? string.Empty, "s", CultureInfo.InvariantCulture);

            private set => RegKeySplashImageViewer.SetValue(nameof(UpdatesLastCheckedTimestamp), value.ToString("s", CultureInfo.InvariantCulture));
        }

        public static CultureInfo CurrentUICulture
        {
            get => CultureInfo.GetCultureInfo((string?)RegKeySplashImageViewer.GetValue(nameof(CurrentUICulture)) ?? string.Empty);

            set => RegKeySplashImageViewer.SetValue(nameof(CurrentUICulture), value.Name);
        }

        public static int ScreenSizeWidth
        {
            get => (int?)RegKeySplashImageViewer.GetValue(nameof(ScreenSizeWidth)) ?? 0;

            set => RegKeySplashImageViewer.SetValue(nameof(ScreenSizeWidth), value);
        }

        public static int ScreenSizeHeight
        {
            get => (int?)RegKeySplashImageViewer.GetValue(nameof(ScreenSizeHeight)) ?? 0;

            set => RegKeySplashImageViewer.SetValue(nameof(ScreenSizeHeight), value);
        }

        public static bool ScreenIsMaximized
        {
            get => bool.Parse((string?)RegKeySplashImageViewer.GetValue(nameof(ScreenIsMaximized)) ?? string.Empty);

            set => RegKeySplashImageViewer.SetValue(nameof(ScreenIsMaximized), value);
        }

        public static void UpdateUpdatesLastCheckedTimestamp() => UpdatesLastCheckedTimestamp = DateTime.Now;

        public static void CheckSettings()
        {
            if (ConfigVersion is null or not CurrentConfigVersion)
            {
                ResetSettings();
            }
        }

        public static void ResetSettings()
        {
            // clear root config reg keys
            ClearRegistryKey(RegKeySplashImageViewer);

            // clear recent items reg keys
            ClearRegistryKey(RegKeyRecentItems);

            // set default values
            foreach (var pair in DefaultSettingsDict)
            {
                RegKeySplashImageViewer.SetValue(pair.Key, pair.Value);
            }
        }

        public static IList<string> GetRecentItemsFromRegistry() => RegKeyRecentItems.GetValueNames().ToList();

        public static void WriteRecentItemsToRegistry(IList<string> items)
        {
            // clear recent items reg keys
            ClearRegistryKey(RegKeyRecentItems);

            // write a list of items to the registry
            foreach (string? item in items)
            {
                RegKeyRecentItems.SetValue(item, string.Empty);
            }
        }

        private static void ClearRegistryKey(RegistryKey regKey)
        {
            foreach (string? key in regKey.GetValueNames())
            {
                regKey.DeleteValue(key);
            }
        }
    }
}
