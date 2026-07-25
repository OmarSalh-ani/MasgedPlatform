namespace MasgedParentMobileAPI.Services;

public static class StudentPhotoStorage
{
    public static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png"];
    public const long MaxBytes = 1_048_576;

    public static async Task<string?> SaveAsync(
        IFormFile file,
        string uploadDirectory,
        CancellationToken cancellationToken = default)
    {
        if (file.Length == 0 || file.Length > MaxBytes)
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
}
