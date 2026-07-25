namespace AdminAPI.Services;

public class PublicRegistrationOptions
{
    public const string SectionName = "Registration";
    public bool Enabled { get; set; }
    public string AdminNotificationMobile { get; set; } = "+96566739470";
    public string FallbackWhatsappUrl { get; set; } = "https://wa.me/96566739470";
}
