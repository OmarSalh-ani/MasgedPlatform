namespace AdminAPI.Models;

public class QuranCircle
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public int? TeacherId { get; set; }
    public bool? ForGirls { get; set; }

    public virtual Teacher? Teacher { get; set; }
    public virtual ICollection<CircleDay> CircleDays { get; set; } = new List<CircleDay>();
}
