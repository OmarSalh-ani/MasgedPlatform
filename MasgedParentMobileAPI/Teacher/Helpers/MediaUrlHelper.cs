namespace MasgedTeacherMobileAPI.Helpers;

public static class MediaUrlHelper
{
    private const string SiteBaseUrl = "https://admin-api.mosque-mbark-j.com";

    public static string Resolve(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        var trimmed = url.Trim();
        if (trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return trimmed;

        if (trimmed.StartsWith("~/", StringComparison.Ordinal))
            trimmed = trimmed[2..];

        trimmed = trimmed.TrimStart('/');
        return $"{SiteBaseUrl}/{trimmed}";
    }
}
