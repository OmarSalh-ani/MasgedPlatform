namespace AdminAPI.Services;

public static class TeacherImageStorage
{
    public const string RequestPath = "/uploads";
    public static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];

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

        return fileName;
    }

    public static void DeleteIfExists(string? imageFileName, string uploadDirectory)
    {
        if (string.IsNullOrEmpty(imageFileName))
            return;

        var fullPath = Path.Combine(uploadDirectory, Path.GetFileName(imageFileName));
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

    public static string? BuildPublicImageUrl(string? imageFileName, string publicSiteBaseUrl)
    {
        if (string.IsNullOrWhiteSpace(imageFileName))
            return null;

        var fileName = Path.GetFileName(imageFileName.Trim());
        if (string.IsNullOrEmpty(fileName))
            return null;

        var baseUrl = publicSiteBaseUrl.TrimEnd('/');
        return $"{baseUrl}{RequestPath}/{fileName}";
    }
}
