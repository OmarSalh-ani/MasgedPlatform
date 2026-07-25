namespace AdminAPI.DTOs.News;

public class SaveNewsRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime NewsDate { get; set; }
    public int SortOrder { get; set; }
    public IFormFile? Image { get; set; }
}
