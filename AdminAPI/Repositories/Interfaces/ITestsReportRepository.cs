using AdminAPI.DTOs.Tests;

namespace AdminAPI.Repositories.Interfaces;

public interface ITestsReportRepository
{
    Task<List<TestsReportCircleOptionDto>> GetCirclesAsync(
        bool isGirlTeacher,
        CancellationToken cancellationToken = default);

    Task<List<TestsReportSourceRow>> GetReportRowsAsync(
        DateTime fromDate,
        DateTime toDateExclusive,
        int? circleId,
        CancellationToken cancellationToken = default);
}
