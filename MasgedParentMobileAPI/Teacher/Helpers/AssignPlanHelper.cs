using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Entities;
using MasgedTeacherMobileAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Helpers;

public static class AssignPlanHelper
{
    public static async Task<(bool Success, string? Error)> AssignPlanAsync(
        AppDbContext db,
        List<int> studentIds,
        int planLevelId,
        int fromSurahId,
        int toSurahId,
        int? fromJozz,
        int? toJozz,
        string fromDate,
        string toDate,
        string planType,
        int? fromAyahNumber,
        int? toAyahNumber,
        int circleId,
        IReadOnlyList<int> workDayNumbers,
        CancellationToken cancellationToken = default)
    {
        if (studentIds.Count == 0)
            return (false, "لا يوجد طلاب");

        var level = await db.PlanLevels.FirstOrDefaultAsync(l => l.Id == planLevelId, cancellationToken);
        if (level is null)
            return (false, "مستوى الخطة غير موجود");

        var startDate = DateTime.TryParse(fromDate, out var fd) ? fd.Date : KuwaitTime.Today;
        var endDate = DateTime.TryParse(toDate, out var td) ? td.Date : KuwaitTime.Today;
        if (endDate < startDate)
            endDate = startDate;

        var unit = (PlanUnitType)level.UnitType;
        var dailyQty = Math.Max(1, level.Quantity);

        List<HolyQuran> allQuranData;

        if (unit == PlanUnitType.Jozz && fromJozz.HasValue && toJozz.HasValue)
        {
            var minJ = Math.Min(fromJozz.Value, toJozz.Value);
            var maxJ = Math.Max(fromJozz.Value, toJozz.Value);
            allQuranData = await db.HolyQurans
                .AsNoTracking()
                .Where(h => h.jozz >= minJ && h.jozz <= maxJ)
                .OrderBy(h => h.jozz).ThenBy(h => h.page).ThenBy(h => h.aya_no)
                .ToListAsync(cancellationToken);
        }
        else
        {
            var allSurahs = await db.QuranSurahs
                .AsNoTracking()
                .OrderBy(x => x.SortOrder ?? x.Id)
                .ToListAsync(cancellationToken);

            var fromS = allSurahs.FirstOrDefault(s => s.Id == fromSurahId);
            var toS = allSurahs.FirstOrDefault(s => s.Id == toSurahId);
            if (fromS is null || toS is null)
                return (false, "سورة غير صالحة");

            var minOrder = Math.Min(fromS.SortOrder ?? fromS.Id, toS.SortOrder ?? toS.Id);
            var maxOrder = Math.Max(fromS.SortOrder ?? fromS.Id, toS.SortOrder ?? toS.Id);
            var targetSurahIds = allSurahs
                .Where(s => (s.SortOrder ?? s.Id) >= minOrder && (s.SortOrder ?? s.Id) <= maxOrder)
                .Select(s => s.Id)
                .ToList();

            allQuranData = await db.HolyQurans
                .AsNoTracking()
                .Where(h => targetSurahIds.Contains(h.sura_no))
                .OrderBy(h => h.sura_no).ThenBy(h => h.aya_no)
                .ToListAsync(cancellationToken);

            if (fromAyahNumber.HasValue || toAyahNumber.HasValue)
            {
                allQuranData = allQuranData.Where(h =>
                {
                    if (h.sura_no == fromSurahId && fromAyahNumber.HasValue && h.aya_no < fromAyahNumber.Value)
                        return false;
                    if (h.sura_no == toSurahId && toAyahNumber.HasValue && h.aya_no > toAyahNumber.Value)
                        return false;
                    return true;
                }).ToList();
            }
        }

        if (allQuranData.Count == 0)
            return (false, "لا توجد بيانات للقرآن في النطاق المحدد");

        var rowsWithDates = BuildRowsWithDates(allQuranData, unit, dailyQty, startDate, endDate, workDayNumbers);
        if (rowsWithDates.Count == 0)
            return (false, "لا توجد بيانات للقرآن في النطاق المحدد");

        var wantMem = planType == "حفظ";
        var wantRev = planType == "مراجعة";

        foreach (var sid in studentIds)
        {
            var plan = new StudentPlan
            {
                StudentId = sid,
                Name = (level.LevelName ?? "خطة") + " " + KuwaitTime.Today.ToString("yyyy-MM-dd"),
                PlanFromDate = startDate,
                PlanToDate = endDate,
                CreatedAt = KuwaitTime.Now
            };
            db.StudentPlans.Add(plan);
            await db.SaveChangesAsync(cancellationToken);

            foreach (var row in rowsWithDates)
            {
                if (wantMem)
                {
                    db.StudentPlanMemorizings.Add(new StudentPlanMemorizing
                    {
                        StudentId = sid,
                        PlanId = plan.Id,
                        MemorizationLevel = level.LevelName,
                        SurahId = row.SurahId,
                        FromAyahNumber = row.FromAyah,
                        ToAyahNumber = row.ToAyah,
                        PlanDate = row.PlanDate,
                        PlanEndDate = row.PlanDate,
                        CreatedAt = KuwaitTime.Now
                    });
                }

                if (wantRev)
                {
                    db.StudentPlanRevises.Add(new StudentPlanRevise
                    {
                        StudentId = sid,
                        PlanId = plan.Id,
                        MemorizationLevel = level.LevelName,
                        SurahId = row.SurahId,
                        FromAyahNumber = row.FromAyah,
                        ToAyahNumber = row.ToAyah,
                        PlanDate = row.PlanDate,
                        PlanEndDate = row.PlanDate,
                        CreatedAt = KuwaitTime.Now
                    });
                }
            }
        }

        await db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public static async Task<(bool Success, string? Error)> AssignReviseToExistingPlanAsync(
        AppDbContext db,
        int studentId,
        int planId,
        int planLevelId,
        int fromSurahId,
        int toSurahId,
        int? fromJozz,
        int? toJozz,
        string fromDate,
        string toDate,
        int? fromAyahNumber,
        int? toAyahNumber,
        int circleId,
        IReadOnlyList<int> workDayNumbers,
        CancellationToken cancellationToken = default)
    {
        var plan = await db.StudentPlans
            .FirstOrDefaultAsync(p => p.Id == planId && p.StudentId == studentId, cancellationToken);
        if (plan is null)
            return (false, "الخطة غير موجودة");

        var level = await db.PlanLevels.FirstOrDefaultAsync(l => l.Id == planLevelId, cancellationToken);
        if (level is null)
            return (false, "مستوى الخطة غير موجود");

        var startDate = DateTime.TryParse(fromDate, out var fd) ? fd.Date : KuwaitTime.Today;
        var endDate = DateTime.TryParse(toDate, out var td) ? td.Date : KuwaitTime.Today;
        if (endDate < startDate)
            endDate = startDate;

        var unit = (PlanUnitType)level.UnitType;
        var dailyQty = Math.Max(1, level.Quantity);

        List<HolyQuran> allQuranData;

        if (unit == PlanUnitType.Jozz && fromJozz.HasValue && toJozz.HasValue)
        {
            var minJ = Math.Min(fromJozz.Value, toJozz.Value);
            var maxJ = Math.Max(fromJozz.Value, toJozz.Value);
            allQuranData = await db.HolyQurans
                .AsNoTracking()
                .Where(h => h.jozz >= minJ && h.jozz <= maxJ)
                .OrderBy(h => h.jozz).ThenBy(h => h.page).ThenBy(h => h.aya_no)
                .ToListAsync(cancellationToken);
        }
        else
        {
            var allSurahs = await db.QuranSurahs
                .AsNoTracking()
                .OrderBy(x => x.SortOrder ?? x.Id)
                .ToListAsync(cancellationToken);

            var fromS = allSurahs.FirstOrDefault(s => s.Id == fromSurahId);
            var toS = allSurahs.FirstOrDefault(s => s.Id == toSurahId);
            if (fromS is null || toS is null)
                return (false, "سورة غير صالحة");

            var minOrder = Math.Min(fromS.SortOrder ?? fromS.Id, toS.SortOrder ?? toS.Id);
            var maxOrder = Math.Max(fromS.SortOrder ?? fromS.Id, toS.SortOrder ?? toS.Id);
            var targetSurahIds = allSurahs
                .Where(s => (s.SortOrder ?? s.Id) >= minOrder && (s.SortOrder ?? s.Id) <= maxOrder)
                .Select(s => s.Id)
                .ToList();

            allQuranData = await db.HolyQurans
                .AsNoTracking()
                .Where(h => targetSurahIds.Contains(h.sura_no))
                .OrderBy(h => h.sura_no).ThenBy(h => h.aya_no)
                .ToListAsync(cancellationToken);

            if (fromAyahNumber.HasValue || toAyahNumber.HasValue)
            {
                allQuranData = allQuranData.Where(h =>
                {
                    if (h.sura_no == fromSurahId && fromAyahNumber.HasValue && h.aya_no < fromAyahNumber.Value)
                        return false;
                    if (h.sura_no == toSurahId && toAyahNumber.HasValue && h.aya_no > toAyahNumber.Value)
                        return false;
                    return true;
                }).ToList();
            }
        }

        if (allQuranData.Count == 0)
            return (false, "لا توجد بيانات للقرآن في النطاق المحدد");

        var rowsWithDates = BuildRowsWithDates(allQuranData, unit, dailyQty, startDate, endDate, workDayNumbers);
        if (rowsWithDates.Count == 0)
            return (false, "لا توجد بيانات للقرآن في النطاق المحدد");

        var now = KuwaitTime.Now;
        foreach (var row in rowsWithDates)
        {
            db.StudentPlanRevises.Add(new StudentPlanRevise
            {
                StudentId = studentId,
                PlanId = planId,
                MemorizationLevel = level.LevelName,
                SurahId = row.SurahId,
                FromAyahNumber = row.FromAyah,
                ToAyahNumber = row.ToAyah,
                PlanDate = row.PlanDate,
                PlanEndDate = row.PlanDate,
                CreatedAt = now,
                Status = PlanRowStatus.Pending
            });
        }

        await db.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    private static List<PlanRowWithDate> BuildRowsWithDates(
        List<HolyQuran> allQuranData,
        PlanUnitType unit,
        int dailyQty,
        DateTime startDate,
        DateTime endDate,
        IReadOnlyList<int> workDayNumbers)
    {
        var rowsWithDates = new List<PlanRowWithDate>();
        var surahIdsInOrder = allQuranData.Select(h => h.sura_no).Distinct().ToList();
        var currentDate = startDate;

        foreach (var surahId in surahIdsInOrder)
        {
            var surahData = allQuranData
                .Where(h => h.sura_no == surahId)
                .OrderBy(h => h.page)
                .ThenBy(h => h.aya_no)
                .ToList();

            switch (unit)
            {
                case PlanUnitType.Page:
                case PlanUnitType.QuarterPage:
                case PlanUnitType.Line:
                {
                    var targetLines = dailyQty;
                    if (unit == PlanUnitType.Page)
                        targetLines = 15 * dailyQty;
                    else if (unit == PlanUnitType.QuarterPage)
                        targetLines = 4 * dailyQty;

                    var i = 0;
                    while (i < surahData.Count && currentDate <= endDate)
                    {
                        var startAya = surahData[i].aya_no;
                        var currentAya = startAya;
                        var acc = 0;
                        while (i < surahData.Count && acc < targetLines)
                        {
                            var aya = surahData[i];
                            var linesInAya = (aya.line_end ?? 1) - (aya.line_start ?? 1) + 1;
                            if (linesInAya <= 0)
                                linesInAya = 1;
                            acc += linesInAya;
                            currentAya = aya.aya_no;
                            i++;
                        }

                        rowsWithDates.Add(new PlanRowWithDate(surahId, startAya, currentAya, currentDate));
                        currentDate = GetNextWorkDay(currentDate, workDayNumbers);
                    }
                    break;
                }
                case PlanUnitType.Jozz:
                {
                    var surahJozzs = surahData
                        .GroupBy(h => h.jozz)
                        .Select(g => new
                        {
                            MinAya = g.Min(x => x.aya_no),
                            MaxAya = g.Max(x => x.aya_no)
                        })
                        .OrderBy(x => x.MinAya)
                        .ToList();

                    foreach (var jPart in surahJozzs)
                    {
                        if (currentDate <= endDate)
                        {
                            rowsWithDates.Add(new PlanRowWithDate(surahId, jPart.MinAya, jPart.MaxAya, currentDate));
                            currentDate = GetNextWorkDay(currentDate, workDayNumbers);
                        }
                    }
                    break;
                }
            }
        }

        return rowsWithDates;
    }

    public static DateTime GetNextWorkDay(DateTime current, IReadOnlyList<int> workDays)
    {
        if (workDays.Count == 0)
            return current.AddDays(1);

        var next = current.AddDays(1);
        var safety = 0;
        while (!workDays.Contains((int)next.DayOfWeek) && safety < 30)
        {
            next = next.AddDays(1);
            safety++;
        }

        return next;
    }

    private sealed record PlanRowWithDate(int SurahId, int FromAyah, int ToAyah, DateTime PlanDate);
}
