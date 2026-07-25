namespace AdminAPI.DTOs.Activity;

public class SaveActivityRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public IFormFile? Image { get; set; }
}
