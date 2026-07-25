using AdminAPI.DTOs.MemorizationRevisionReport;

namespace AdminAPI.Services.Interfaces;

public interface IMemorizationRevisionReportService
{
    Task<bool> StudentExistsAsync(int studentId, CancellationToken cancellationToken = default);

    Task<List<MemorizationRevisionStudentPickDto>> GetStudentsAsync(CancellationToken cancellationToken = default);

    Task<MemorizationRevisionReportResponseDto?> GetReportAsync(
        int studentId,
        CancellationToken cancellationToken = default);

    Task<(byte[] Bytes, string FileName)?> ExportFullReportAsync(
        int studentId,
        CancellationToken cancellationToken = default);

    Task<(byte[] Bytes, string FileName)?> ExportCompletedSurahsAsync(
        int studentId,
        CancellationToken cancellationToken = default);
}
