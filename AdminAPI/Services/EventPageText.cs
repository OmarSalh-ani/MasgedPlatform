namespace AdminAPI.Services;

public static class EventPageText
{
    public static string Required(string? value) => (value ?? string.Empty).Trim();

    public static string? Optional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    public static string NormalizeSlug(string? slug) =>
        Required(slug).ToLowerInvariant();
}
