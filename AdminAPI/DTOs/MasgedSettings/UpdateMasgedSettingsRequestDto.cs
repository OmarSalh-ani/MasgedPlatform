namespace AdminAPI.DTOs.MasgedSettings;

public class UpdateMasgedSettingsRequestDto
{
    public string MasgedName { get; set; } = string.Empty;
    public IFormFile? LogoFile { get; set; }
    public bool RemoveLogo { get; set; }
    public string? ParentAppStoreUrl { get; set; }
    public string? ParentGooglePlayUrl { get; set; }
    public string? TeacherAppStoreUrl { get; set; }
    public string? TeacherGooglePlayUrl { get; set; }
    public string? PrimaryColor { get; set; }
}
