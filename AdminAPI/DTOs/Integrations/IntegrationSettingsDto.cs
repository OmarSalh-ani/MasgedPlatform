namespace AdminAPI.DTOs.Integrations;

public class IntegrationSettingsDto
{
    public bool WasenderApiTokenConfigured { get; set; }
    public string? WasenderApiTokenHint { get; set; }
    public bool WasenderSessionApiKeyConfigured { get; set; }
    public string? WasenderSessionApiKeyHint { get; set; }
    public bool AgoraAppIdConfigured { get; set; }
    public string? AgoraAppIdHint { get; set; }
    public bool AgoraAppCertificateConfigured { get; set; }
    public string? AgoraAppCertificateHint { get; set; }
}
