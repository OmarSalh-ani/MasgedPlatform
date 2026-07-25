namespace AdminAPI.Models;

public class CircleDay
{
    public int Id { get; set; }
    public int CircleId { get; set; }
    public int DayNumber { get; set; }

    public virtual QuranCircle Circle { get; set; } = null!;
}
