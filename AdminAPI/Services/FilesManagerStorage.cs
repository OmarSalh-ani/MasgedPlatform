namespace AdminAPI.Services;

public static class FilesManagerStorage
{
    public const string RequestPath = "/FilesManager";
    public const string DbPathPrefix = "~/FilesManager/";

    public static async Task<string?> SaveAsync(
        IFormFile file,
        string uploadDirectory,
        CancellationToken cancellationToken = default)
    {
        if (file.Length == 0)
            return null;

        var fileName = Path.GetFileName(file.FileName);
        if (string.IsNullOrEmpty(fileName))
            return null;

        Directory.CreateDirectory(uploadDirectory);
        var path = Path.Combine(uploadDirectory, fileName);

        await using var stream = File.Create(path);
        await file.CopyToAsync(stream, cancellationToken);

        return $"{DbPathPrefix}{fileName}";
    }

    public static string? GetFileName(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return null;

        return Path.GetFileName(filePath.Replace("~/FilesManager/", "", StringComparison.OrdinalIgnoreCase)
            .Replace("/FilesManager/", "", StringComparison.OrdinalIgnoreCase));
    }

    public static void DeleteIfExists(string? filePath, string uploadDirectory)
    {
        var fileName = GetFileName(filePath);
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

    public static string BuildPublicUrl(string? filePath, string publicBaseUrl)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return string.Empty;

        var relative = filePath.Trim().Replace("~", "", StringComparison.Ordinal);
        if (!relative.StartsWith('/'))
            relative = "/" + relative;

        return publicBaseUrl.TrimEnd('/') + relative;
    }
}
