namespace MasgedParentMobileAPI.DTOs;

public class NewsItemDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime NewsDate { get; set; }
}
