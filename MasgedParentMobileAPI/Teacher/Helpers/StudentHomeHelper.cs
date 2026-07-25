using System.Globalization;
using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Helpers;

public static class StudentHomeHelper
{
    private const int HomeStudentsLimit = 100;
    public const string VacationLabel = "اجازة";

    public static (string IsPresentToday, string DepartureStatusToday, string DepartureTimeToday)
        GetTodayAttendanceDisplay(
            CircleAttendance? todayAttendance,
            DateTime? departureDate,
            bool isWorkDay)
    {
        if (!isWorkDay)
            return (VacationLabel, string.Empty, string.Empty);

        var hasDeparted = departureDate.HasValue;

        var departureTimeToday = hasDeparted
            ? departureDate!.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture)
            : string.Empty;

        var departureStatusToday = hasDeparted ? "منصرف" : "لم ينصرف";

        if (todayAttendance == null || !todayAttendance.IsHere)
            return ("غائب", departureStatusToday, departureTimeToday);

        if (hasDeparted)
            return ("منصرف", departureStatusToday, departureTimeToday);

        return ("حاضر", departureStatusToday, string.Empty);
    }

    public static async Task<StudentsStatisticsDto> ComputeCircleStatisticsAsync(
        AppDbContext db,
        int circleId,
        bool isWorkDay,
        CancellationToken cancellationToken,
        IQueryable<RegisterForm>? filteredQuery = null)
    {
        var query = filteredQuery ?? db.RegisterForms.AsNoTracking().Where(x => x.QuranCircleId == circleId);
        var nowDate = KuwaitTime.Today;
        var tomorrow = nowDate.AddDays(1);

        var totalCount = await query.CountAsync(cancellationToken);
        var studentIds = query.Select(x => x.Id);

        var presentStudents = await db.CircleAttendances
            .AsNoTracking()
            .Where(ca => studentIds.Contains(ca.StudentId)
                         && ca.AttendanceDateTime >= nowDate
                         && ca.AttendanceDateTime < tomorrow
                         && ca.IsHere)
            .Select(ca => ca.StudentId)
            .Distinct()
            .CountAsync(cancellationToken);

        var departedStudents = await db.CircleAttendances
            .AsNoTracking()
            .Where(ca => studentIds.Contains(ca.StudentId)
                         && ca.DepartureDate.HasValue
                         && ca.DepartureDate >= nowDate
                         && ca.DepartureDate < tomorrow)
            .Select(ca => ca.StudentId)
            .Distinct()
            .CountAsync(cancellationToken);

        return new StudentsStatisticsDto
        {
            TotalStudents = totalCount,
            PresentStudents = presentStudents,
            AbsentStudents = isWorkDay ? totalCount - presentStudents : 0,
            DepartedStudents = departedStudents
        };
    }

    public static async Task<List<StudentDto>> LoadCircleStudentsAsync(
        AppDbContext db,
        int circleId,
        bool isWorkDay,
        CancellationToken cancellationToken,
        string? search = null)
    {
        var query = db.RegisterForms
            .AsNoTracking()
            .Where(x => x.QuranCircleId == circleId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.StudentName.Contains(term));
        }

        var students = await query
            .Include(x => x.QuranCircle)
            .Include(x => x.ParentFollowup)
            .Include(x => x.PlanLevel)
            .Include(x => x.CircleAttendances)
            .OrderBy(x => x.StudentName)
            .Take(HomeStudentsLimit)
            .ToListAsync(cancellationToken);

        return await MapToStudentDtosAsync(db, students, isWorkDay, cancellationToken);
    }

    public static async Task<List<StudentDto>> MapToStudentDtosAsync(
        AppDbContext db,
        List<RegisterForm> students,
        bool isWorkDay,
        CancellationToken cancellationToken)
    {
        if (students.Count == 0)
            return [];

        var nowDate = KuwaitTime.Today;
        var tomorrow = nowDate.AddDays(1);
        var studentIds = students.Select(s => s.Id).ToList();

        var warningCounts = await db.TeacherNotes
            .AsNoTracking()
            .Where(n => studentIds.Contains(n.StudentId) && n.IsWarning)
            .GroupBy(n => n.StudentId)
            .Select(g => new { StudentId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.StudentId, x => x.Count, cancellationToken);

        var parentQuestionCounts = await db.ParentNotes
            .AsNoTracking()
            .Where(n => studentIds.Contains(n.StudentId) && !n.IsRead)
            .GroupBy(n => n.StudentId)
            .Select(g => new { StudentId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.StudentId, x => x.Count, cancellationToken);

        var todayDepartures = await db.CircleAttendances
            .AsNoTracking()
            .Where(ca => studentIds.Contains(ca.StudentId)
                         && ca.DepartureDate.HasValue
                         && ca.DepartureDate >= nowDate
                         && ca.DepartureDate < tomorrow)
            .GroupBy(ca => ca.StudentId)
            .Select(g => g.OrderByDescending(ca => ca.DepartureDate).First())
            .ToDictionaryAsync(x => x.StudentId, cancellationToken);

        var studentsNeedingPlanFallback = students
            .Where(s => string.IsNullOrWhiteSpace(s.PlanLevel?.LevelName))
            .Select(s => s.Id)
            .ToList();

        var planLevelFallbacks = await GetLatestPlanMemorizationLevelsAsync(
            db,
            studentsNeedingPlanFallback,
            cancellationToken);

        return students.Select(x =>
        {
            var todayAttendance = x.CircleAttendances
                .FirstOrDefault(c => c.AttendanceDateTime >= nowDate
                                     && c.AttendanceDateTime < tomorrow
                                     && c.StudentId == x.Id);

            todayDepartures.TryGetValue(x.Id, out var todayDeparture);

            var departureDate = todayDeparture?.DepartureDate ?? todayAttendance?.DepartureDate;
            var (isPresentToday, departureStatusToday, departureTimeToday) =
                GetTodayAttendanceDisplay(todayAttendance, departureDate, isWorkDay);

            return new StudentDto
            {
                Age = x.Age,
                Group = x.QuranCircle?.Name ?? string.Empty,
                IsPresentToday = isPresentToday,
                Id = x.Id,
                Name = x.StudentName,
                ImageUrl = x.ParentFollowup?.photoPath != null
                    ? MediaUrlHelper.Resolve(x.ParentFollowup.photoPath)
                    : string.Empty,
                FatherPhone = x.FatherPhone,
                WarningCount = warningCounts.GetValueOrDefault(x.Id),
                ParentQuestionsCount = parentQuestionCounts.GetValueOrDefault(x.Id),
                HasHealthCondition = x.ParentFollowup?.healthCondition == "نعم",
                HasLearningDifficulties = x.ParentFollowup?.learningDifficulties == "نعم",
                DepartureStatusToday = departureStatusToday,
                DepartureTimeToday = departureTimeToday,
                IsSpecial = x.IsSpecial,
                IsElite = x.IsElite,
                PlanLevelName = ResolvePlanLevelName(x, planLevelFallbacks),
                PlanLevelId = x.PlanLevelId
            };
        }).ToList();
    }

    public static string ResolvePlanLevelName(
        RegisterForm student,
        IReadOnlyDictionary<int, string> planRowLevelFallbacks)
    {
        if (!string.IsNullOrWhiteSpace(student.PlanLevel?.LevelName))
            return student.PlanLevel.LevelName;

        if (planRowLevelFallbacks.TryGetValue(student.Id, out var level)
            && !string.IsNullOrWhiteSpace(level))
            return level;

        return "غير محدد";
    }

    public static async Task<Dictionary<int, string>> GetLatestPlanMemorizationLevelsAsync(
        AppDbContext db,
        IEnumerable<int> studentIds,
        CancellationToken cancellationToken)
    {
        var ids = studentIds.Distinct().ToList();
        if (ids.Count == 0)
            return [];

        var memorizingLevels = await db.StudentPlanMemorizings
            .AsNoTracking()
            .Where(m => ids.Contains(m.StudentId) && m.MemorizationLevel != "")
            .Select(m => new { m.StudentId, m.MemorizationLevel, m.PlanDate })
            .ToListAsync(cancellationToken);

        var reviseLevels = await db.StudentPlanRevises
            .AsNoTracking()
            .Where(m => ids.Contains(m.StudentId) && m.MemorizationLevel != "")
            .Select(m => new { m.StudentId, m.MemorizationLevel, m.PlanDate })
            .ToListAsync(cancellationToken);

        return memorizingLevels
            .Concat(reviseLevels)
            .GroupBy(x => x.StudentId)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(x => x.PlanDate).First().MemorizationLevel);
    }
}
