namespace AdminAPI.DTOs.PublicEventPages;

public class PublicEventPageTrackDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
}
