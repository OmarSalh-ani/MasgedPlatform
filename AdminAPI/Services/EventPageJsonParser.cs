using System.Text.Json;
using AdminAPI.DTOs.EventPages;

namespace AdminAPI.Services;

public static class EventPageJsonParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static List<SaveEventPageTrackItemDto> ParseTracks(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return [];

        try
        {
            return JsonSerializer.Deserialize<List<SaveEventPageTrackItemDto>>(json, Options) ?? [];
        }
        catch (JsonException ex)
        {
            throw new ArgumentException("صيغة المسارات غير صحيحة", ex);
        }
    }

    public static List<SaveEventPageFormFieldItemDto> ParseFields(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return [];

        try
        {
            return JsonSerializer.Deserialize<List<SaveEventPageFormFieldItemDto>>(json, Options) ?? [];
        }
        catch (JsonException ex)
        {
            throw new ArgumentException("صيغة حقول النموذج غير صحيحة", ex);
        }
    }

    public static List<string> ParseOptions(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return [];

        try
        {
            return JsonSerializer.Deserialize<List<string>>(json, Options) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    public static string? SerializeOptions(IEnumerable<string>? options)
    {
        var cleaned = (options ?? [])
            .Select(o => o.Trim())
            .Where(o => o.Length > 0)
            .ToList();

        return cleaned.Count == 0 ? null : JsonSerializer.Serialize(cleaned);
    }
}
