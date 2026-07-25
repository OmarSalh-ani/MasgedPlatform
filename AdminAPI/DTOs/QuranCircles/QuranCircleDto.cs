namespace AdminAPI.DTOs.QuranCircles;

public class QuranCircleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? TeacherId { get; set; }
    public bool ForGirls { get; set; }
}
