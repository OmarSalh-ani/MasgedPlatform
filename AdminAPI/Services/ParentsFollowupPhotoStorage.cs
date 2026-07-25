namespace AdminAPI.Services;

public static class ParentsFollowupPhotoStorage
{
    public static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png"];
    public const long MaxBytes = 1_048_576;
    public const string RequestPath = "/uploads";

    public static async Task<string?> SaveAsync(
        IFormFile file,
        string uploadDirectory,
        CancellationToken cancellationToken = default)
    {
        if (file.Length == 0)
            return null;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return null;

        Directory.CreateDirectory(uploadDirectory);
        var fileName = Guid.NewGuid().ToString() + extension;
        var path = Path.Combine(uploadDirectory, fileName);

        await using var stream = File.Create(path);
        await file.CopyToAsync(stream, cancellationToken);

        return $"~/uploads/{fileName}";
    }

    public static string? NormalizePhotoUrl(string? photoPath, string? requestBaseUrl = null)
    {
        if (string.IsNullOrWhiteSpace(photoPath))
            return null;

        var url = photoPath.Trim().Replace("~", "", StringComparison.Ordinal);
        if (url.StartsWith('/'))
        {
            return requestBaseUrl is null
                ? url
                : $"{requestBaseUrl.TrimEnd('/')}{url}";
        }

        return $"{RequestPath}/{url.TrimStart('/')}";
    }
}
