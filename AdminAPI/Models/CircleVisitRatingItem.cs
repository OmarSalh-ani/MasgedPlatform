namespace AdminAPI.Models;

public class CircleVisitRatingItem
{
    public int Id { get; set; }
    public int CircleVisitRatingId { get; set; }
    public int Sequence { get; set; }
    public string Criterion { get; set; } = string.Empty;
    public string Rating { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public virtual CircleVisitRating? CircleVisitRating { get; set; }
}
