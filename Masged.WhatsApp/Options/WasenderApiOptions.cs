namespace Masged.WhatsApp.Options;

public class WasenderApiOptions
{
    public const string SectionName = "Wasender";

    public string BaseUrl { get; set; } = "https://www.wasenderapi.com/api";
    public string? ApiToken { get; set; }
    public string? SessionApiKey { get; set; }
    public string ErrorLogDirectory { get; set; } = "Logs";
}
