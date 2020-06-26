using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace SplashImageViewer.Helpers
{
    public static class AppSettings
    {
#if DEBUG
        public const string REGISTRY_BASE_KEY_FULL = @"HKCU\SOFTWARE\Splash Image Viewer [Debug]";
        public const string REGISTRY_BASE_KEY = @"SOFTWARE\Splash Image Viewer [Debug]";
#else
        public const string REGISTRY_BASE_KEY_FULL = @"HKCU\SOFTWARE\Splash Image Viewer";
        public const string REGISTRY_BASE_KEY = @"SOFTWARE\Splash Image Viewer";
#endif
        const int CONFIG_VER = 10;

        public const string REGISTRY_RECENT_ITEMS_KEY = REGISTRY_BASE_KEY + "\\Recent Items";

        static readonly RegistryKey _regKeyRoot = Registry.CurrentUser.CreateSubKey(REGISTRY_BASE_KEY);
        static readonly RegistryKey _regKeyRecentItems = Registry.CurrentUser.CreateSubKey(REGISTRY_RECENT_ITEMS_KEY);

        public static Color LabelsColor => ((uint)ThemeColor.ToArgb() > 0xFF808080) ? Color.Black : Color.White;

        public static Color ThemeColor
        {
            get
            {
                return Color.FromArgb(Convert.ToInt32((string)_regKeyRoot.GetValue(nameof(ThemeColor)), 16));
            }
            set
            {
                _regKeyRoot.SetValue(nameof(ThemeColor), Convert.ToString(value.ToArgb(), 16).ToUpper());
            }
        }

        public static int SlideshowTransitionMs
        {
            get
            {
                return int.Parse((string)_regKeyRoot.GetValue(nameof(SlideshowTransitionMs)));
            }
            set
            {
                _regKeyRoot.SetValue(nameof(SlideshowTransitionMs), value.ToString());
            }
        }

        public static bool SlideshowOrderIsRandom
        {
            get
            {
                return bool.Parse((string)_regKeyRoot.GetValue(nameof(SlideshowOrderIsRandom)));
            }
            set
            {
                _regKeyRoot.SetValue(nameof(SlideshowOrderIsRandom), value.ToString());
            }
        }

        public static SearchOption SearchInSubdirs
        {
            get
            {
                return bool.Parse((string)_regKeyRoot.GetValue(nameof(SearchInSubdirs))) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            }
            set
            {
                _regKeyRoot.SetValue(nameof(SearchInSubdirs), (value == SearchOption.AllDirectories));
            }
        }

        public static bool ShowFileDeletePrompt
        {
            get
            {
                return bool.Parse((string)_regKeyRoot.GetValue(nameof(ShowFileDeletePrompt)));
            }
            set
            {
                _regKeyRoot.SetValue(nameof(ShowFileDeletePrompt), value.ToString());
            }
        }

        public static bool ShowFileOverwritePrompt
        {
            get
            {
                return bool.Parse((string)_regKeyRoot.GetValue(nameof(ShowFileOverwritePrompt)));
            }
            set
            {
                _regKeyRoot.SetValue(nameof(ShowFileOverwritePrompt), value.ToString());
            }
        }

        public static bool ForceCheckUpdates
        {
            get
            {
                return bool.Parse((string)_regKeyRoot.GetValue(nameof(ForceCheckUpdates)));
            }
            set
            {
                _regKeyRoot.SetValue(nameof(ForceCheckUpdates), value.ToString());
            }
        }

        public static DateTime UpdatesLastChecked
        {
            get
            {
                return DateTime.Parse((string)_regKeyRoot.GetValue(nameof(UpdatesLastChecked)));
            }
            set
            {
                _regKeyRoot.SetValue(nameof(UpdatesLastChecked), value.ToString());
            }
        }

        static readonly Dictionary<string, string> _defaultSettingsDict = new Dictionary<string, string>()
        {
            { nameof(ThemeColor), "FF000000" },
            { nameof(SlideshowTransitionMs), "10000" },
            { nameof(SlideshowOrderIsRandom), "False" },
            { nameof(SearchInSubdirs), "False" },
            { nameof(ShowFileDeletePrompt), "True" },
            { nameof(ShowFileOverwritePrompt), "True" },
            { nameof(ForceCheckUpdates), "False" },
            { nameof(UpdatesLastChecked), DateTime.Now.ToString() }
        };

        public static int MinScreenSizeWidth => 1024;
        public static int MinScreenSizeHeight => 768;

        public static void CheckSettings()
        {
            int? version = (int?)_regKeyRoot.GetValue("ConfigVersion");

            if (version == null || version != CONFIG_VER)
            {
                // set config version
                _regKeyRoot.SetValue("ConfigVersion", CONFIG_VER);
                ResetSettings();

                // clear recent items reg keys
                foreach (var key in _regKeyRecentItems.GetValueNames())
                {
                    _regKeyRecentItems.DeleteValue(key);
                }
            }
            else
            {
                // check if keys exist / set default values, if they don't
                foreach (var pair in _defaultSettingsDict)
                {
                    if (_regKeyRoot.GetValue(pair.Key) == null)
                    {
                        _regKeyRoot.SetValue(pair.Key, pair.Value);
                    }
                }
            }
        }

        public static void ResetSettings()
        {
            // set default values
            foreach (var pair in _defaultSettingsDict)
            {
                _regKeyRoot.SetValue(pair.Key, pair.Value);
            }
        }

        public static List<string> GetRecentItemsFromRegistry()
        {
            var items = new List<string>();

            if (_regKeyRecentItems.ValueCount != 0)
            {
                foreach (var key in _regKeyRecentItems.GetValueNames())
                {
                    if (File.Exists(key)) { items.Add(key); }
                    else { _regKeyRecentItems.DeleteValue(key); }
                }
            }

            return items;
        }

        public static void WriteRecentItemsToRegistry(List<string> items)
        {
            // delete reg items, that don't exist in items list
            foreach (var key in _regKeyRecentItems.GetValueNames())
            {
                bool keyNotPresent = true;

                foreach (var item in items)
                {
                    if (item.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        keyNotPresent = false;
                        break;
                    }
                }

                if (keyNotPresent)
                {
                    _regKeyRecentItems.DeleteValue(key);
                }
            }

            // write a list of items to the registry
            for (int i = 0; i < items.Count; i++)
            {
                _regKeyRecentItems.SetValue(items[i], string.Empty);
            }
        }
    }
}
