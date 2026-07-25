namespace AdminAPI.DTOs.Integrations;

public class UpdateIntegrationSettingsRequestDto
{
    /// <summary>Null = leave unchanged; empty = clear DB override (fall back to env).</summary>
    public string? WasenderApiToken { get; set; }
    public string? WasenderSessionApiKey { get; set; }
    public string? AgoraAppId { get; set; }
    public string? AgoraAppCertificate { get; set; }
}
