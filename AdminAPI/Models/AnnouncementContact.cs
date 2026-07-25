namespace AdminAPI.Models;

public class AnnouncementContact
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
