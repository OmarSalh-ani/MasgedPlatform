using System.Text.Json.Serialization;

namespace AdminAPI.DTOs.CountryDialCodes;

public class CountryDialEntryDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("dial_code")]
    public string DialCode { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string IsoCode { get; set; } = string.Empty;
}
