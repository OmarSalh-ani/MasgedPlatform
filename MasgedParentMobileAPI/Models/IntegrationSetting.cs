namespace MasgedParentMobileAPI.Models;

public class IntegrationSetting
{
    public int Id { get; set; }
    public string? WasenderApiToken { get; set; }
    public string? WasenderSessionApiKey { get; set; }
    public string? AgoraAppId { get; set; }
    public string? AgoraAppCertificate { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
