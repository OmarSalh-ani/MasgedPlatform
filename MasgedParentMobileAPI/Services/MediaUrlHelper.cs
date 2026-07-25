namespace MasgedParentMobileAPI.Services;

public static class MediaUrlHelper
{
    public static string? Resolve(string? path, string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(path))
            return null;

        if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return path;

        var baseUri = baseUrl.TrimEnd('/');
        // DB paths like ~/uploads/photo.jpg or ~/Uploads/photo.jpg
        var relative = path.Trim().Replace('\\', '/');
        if (relative.StartsWith("~/", StringComparison.Ordinal))
            relative = relative[2..];
        else if (relative.StartsWith('~'))
            relative = relative[1..];
        relative = relative.TrimStart('/');

        return $"{baseUri}/{relative}";
    }
}
