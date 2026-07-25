using AdminAPI.DTOs.TeachersAttendance;

namespace AdminAPI.Services.Interfaces;

public interface ITeachersAttendanceService
{
    Task<TeachersAttendanceFilterOptionsDto> GetFilterOptionsAsync(
        CancellationToken cancellationToken = default);

    Task<TeachersAttendanceListResponseDto> GetListAsync(
        string? fromDate,
        string? toDate,
        int? teacherId,
        CancellationToken cancellationToken = default);

    Task<byte[]> ExportExcelAsync(
        string? fromDate,
        string? toDate,
        int? teacherId,
        CancellationToken cancellationToken = default);
}
