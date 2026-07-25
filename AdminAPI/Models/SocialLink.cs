namespace AdminAPI.Models;

public class SocialLink
{
    public int Id { get; set; }

    public string PlatformName { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string? IconClass { get; set; }

    public int SortOrder { get; set; }
}
