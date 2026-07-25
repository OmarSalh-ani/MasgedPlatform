using System.Globalization;
using AdminAPI.DTOs.ParentPanelLogStatistics;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;

namespace AdminAPI.Services;

public class ParentPanelLogStatisticsService(
    IParentPanelLogStatisticsRepository repository) : IParentPanelLogStatisticsService
{
    public async Task<ParentPanelLogStatisticsResponseDto> GetStatisticsAsync(
        string? fromDate,
        string? toDate,
        CancellationToken cancellationToken = default)
    {
        var from = ResolveFromDate(fromDate);
        var toInclusive = ResolveToDateInclusive(toDate);

        try
        {
            var logEntries = await repository.GetLogEntriesAsync(from, toInclusive, cancellationToken);
            var allParentMobiles = await repository.GetAllParentMobilesAsync(cancellationToken);

            var parentsOpenedMobiles = logEntries
                .Select(x => x.ParentMobile)
                .Distinct()
                .ToList();

            var uniqueParentsOpened = parentsOpenedMobiles.Count;
            var uniqueParentsNotOpened = allParentMobiles
                .Count(x => !parentsOpenedMobiles.Contains(x));
            var totalUniqueParents = allParentMobiles.Count;
            var totalLogEntries = logEntries.Count;
            var percentage = totalUniqueParents > 0
                ? (double)uniqueParentsOpened / totalUniqueParents * 100
                : 0;

            var entries = logEntries
                .Select(x => new
                {
                    x.ParentMobile,
                    StudentName = x.RegisterForm != null ? x.RegisterForm.StudentName : "غير محدد",
                    x.StudentId,
                    AccessDate = x.AccessDateTime.Date,
                    x.AccessDateTime,
                })
                .GroupBy(x => new { x.ParentMobile, x.AccessDate })
                .Select(g => g.OrderByDescending(x => x.AccessDateTime).First())
                .OrderByDescending(x => x.AccessDate)
                .ThenByDescending(x => x.AccessDateTime)
                .Select(x => new ParentPanelLogEntryDto
                {
                    ParentMobile = x.ParentMobile,
                    StudentName = x.StudentName,
                    StudentId = x.StudentId,
                    AccessDate = x.AccessDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    AccessTime = x.AccessDateTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
                })
                .ToList();

            return new ParentPanelLogStatisticsResponseDto
            {
                FromDate = from.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                ToDate = toInclusive.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Summary = new ParentPanelLogStatisticsSummaryDto
                {
                    ParentsOpened = uniqueParentsOpened,
                    ParentsNotOpened = uniqueParentsNotOpened,
                    TotalLogEntries = totalLogEntries,
                    Percentage = percentage.ToString("F1", CultureInfo.InvariantCulture) + "%",
                },
                Entries = entries,
            };
        }
        catch
        {
            return EmptyResponse(from, toInclusive);
        }
    }

    private static DateTime ResolveFromDate(string? fromDate)
    {
        if (DateTime.TryParse(fromDate, out var parsed))
            return parsed;

        return KuwaitTime.Now.AddDays(-30);
    }

    private static DateTime ResolveToDateInclusive(string? toDate)
    {
        DateTime parsed;
        if (!DateTime.TryParse(toDate, out parsed))
            parsed = KuwaitTime.Now;

        return parsed.Date.AddDays(1).AddSeconds(-1);
    }

    private static ParentPanelLogStatisticsResponseDto EmptyResponse(DateTime from, DateTime toInclusive) =>
        new()
        {
            FromDate = from.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            ToDate = toInclusive.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            Summary = new ParentPanelLogStatisticsSummaryDto
            {
                ParentsOpened = 0,
                ParentsNotOpened = 0,
                TotalLogEntries = 0,
                Percentage = "0%",
            },
            Entries = [],
        };
}
