using AdminAPI.DTOs.TeachersAttendance;

namespace AdminAPI.Repositories.Interfaces;

public interface ITeachersAttendanceRepository
{
    Task<List<TeachersAttendanceTeacherOptionDto>> GetTeachersAsync(
        bool isGirlTeacher,
        CancellationToken cancellationToken = default);

    Task<List<TeachersAttendanceSourceRow>> GetAttendanceRowsAsync(
        bool isGirlTeacher,
        DateTime fromDate,
        DateTime toDateInclusive,
        int? teacherId,
        CancellationToken cancellationToken = default);
}

public class TeachersAttendanceSourceRow
{
    public string TeacherName { get; set; } = string.Empty;
    public DateTime AttendanceDateTime { get; set; }
    public DateTime? DepartureDateTime { get; set; }
}
