namespace AdminAPI.DTOs.Activities;

public class ActivityListItemDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public string? IconClass { get; set; }
}
