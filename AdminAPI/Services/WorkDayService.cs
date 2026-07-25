using AdminAPI.Data;
using AdminAPI.DTOs.WorkDays;
using AdminAPI.Models;
using AdminAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AdminAPI.Services;

public class WorkDayService(
    AdminDbContext db,
    ICurrentUserContext currentUser,
    IMemoryCache cache) : IWorkDayService
{
    public const string SettingKey = "MasgedWorkDayNumbers";
    private const string CacheKey = "MasgedWorkDayNumbers";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private static readonly int[] DefaultDayNumbers = [6, 0, 1, 2, 3];

    private static readonly (int Number, string NameAr)[] DisplayOrder =
    [
        (6, "السبت"),
        (0, "الأحد"),
        (1, "الاثنين"),
        (2, "الثلاثاء"),
        (3, "الأربعاء"),
        (4, "الخميس"),
        (5, "الجمعة"),
    ];

    public async Task<WorkDaysDto> GetAsync(CancellationToken cancellationToken = default)
    {
        var numbers = await GetWorkDayNumbersAsync(cancellationToken);
        return BuildDto(numbers);
    }

    public async Task<WorkDaysDto> UpdateAsync(
        UpdateWorkDaysRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAdmin || currentUser.IsGirlTeacher)
            throw new UnauthorizedAccessException("ليس لديك صلاحية لتعديل أيام العمل");

        var normalized = request.DayNumbers
            .Distinct()
            .OrderBy(n => n)
            .ToList();

        var setting = await db.AppSettings
            .FirstOrDefaultAsync(x => x.Key == SettingKey && !x.ForGirl, cancellationToken);

        var value = string.Join(",", normalized);

        if (setting is null)
        {
            setting = new AppSetting
            {
                Key = SettingKey,
                Value = value,
                Description = "Masged-wide work day numbers (.NET DayOfWeek)",
                CreatedAt = KuwaitTime.Now,
                UpdatedAt = KuwaitTime.Now,
                ForGirl = false,
            };
            db.AppSettings.Add(setting);
        }
        else
        {
            setting.Value = value;
            setting.UpdatedAt = KuwaitTime.Now;
        }

        await db.SaveChangesAsync(cancellationToken);
        cache.Remove(CacheKey);

        return BuildDto(normalized);
    }

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

    private static WorkDaysDto BuildDto(IReadOnlyList<int> enabledNumbers)
    {
        var enabledSet = enabledNumbers.ToHashSet();
        return new WorkDaysDto
        {
            DayNumbers = enabledNumbers.OrderBy(n => n).ToList(),
            DayLabels = DisplayOrder
                .Select(d => new WorkDayLabelDto
                {
                    Number = d.Number,
                    NameAr = d.NameAr,
                    Enabled = enabledSet.Contains(d.Number),
                })
                .ToList(),
        };
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
