using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.TeacherSalaries;

namespace AdminAPI.Services.Interfaces;

public interface ITeacherSalaryService
{
    Task<PagedResultDto<TeacherSalaryListItemDto>> GetListAsync(
        int? month,
        int? year,
        int? teacherId,
        CancellationToken cancellationToken = default);

    Task<TeacherSalaryFilterOptionsDto> GetFilterOptionsAsync(
        CancellationToken cancellationToken = default);

    Task<List<TeacherSalaryFormTeacherDto>> GetFormTeachersAsync(
        CancellationToken cancellationToken = default);

    Task<TeacherSalaryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<TeacherSalaryDto> CreateAsync(
        SaveTeacherSalaryRequestDto request,
        CancellationToken cancellationToken = default);

    Task<TeacherSalaryDto> UpdateAsync(
        int id,
        SaveTeacherSalaryRequestDto request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<AttendanceCalculationResultDto> CalculateAttendanceAsync(
        CalculateTeacherAttendanceRequestDto request,
        CancellationToken cancellationToken = default);

    Task<SalaryCalculationResultDto> CalculateSalaryAsync(
        CalculateTeacherSalaryRequestDto request,
        CancellationToken cancellationToken = default);

    Task<AutoCalculateMonthResultDto> AutoCalculateAllForMonthAsync(
        AutoCalculateMonthRequestDto request,
        CancellationToken cancellationToken = default);

    Task<PaySelectedSalariesResultDto> PaySelectedSalariesAsync(
        PaySelectedSalariesRequestDto request,
        CancellationToken cancellationToken = default);

    Task<TeacherSalaryReportDto> GetReportAsync(
        int month,
        int year,
        CancellationToken cancellationToken = default);

    Task<byte[]> ExportReportExcelAsync(int month, int year, CancellationToken cancellationToken = default);
}
