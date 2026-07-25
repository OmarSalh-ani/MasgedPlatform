namespace AdminAPI.DTOs.HeroSlides;

public class SaveHeroSlideRequestDto
{
    public int SortOrder { get; set; }

    public List<IFormFile> Images { get; set; } = [];
}
