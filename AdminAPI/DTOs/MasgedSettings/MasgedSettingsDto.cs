namespace AdminAPI.DTOs.MasgedSettings;

public class MasgedSettingsDto
{
    public int Id { get; set; }
    public string MasgedName { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? ParentAppStoreUrl { get; set; }
    public string? ParentGooglePlayUrl { get; set; }
    public string? TeacherAppStoreUrl { get; set; }
    public string? TeacherGooglePlayUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? Domain { get; set; }
    public bool SetupCompleted { get; set; }
}
