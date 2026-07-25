using AdminAPI.DTOs.Tests;

namespace AdminAPI.Services.Interfaces;

public interface ITestsReportService
{
    Task<TestsReportFilterOptionsDto> GetFilterOptionsAsync(
        CancellationToken cancellationToken = default);

    Task<TestsReportListResponseDto> GetReportAsync(
        string fromDate,
        string toDate,
        int? circleId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<byte[]> ExportReportExcelAsync(
        string fromDate,
        string toDate,
        int? circleId,
        CancellationToken cancellationToken = default);
}
