using AdminAPI.DTOs.CircleMemorizationRevisionReport;

namespace AdminAPI.Services.Interfaces;

public interface ICircleMemorizationRevisionReportService
{
    Task<List<CircleMemorizationTeacherOptionDto>> GetTeachersAsync(
        CancellationToken cancellationToken = default);

    Task<(byte[] Bytes, string FileName, string ContentType)?> ExportAsync(
        int teacherId,
        DateTime fromDate,
        DateTime toDate,
        string format,
        CancellationToken cancellationToken = default);
}
