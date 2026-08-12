namespace AdminAPI.DTOs.Teachers;

public class TeacherDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Mobile { get; set; }
    public string Email { get; set; } = string.Empty;
    public decimal? BaseSalary { get; set; }
    public bool UsersManage { get; set; }
    public bool IsGirlTeacher { get; set; }
    public bool IsViewOnly { get; set; }
    public bool IsSupervisor { get; set; }
    public string? ImageUrl { get; set; }
    public List<int> SelectedMosqueIds { get; set; } = [];
    public List<TeacherMapLocationDto> ManualLocations { get; set; } = [];
}
