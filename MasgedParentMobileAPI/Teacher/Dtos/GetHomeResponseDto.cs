namespace MasgedTeacherMobileAPI.Dtos;

public class GetHomeResponseDto
{
    public string TeacherName { get; set; } = string.Empty;
    public string CircleName { get; set; } = string.Empty;
    public bool IsWorkDayToday { get; set; }
    public StudentsStatisticsDto Statistics { get; set; } = new();
    public List<IdNameDto> PlanLevels { get; set; } = [];
    public int UnreadAdminNotesCount { get; set; }
    public List<StudentDto> Students { get; set; } = [];
}
