namespace AdminAPI.Models;

public class TeacherMapLocation
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public string MapURL { get; set; } = string.Empty;
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public Teacher? Teacher { get; set; }
}
