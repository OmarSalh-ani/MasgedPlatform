namespace MasgedTeacherMobileAPI.Dtos;

public class StudentsListResponseDto
{
    public PagedResultDto<StudentDto> Students { get; set; } = new();
    public StudentsStatisticsDto Statistics { get; set; } = new();
}
