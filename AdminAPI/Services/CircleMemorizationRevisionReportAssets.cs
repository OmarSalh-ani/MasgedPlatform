using QuestPDF.Drawing;

namespace AdminAPI.Services;

public static class CircleMemorizationRevisionReportAssets
{
    public const string FontRegular = "Cairo";
    public const string FontBold = "Cairo-Bold";

    private static bool _fontsRegistered;

    public static string? ResolveLogoPath()
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "Assets", "logo.png"),
            Path.Combine(Directory.GetCurrentDirectory(), "Assets", "logo.png"),
        };
        return candidates.FirstOrDefault(File.Exists);
    }

    public static byte[]? TryReadLogoBytes()
    {
        var path = ResolveLogoPath();
        if (path is null)
            return null;
        try
        {
            return File.ReadAllBytes(path);
        }
        catch
        {
            return null;
        }
    }

    public static void EnsureFontsRegistered()
    {
        if (_fontsRegistered)
            return;

        RegisterFontFile(FontRegular, "Cairo-Regular.woff");
        RegisterFontFile(FontBold, "Cairo-Bold.woff");
        _fontsRegistered = true;
    }

    private static void RegisterFontFile(string fontName, string fileName)
    {
        var path = ResolveFontPath(fileName);
        if (path is null)
            return;

        try
        {
            using var stream = File.OpenRead(path);
            FontManager.RegisterFontWithCustomName(fontName, stream);
        }
        catch
        {
            // Fall back to QuestPDF default fonts if Cairo is unavailable.
        }
    }

    private static string? ResolveFontPath(string fileName)
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "Assets", "Fonts", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Fonts", fileName),
        };

        return candidates.FirstOrDefault(File.Exists);
    }
}
