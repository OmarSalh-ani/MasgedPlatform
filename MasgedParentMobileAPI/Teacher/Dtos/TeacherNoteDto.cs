namespace MasgedTeacherMobileAPI.Dtos;

public class TeacherNoteDto
{
    public int Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string CreatedDate { get; set; } = string.Empty;
    public bool IsWarning { get; set; }
    public bool IsRead { get; set; }
}

public class IndexDashboardDto
{
    public string TeacherName { get; set; } = string.Empty;
    public StudentsStatisticsDto Statistics { get; set; } = new();
    public List<IdNameDto> PlanLevels { get; set; } = [];
    public int UnreadAdminNotesCount { get; set; }
}
