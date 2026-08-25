using System.Globalization;
using AdminAPI.DTOs.Tests;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;

namespace AdminAPI.Services;

public class TestsReportService(
    ITestsReportRepository repository,
    ICurrentUserContext currentUser) : ITestsReportService
{
    private const string ProgramType = "حلقات تحفيظ القرآن الكريم";
    private const string GridTestType = TestRangeResolver.DefaultTestType;
    private static readonly CultureInfo DisplayCulture = CultureInfo.InvariantCulture;

    public async Task<TestsReportFilterOptionsDto> GetFilterOptionsAsync(
        CancellationToken cancellationToken = default)
    {
        var circles = await repository.GetCirclesAsync(currentUser.IsGirlTeacher, cancellationToken);
        return new TestsReportFilterOptionsDto { Circles = circles };
    }

    public async Task<TestsReportListResponseDto> GetReportAsync(
        string fromDate,
        string toDate,
        int? circleId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (from, toExclusive) = ParseDateRange(fromDate, toDate);
        var normalizedCircleId = circleId is > 0 ? circleId : null;
        var rows = await repository.GetReportRowsAsync(
            from, toExclusive, normalizedCircleId, cancellationToken);

        var page = pageNumber < 1 ? 1 : pageNumber;
        var size = pageSize < 1 ? 20 : pageSize;
        var totalCount = rows.Count;
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)size);
        var items = rows
            .Skip((page - 1) * size)
            .Take(size)
            .Select(MapGridRow)
            .ToList();

        return new TestsReportListResponseDto
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = size,
            TotalPages = totalPages,
        };
    }

    public async Task<byte[]> ExportReportExcelAsync(
        string fromDate,
        string toDate,
        int? circleId,
        CancellationToken cancellationToken = default)
    {
        var (from, toExclusive) = ParseDateRange(fromDate, toDate);
        var normalizedCircleId = circleId is > 0 ? circleId : null;
        var rows = await repository.GetReportRowsAsync(
            from, toExclusive, normalizedCircleId, cancellationToken);

        return TestsReportExcelExporter.Build(rows, from, toExclusive.AddDays(-1));
    }

    private static TestsReportRowDto MapGridRow(TestsReportSourceRow row) =>
        new()
        {
            StudentId = row.StudentId,
            StudentName = row.StudentName,
            ParentPhone = row.ParentPhone,
            TeacherName = row.TeacherName,
            CircleName = row.CircleName,
            ProgramType = ProgramType,
            TestFrom = row.TestFrom,
            TestTo = row.TestTo,
            TestDate = row.TestDate.ToString("dd/MM/yyyy", DisplayCulture),
            FinalResults = row.FinalResults,
            Notes = row.Notes,
            TestType = GridTestType,
        };

    private static (DateTime From, DateTime ToExclusive) ParseDateRange(string fromDate, string toDate)
    {
        if (string.IsNullOrWhiteSpace(fromDate) || string.IsNullOrWhiteSpace(toDate))
            throw new InvalidOperationException("يرجى اختيار تاريخ البداية والنهاية");

        if (!DateTime.TryParse(fromDate, out var from) || !DateTime.TryParse(toDate, out var to))
            throw new InvalidOperationException("يرجى إدخال تاريخ صحيح");

        if (from.Date > to.Date)
            throw new InvalidOperationException("تاريخ البداية يجب أن يكون قبل تاريخ النهاية");

        return (from.Date, to.Date.AddDays(1));
    }
}
