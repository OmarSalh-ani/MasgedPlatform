namespace AdminAPI.DTOs.PublicIndex;

public class PublicCompetitionItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? LinkUrl { get; set; }
}

public class PublicMosqueItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? GoogleMapsUrl { get; set; }
    public string? ImageUrl { get; set; }
}

public class PublicNewsItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime NewsDate { get; set; }
    public string? ImageUrl { get; set; }
    public string? LinkUrl { get; set; }
}

public class PublicActivityItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? IconClass { get; set; }
}

public class PublicHeroSlideItemDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

public class PublicSocialLinkItemDto
{
    public int Id { get; set; }
    public string PlatformName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? IconClass { get; set; }
    public string ResolvedIconClass { get; set; } = "fas fa-link";
}

public class PublicAboutDto
{
    public string Content { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? MapsUrl { get; set; }
}

public class PublicWebsiteContentDto
{
    public List<PublicHeroSlideItemDto> HeroSlides { get; set; } = [];
    public List<PublicCompetitionItemDto> Competitions { get; set; } = [];
    public List<PublicMosqueItemDto> Mosques { get; set; } = [];
    public List<PublicNewsItemDto> News { get; set; } = [];
    public List<PublicActivityItemDto> Activities { get; set; } = [];
    public PublicAboutDto About { get; set; } = new();
    public List<PublicSocialLinkItemDto> SocialLinks { get; set; } = [];
}
