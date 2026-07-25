namespace AdminAPI.Models;

public class AnnouncementMessage
{
    public int Id { get; set; }
    public int? ContactId { get; set; }
    public string Mobile { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Image { get; set; }
    public DateTime SentAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsProcessed { get; set; }
}
