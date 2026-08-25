namespace AdminAPI.Models;

public class EventPageTrack
{
    public int Id { get; set; }
    public int EventPageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }

    public EventPage? EventPage { get; set; }
}
