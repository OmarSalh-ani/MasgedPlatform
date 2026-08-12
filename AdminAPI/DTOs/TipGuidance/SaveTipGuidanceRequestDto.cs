namespace AdminAPI.DTOs.TipGuidance;

public class SaveTipGuidanceRequestDto
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? LinkUrl { get; set; }

    public int SortOrder { get; set; }

    public IFormFile? Image { get; set; }
}
