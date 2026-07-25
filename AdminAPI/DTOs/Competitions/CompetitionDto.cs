namespace AdminAPI.DTOs.Competitions;

public class CompetitionDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? LinkUrl { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
}
