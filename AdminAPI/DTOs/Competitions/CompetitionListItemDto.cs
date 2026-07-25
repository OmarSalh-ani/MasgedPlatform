namespace AdminAPI.DTOs.Competitions;

public class CompetitionListItemDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
}
