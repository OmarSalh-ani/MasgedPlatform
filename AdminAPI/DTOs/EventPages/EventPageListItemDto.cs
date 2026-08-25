namespace AdminAPI.DTOs.EventPages;

public class EventPageListItemDto
{
    public int Id { get; set; }
    public string ActivityName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsPublished { get; set; }
    public bool IsRegistrationOpen { get; set; }
    public DateTime CreatedAt { get; set; }
}
