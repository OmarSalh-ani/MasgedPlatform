namespace AdminAPI.DTOs.SocialLink;

public class SocialLinkListItemDto
{
    public int Id { get; set; }

    public string PlatformName { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
}
