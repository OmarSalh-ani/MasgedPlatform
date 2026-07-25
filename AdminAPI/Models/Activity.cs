namespace AdminAPI.Models;

public class Activity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ImageUrl { get; set; }
}
