using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Helpers;

public static class StudentPlan2Helper
{
    private static readonly string[] ArabicDayNames =
    [
        "الأحد", "الاثنين", "الثلاثاء", "الأربعاء", "الخميس", "الجمعة", "السبت"
    ];

    public static int CalcCircleDaysInRange(DateTime start, DateTime end, List<int> circleDayNumbers)
    {
        if (circleDayNumbers.Count == 0)
            return 0;

        var count = 0;
        for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
        {
            if (circleDayNumbers.Contains((int)d.DayOfWeek))
                count++;
        }

        return count;
    }

    public static PlanProgressDto BuildProgress(
        List<string?> statuses,
        DateTime planStart,
        DateTime planEnd,
        List<int> circleDayNumbers)
    {
        var passed = statuses.Count(s => PlanRowStatus.IsPass(s));
        var failed = statuses.Count(s => PlanRowStatus.IsFail(s));
        var retake = statuses.Count(s => PlanRowStatus.IsRetake(s));
        var total = statuses.Count;
        var pending = Math.Max(0, total - passed - failed - retake);

        var today = KuwaitTime.Today;
        var totalPlanDays = CalcCircleDaysInRange(planStart.Date, planEnd.Date, circleDayNumbers);
        if (totalPlanDays <= 0)
            totalPlanDays = Math.Max(1, (planEnd.Date - planStart.Date).Days + 1);

        var rangeStart = today > planStart.Date ? today : planStart.Date;
        var daysRemaining = CalcCircleDaysInRange(rangeStart, planEnd.Date, circleDayNumbers);
        if (today > planEnd.Date)
            daysRemaining = 0;

        var daysElapsed = totalPlanDays - daysRemaining;
        if (daysElapsed < 0)
            daysElapsed = 0;

        var progressPercent = total > 0 ? (int)Math.Round(100.0 * passed / total) : 0;

        return new PlanProgressDto
        {
            Passed = passed,
            Failed = failed,
            Pending = pending,
            Retake = retake,
            Total = total,
            DaysRemaining = daysRemaining,
            TotalPlanDays = totalPlanDays,
            DaysElapsed = daysElapsed,
            ProgressPercent = progressPercent,
            CircleDaysInRange = CalcCircleDaysInRange(planStart.Date, planEnd.Date, circleDayNumbers)
        };
    }

    public static PlanRowDto MapMemorizingRow(StudentPlanMemorizing x) =>
        new()
        {
            Key = "memorizing_" + x.Id,
            PlanType = "حفظ",
            MemorizationLevel = x.MemorizationLevel,
            SurahId = x.SurahId,
            SurahName = ManualPlanRowHelper.IsManual(x.MemorizationLevel)
                ? ManualPlanRowHelper.ExtractName(x.MemorizationLevel)
                : x.QuranSurah?.NameAr ?? "—",
            FromAyahNumber = x.FromAyahNumber,
            ToAyahNumber = x.ToAyahNumber,
            PlanDate = x.PlanDate,
            Status = PlanRowStatus.Normalize(x.Status),
            StatusDisplay = PlanRowStatus.GetDisplayLabel("memorizing_" + x.Id, x.Status),
            MemorizeDate = x.MemorizeDate,
            ReviseDate = x.ReviseDate,
            IsManual = ManualPlanRowHelper.IsManual(x.MemorizationLevel),
        };

    public static PlanRowDto MapReviseRow(StudentPlanRevise x) =>
        new()
        {
            Key = "revise_" + x.Id,
            PlanType = "مراجعة",
            MemorizationLevel = x.MemorizationLevel,
            SurahId = x.SurahId,
            SurahName = ManualPlanRowHelper.IsManual(x.MemorizationLevel)
                ? ManualPlanRowHelper.ExtractName(x.MemorizationLevel)
                : x.QuranSurah?.NameAr ?? "—",
            FromAyahNumber = x.FromAyahNumber,
            ToAyahNumber = x.ToAyahNumber,
            PlanDate = x.PlanDate,
            Status = PlanRowStatus.Normalize(x.Status),
            StatusDisplay = PlanRowStatus.GetDisplayLabel("revise_" + x.Id, x.Status),
            MemorizeDate = x.MemorizeDate,
            ReviseDate = x.ReviseDate,
            IsManual = ManualPlanRowHelper.IsManual(x.MemorizationLevel),
        };

    public static async Task<List<PlanRowInputDto>> ExpandSurahRowsAsync(
        AppDbContext db,
        IEnumerable<PlanRowInputDto> rows,
        CancellationToken cancellationToken)
    {
        var expanded = new List<PlanRowInputDto>();
        foreach (var row in rows)
        {
            if (!string.IsNullOrWhiteSpace(row.SurahName))
            {
                expanded.Add(new PlanRowInputDto
                {
                    SurahId = ManualPlanRowHelper.PlaceholderSurahId,
                    SurahName = row.SurahName.Trim(),
                    FromAyahNumber = row.FromAyahNumber,
                    ToAyahNumber = row.ToAyahNumber,
                    PlanType = row.PlanType,
                    PlanDate = row.PlanDate,
                    Status = row.Status,
                    UseNextWorkDay = row.UseNextWorkDay,
                });
                continue;
            }

            if (row.SurahId <= 114)
            {
                expanded.Add(row);
                continue;
            }

            IQueryable<HolyQuran> q = db.HolyQurans.AsNoTracking();
            if (row.SurahId > 1000 && row.SurahId <= 1100)
            {
                var hezb = row.SurahId - 1000;
                q = q.Where(x => x.jozz == hezb);
            }
            else if (row.SurahId > 3000 && row.SurahId <= 3030)
            {
                var juz = row.SurahId - 3000;
                q = q.Where(x => x.jozz == juz);
            }
            else
            {
                expanded.Add(row);
                continue;
            }

            var items = await q
                .GroupBy(x => x.sura_no)
                .Select(g => new { sura_no = g.Key, min = g.Min(a => a.aya_no), max = g.Max(a => a.aya_no) })
                .ToListAsync(cancellationToken);

            foreach (var item in items)
            {
                expanded.Add(new PlanRowInputDto
                {
                    SurahId = item.sura_no,
                    FromAyahNumber = item.min,
                    ToAyahNumber = item.max,
                    PlanType = row.PlanType,
                    PlanDate = row.PlanDate,
                    Status = row.Status,
                    UseNextWorkDay = row.UseNextWorkDay
                });
            }
        }

        return expanded;
    }

    public static async Task<StudentPlan2DetailDto> BuildPlanDetailAsync(
        AppDbContext db,
        RegisterForm student,
        StudentPlan plan,
        List<int> circleDayNumbers,
        CancellationToken cancellationToken)
    {
        var mem = await db.StudentPlanMemorizings
            .AsNoTracking()
            .Include(x => x.QuranSurah)
            .Where(x => x.StudentId == student.Id && x.PlanId == plan.Id)
            .OrderBy(x => x.PlanDate)
            .ThenBy(x => x.FromAyahNumber)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        var rev = await db.StudentPlanRevises
            .AsNoTracking()
            .Include(x => x.QuranSurah)
            .Where(x => x.StudentId == student.Id && x.PlanId == plan.Id)
            .OrderBy(x => x.PlanDate)
            .ThenBy(x => x.FromAyahNumber)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        var memRows = mem.Select(MapMemorizingRow).ToList();
        var revRows = rev.Select(MapReviseRow).ToList();
        var combined = memRows.Concat(revRows)
            .OrderBy(x => x.PlanDate)
            .ThenBy(x => x.FromAyahNumber)
            .ThenBy(x => x.Key)
            .ToList();

        var currentMem = memRows.FirstOrDefault(x =>
            PlanRowStatus.IsPending(x.Status) || PlanRowStatus.IsFail(x.Status));
        var currentRev = revRows.FirstOrDefault(x =>
            PlanRowStatus.IsPending(x.Status) || PlanRowStatus.IsFail(x.Status));

        var tathbitRows = combined
            .Where(x => PlanRowStatus.IsPass(x.Status))
            .ToList();

        var editableMem = memRows
            .Where(x => !PlanRowStatus.IsPass(x.Status))
            .ToList();

        var allRowDates = mem.Select(x => new { x.PlanDate, x.PlanEndDate, x.MemorizationLevel })
            .Concat(rev.Select(x => new { x.PlanDate, x.PlanEndDate, x.MemorizationLevel }))
            .ToList();

        var planStart = plan.PlanFromDate;
        var planEnd = plan.PlanToDate;
        var level = allRowDates.OrderByDescending(x => x.PlanDate).FirstOrDefault()?.MemorizationLevel;

        var statuses = mem.Select(x => x.Status).Concat(rev.Select(x => x.Status)).ToList();
        var progress = BuildProgress(statuses, planStart, planEnd, circleDayNumbers);

        var today = KuwaitTime.Today;
        var plans = await db.StudentPlans
            .AsNoTracking()
            .Where(p => p.StudentId == student.Id && !p.IsArchived)
            .OrderBy(p => p.PlanFromDate)
            .ToListAsync(cancellationToken);

        var planSummaries = plans.Select(p => new StudentPlanSummaryDto
        {
            Id = p.Id,
            Name = p.Name,
            PlanFromDate = p.PlanFromDate,
            PlanToDate = p.PlanToDate,
            IsCurrent = today >= p.PlanFromDate && today <= p.PlanToDate
        }).ToList();

        var calendarDays = BuildCalendar(plan, combined, circleDayNumbers);
        var assessmentLog = await BuildAssessmentLogAsync(db, plan.Id, cancellationToken);

        return new StudentPlan2DetailDto
        {
            StudentId = student.Id,
            StudentName = student.FullName ?? student.StudentName,
            PlanId = plan.Id,
            PlanName = plan.Name,
            PlanFromDate = planStart,
            PlanToDate = planEnd,
            MemorizationLevel = level,
            Progress = progress,
            CurrentMemorizing = currentMem,
            CurrentRevise = currentRev,
            TathbitRows = tathbitRows,
            AllRows = combined,
            EditableMemorizingRows = editableMem,
            CalendarDays = calendarDays,
            AssessmentLog = assessmentLog,
            Plans = planSummaries
        };
    }

    private static List<CalendarDayDto> BuildCalendar(
        StudentPlan plan,
        List<PlanRowDto> allRows,
        List<int> circleDayNumbers)
    {
        var byDate = allRows.GroupBy(r => r.PlanDate.Date).ToDictionary(g => g.Key, g => g.ToList());
        var days = new List<CalendarDayDto>();
        var start = plan.PlanFromDate.Date;
        var end = plan.PlanToDate.Date;

        for (var d = start; d <= end; d = d.AddDays(1))
        {
            var isCircle = circleDayNumbers.Contains((int)d.DayOfWeek);
            var vm = new CalendarDayDto
            {
                Date = d,
                DayNameAr = ArabicDayNames[(int)d.DayOfWeek],
                IsCircleDay = isCircle
            };

            if (isCircle && byDate.TryGetValue(d, out var rows))
            {
                vm.Items = rows.Select(row =>
                {
                    var typeDisplay = PlanRowStatus.IsPass(row.Status) ? "تثبيت" : row.PlanType;
                    return new CalendarSurahItemDto
                    {
                        SurahId = row.SurahId,
                        SurahName = "سورة " + row.SurahName,
                        PlanType = typeDisplay,
                        FromAyahNumber = row.FromAyahNumber,
                        ToAyahNumber = row.ToAyahNumber
                    };
                }).ToList();
            }

            days.Add(vm);
        }

        return days;
    }

    private static async Task<List<AssessmentLogEntryDto>> BuildAssessmentLogAsync(
        AppDbContext db,
        int planId,
        CancellationToken cancellationToken)
    {
        var logEntries = await db.StudentPlanItemLogs
            .AsNoTracking()
            .Include(x => x.Teacher)
            .Where(x => x.PlanId == planId)
            .OrderByDescending(x => x.LoggedAt)
            .Take(50)
            .ToListAsync(cancellationToken);

        var keys = logEntries.Select(x => x.RowKey).Distinct().ToList();
        var rowLabels = new Dictionary<string, string>();

        foreach (var key in keys)
        {
            var label = key;
            if (key.StartsWith("memorizing_") && int.TryParse(key["memorizing_".Length..], out var memId))
            {
                var e = await db.StudentPlanMemorizings
                    .AsNoTracking()
                    .Include(x => x.QuranSurah)
                    .FirstOrDefaultAsync(x => x.Id == memId && x.PlanId == planId, cancellationToken);
                if (e is not null)
                    label = (e.QuranSurah?.NameAr ?? "—") + " " + e.FromAyahNumber + "-" + e.ToAyahNumber;
            }
            else if (key.StartsWith("revise_") && int.TryParse(key["revise_".Length..], out var revId))
            {
                var e = await db.StudentPlanRevises
                    .AsNoTracking()
                    .Include(x => x.QuranSurah)
                    .FirstOrDefaultAsync(x => x.Id == revId && x.PlanId == planId, cancellationToken);
                if (e is not null)
                    label = (e.QuranSurah?.NameAr ?? "—") + " " + e.FromAyahNumber + "-" + e.ToAyahNumber;
            }

            rowLabels[key] = label;
        }

        return logEntries.Select(x => new AssessmentLogEntryDto
        {
            RowLabel = rowLabels.GetValueOrDefault(x.RowKey, x.RowKey),
            Status = x.Status,
            StatusDisplay = PlanRowStatus.GetDisplayLabel(x.RowKey, x.Status),
            TeacherName = x.Teacher?.Name ?? "",
            LoggedAtFormatted = x.LoggedAt.ToString("yyyy-MM-dd hh:mm tt")
        }).ToList();
    }
}
