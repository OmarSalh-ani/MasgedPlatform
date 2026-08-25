namespace AdminAPI.Services;

public static class EventPageImageStorage
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif"];
    public const string RequestPath = "/uploads/event-pages";

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
            .Replace("~/Uploads/EventPages/", $"{RequestPath}/", StringComparison.OrdinalIgnoreCase)
            .Replace("~/uploads/event-pages/", $"{RequestPath}/", StringComparison.OrdinalIgnoreCase);
    }
}
