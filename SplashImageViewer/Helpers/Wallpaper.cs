namespace SplashImageViewer.Helpers;

public class Wallpaper
{
    private const int SPISETDESKWALLPAPER = 20;
    private const int SPIFUPDATEINIFILE = 0x01;
    private const int SPIFSENDWININICHANGE = 0x02;

    public enum Style : int
    {
        Fill,
        Fit,
        Span,
        Stretch,
        Tile,
        Center,
    }

    public static void SetDesktopBackground(Image img, Style style = Style.Fill)
    {
        // creating tmp image (as bmp)
        string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
        img.Save(tempPath, ImageFormat.Bmp);

        using var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

        if (key is null)
        {
            throw new NullReferenceException(nameof(key));
        }

        switch (style)
        {
            case Style.Fill:
                key.SetValue(@"WallpaperStyle", "10");
                key.SetValue(@"TileWallpaper", "0");
                break;

            case Style.Fit:
                key.SetValue(@"WallpaperStyle", "6");
                key.SetValue(@"TileWallpaper", "0");
                break;

            case Style.Span:
                key.SetValue(@"WallpaperStyle", "22");
                key.SetValue(@"TileWallpaper", "0");
                break;

            case Style.Stretch:
                key.SetValue(@"WallpaperStyle", "2");
                key.SetValue(@"TileWallpaper", "0");
                break;

            case Style.Tile:
                key.SetValue(@"WallpaperStyle", "0");
                key.SetValue(@"TileWallpaper", "1");
                break;

            case Style.Center:
                key.SetValue(@"WallpaperStyle", "0");
                key.SetValue(@"TileWallpaper", "0");
                break;

            default:
                break;
        }

        int result = SystemParametersInfo(
            SPISETDESKWALLPAPER,
            0,
            tempPath,
            SPIFUPDATEINIFILE | SPIFSENDWININICHANGE);

        if (result != 1)
        {
            throw new Exception("Desktop background set failed");
        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
}
