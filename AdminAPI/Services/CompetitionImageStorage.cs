namespace AdminAPI.Services;

public static class CompetitionImageStorage
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];

    public const string RequestPath = "/uploads/competitions";

    public static async Task<string?> SaveAsync(
        IFormFile? file,
        string uploadDirectory,
        CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length == 0)
            return null;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return null;

        Directory.CreateDirectory(uploadDirectory);
        var fileName = Guid.NewGuid().ToString("N") + extension;
        var path = Path.Combine(uploadDirectory, fileName);

        await using var stream = File.Create(path);
        await file.CopyToAsync(stream, cancellationToken);

        return $"{RequestPath}/{fileName}";
    }

    public static string? NormalizeImageUrl(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return null;

        return imageUrl
            .Replace("~/Uploads/competitions/", $"{RequestPath}/", StringComparison.OrdinalIgnoreCase)
            .Replace("~/uploads/competitions/", $"{RequestPath}/", StringComparison.OrdinalIgnoreCase);
    }

    public static void DeleteIfExists(string? imageUrl, string uploadDirectory)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return;

        var fileName = Path.GetFileName(imageUrl);
        if (string.IsNullOrEmpty(fileName))
            return;

        var fullPath = Path.Combine(uploadDirectory, fileName);
        if (!File.Exists(fullPath))
            return;

        try
        {
            File.Delete(fullPath);
        }
        catch
        {
            // ignored — matches legacy WebForms behavior
        }
    }
}
