namespace AdminAPI.DTOs.MasgedSettings;

public class FirstTimeSetupRequestDto
{
    public string MasgedName { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = "#2563eb";
    public string Domain { get; set; } = string.Empty;
    public IFormFile? LogoFile { get; set; }
    public string? ParentAppStoreUrl { get; set; }
    public string? ParentGooglePlayUrl { get; set; }
    public string? TeacherAppStoreUrl { get; set; }
    public string? TeacherGooglePlayUrl { get; set; }

    public string AdminName { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string AdminPassword { get; set; } = string.Empty;
}
