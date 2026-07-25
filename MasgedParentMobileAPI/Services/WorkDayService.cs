using MasgedParentMobileAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MasgedParentMobileAPI.Services;

public class WorkDayService(NewMasgedTeacherAPIDBContext db, IMemoryCache cache) : IWorkDayService
{
    public const string SettingKey = "MasgedWorkDayNumbers";
    private const string CacheKey = "MasgedWorkDayNumbers";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private static readonly int[] DefaultDayNumbers = [6, 0, 1, 2, 3];

    public async Task<IReadOnlyList<int>> GetWorkDayNumbersAsync(CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(CacheKey, out int[]? cached) && cached is { Length: > 0 })
            return cached;

        var setting = await db.AppSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Key == SettingKey && !x.ForGirl, cancellationToken);

        var numbers = ParseDayNumbers(setting?.Value);
        cache.Set(CacheKey, numbers.ToArray(), CacheDuration);
        return numbers;
    }

    public async Task<bool> IsWorkDayAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var numbers = await GetWorkDayNumbersAsync(cancellationToken);
        return numbers.Contains((int)date.DayOfWeek);
    }

    public async Task<int> CountWorkDaysInRangeAsync(
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken = default)
    {
        var numbers = await GetWorkDayNumbersAsync(cancellationToken);
        var count = 0;
        for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
        {
            if (numbers.Contains((int)d.DayOfWeek))
                count++;
        }

        return count;
    }

    public async Task<DateTime> GetNextWorkDayAsync(DateTime from, CancellationToken cancellationToken = default)
    {
        var numbers = await GetWorkDayNumbersAsync(cancellationToken);
        var date = from.Date;
        for (var i = 0; i < 14; i++)
        {
            if (numbers.Contains((int)date.DayOfWeek))
                return date;
            date = date.AddDays(1);
        }

        return from.Date;
    }

    private static List<int> ParseDayNumbers(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return DefaultDayNumbers.ToList();

        var parsed = value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s => int.TryParse(s, out var n) ? n : (int?)null)
            .Where(n => n is >= 0 and <= 6)
            .Select(n => n!.Value)
            .Distinct()
            .OrderBy(n => n)
            .ToList();

        return parsed.Count > 0 ? parsed : DefaultDayNumbers.ToList();
    }
}
