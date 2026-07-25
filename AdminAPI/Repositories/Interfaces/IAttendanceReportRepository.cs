using AdminAPI.DTOs.AttendanceReport;
using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface IAttendanceReportRepository
{
    Task<AttendanceReportFilterOptionsDto> GetFilterOptionsAsync(
        bool isGirlTeacher,
        CancellationToken cancellationToken = default);

    IQueryable<RegisterForm> BuildStudentQuery(
        bool isGirlTeacher,
        int? circleId,
        int? teacherId);
}
