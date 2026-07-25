namespace AdminAPI.DTOs.SocialLink;

public class SocialLinkDto
{
    public int Id { get; set; }

    public string PlatformName { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string? IconClass { get; set; }

    public int SortOrder { get; set; }
}
