namespace AdminAPI.Models;

public class CircleVisitRating
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public int QuranCircleId { get; set; }
    public DateTime VisitDate { get; set; }
    public TimeSpan VisitTime { get; set; }
    public int VisitNumberInMonth { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual Teacher? Teacher { get; set; }
    public virtual QuranCircle? QuranCircle { get; set; }
    public virtual ICollection<CircleVisitRatingItem> Items { get; set; } = new List<CircleVisitRatingItem>();
}
