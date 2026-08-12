namespace AdminAPI.Services;

public static class CircleMemorizationRevisionReportAssets
{
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
}
