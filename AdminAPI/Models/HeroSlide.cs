namespace AdminAPI.Models;

public class HeroSlide
{
    public int Id { get; set; }

    public string? ImageUrl { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
}
