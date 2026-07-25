namespace AdminAPI.DTOs.QuranCircles;

public class QuranCircleListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StudentsCount { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public int? TeacherId { get; set; }
    public bool ForGirls { get; set; }
}
