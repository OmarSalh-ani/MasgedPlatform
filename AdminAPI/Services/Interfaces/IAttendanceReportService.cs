using AdminAPI.DTOs.AttendanceReport;

namespace AdminAPI.Services.Interfaces;

public interface IAttendanceReportService
{
    Task<AttendanceReportFilterOptionsDto> GetFilterOptionsAsync(
        CancellationToken cancellationToken = default);

    Task<AttendanceReportListResponseDto> GetReportAsync(
        DateTime fromDate,
        DateTime toDate,
        int? circleId,
        int? teacherId,
        string attendanceFilter,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<byte[]> ExportReportExcelAsync(
        DateTime fromDate,
        DateTime toDate,
        int? circleId,
        int? teacherId,
        string attendanceFilter,
        CancellationToken cancellationToken = default);

    Task<string> SendWhatsappAsync(
        SendAttendanceWhatsappRequestDto request,
        string? base64Image,
        CancellationToken cancellationToken = default);

    Task<SaveDepartureResultDto> SaveDeparturesAsync(
        IReadOnlyList<SaveDepartureItemDto> items,
        CancellationToken cancellationToken = default);
}
