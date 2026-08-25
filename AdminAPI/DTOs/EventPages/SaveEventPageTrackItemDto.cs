namespace AdminAPI.DTOs.EventPages;

public class SaveEventPageTrackItemDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
}
