using AdminAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public static class CircleVisitRatingAssets
{
    public static async Task<byte[]?> TryReadLogoBytesAsync(
        AdminDbContext db,
        string uploadDirectory,
        CancellationToken cancellationToken = default)
    {
        var logoFileName = await db.MasgedSettings
            .AsNoTracking()
            .Select(x => x.LogoFileName)
            .FirstOrDefaultAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(logoFileName) && !string.IsNullOrWhiteSpace(uploadDirectory))
        {
            var uploadedPath = Path.Combine(uploadDirectory, Path.GetFileName(logoFileName));
            var uploaded = TryReadFile(uploadedPath);
            if (uploaded is { Length: > 0 })
                return uploaded;
        }

        return TryReadStaticLogo();
    }

    private static byte[]? TryReadStaticLogo()
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "Assets", "logo.png"),
            Path.Combine(Directory.GetCurrentDirectory(), "Assets", "logo.png"),
        };
        foreach (var path in candidates)
        {
            var bytes = TryReadFile(path);
            if (bytes is { Length: > 0 })
                return bytes;
        }

        return null;
    }

    private static byte[]? TryReadFile(string path)
    {
        if (!File.Exists(path))
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
