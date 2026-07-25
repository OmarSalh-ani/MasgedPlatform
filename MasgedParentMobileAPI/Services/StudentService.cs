using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Models;
using MasgedTeacherMobileAPI.Helpers;
using Microsoft.EntityFrameworkCore;

namespace MasgedParentMobileAPI.Services;

public class StudentService
{
    private readonly NewMasgedTeacherAPIDBContext _db;
    private readonly string _mediaBaseUrl;
    private readonly IWorkDayService _workDayService;

    public StudentService(
        NewMasgedTeacherAPIDBContext db,
        string mediaBaseUrl,
        IWorkDayService workDayService)
    {
        _db = db;
        _mediaBaseUrl = mediaBaseUrl;
        _workDayService = workDayService;
    }

    public async Task<List<RegisterForm>> GetParentStudentsAsync(string fatherPhone)
    {
        var variants = PhoneNormalizer.GetVariants(fatherPhone).ToList();
        return await _db.RegisterForms
            .Include(r => r.ParentFollowup)
            .Include(r => r.PlanLevel)
            .Include(r => r.QuranCircle)
                .ThenInclude(c => c!.Teacher)
            .Where(r => variants.Contains(r.FatherPhone) || variants.Contains(r.FatherPhone2))
            .ToListAsync();
    }

    public async Task<RegisterForm?> GetParentStudentByIdAsync(string fatherPhone, int studentId)
    {
        var variants = PhoneNormalizer.GetVariants(fatherPhone).ToList();
        return await _db.RegisterForms
            .Include(r => r.ParentFollowup)
            .Include(r => r.PlanLevel)
            .Include(r => r.QuranCircle)
                .ThenInclude(c => c.Teacher)
            .FirstOrDefaultAsync(r =>
                r.Id == studentId &&
                (variants.Contains(r.FatherPhone) || variants.Contains(r.FatherPhone2)));
    }

    public Task<List<CircleAttendance>> GetAttendanceBetweenAsync(
        int studentId,
        DateTime start,
        DateTime end) =>
        _db.CircleAttendances
            .Where(a =>
                a.StudentId == studentId &&
                a.AttendanceDateTime.Date >= start.Date &&
                a.AttendanceDateTime.Date <= end.Date)
            .ToListAsync();

    public Task<List<CircleDeparture>> GetDeparturesBetweenAsync(
        int studentId,
        DateTime start,
        DateTime end) =>
        _db.CircleDepartures
            .Where(d =>
                d.StudentId == studentId &&
                d.DepartureDate >= DateOnly.FromDateTime(start) &&
                d.DepartureDate <= DateOnly.FromDateTime(end))
            .ToListAsync();

    public async Task<StudentDto> BuildListItemAsync(
        RegisterForm student,
        List<CircleAttendance> weekAttendances,
        CircleDeparture? todayDeparture,
        DateTime weekStart)
    {
        var today = DateTime.Today;
        var isWorkDay = await _workDayService.IsWorkDayAsync(today);
        var todayAttendance = weekAttendances
            .Where(a => a.AttendanceDateTime.Date == today)
            .OrderByDescending(a => a.AttendanceDateTime)
            .FirstOrDefault();

        var status = AttendanceHelper.DetermineStatus(todayAttendance, todayDeparture, isWorkDay);
        var attendTime = AttendanceHelper.FormatTimeArabic(todayAttendance?.AttendanceDateTime);
        var workDayNumbers = await _workDayService.GetWorkDayNumbersAsync();

        string? departureTime = null;
        if (todayDeparture != null)
            departureTime = AttendanceHelper.FormatTimeOnlyArabic(todayDeparture.DepartureTime);
        else if (todayAttendance?.DepartureTime != null)
            departureTime = AttendanceHelper.FormatTimeOnlyArabic(todayAttendance.DepartureTime);

        return new StudentDto
        {
            Id = student.Id.ToString(),
            Name = student.StudentName ?? student.FullName ?? string.Empty,
            FullName = student.FullName,
            Level = student.PlanLevel?.LevelName ?? string.Empty,
            Group = student.QuranCircle?.Name ?? string.Empty,
            AvatarUrl = MediaUrlHelper.Resolve(student.ParentFollowup?.PhotoPath, _mediaBaseUrl),
            Status = status,
            AttendTime = attendTime,
            DepartureTime = departureTime,
            LogTime = attendTime,
            Notes = todayDeparture?.Notes,
            AttendancePercent = await CalcAttendancePercentAsync(student.Id),
            NextSession = string.Empty,
            WeeklyAttendance = AttendanceHelper.BuildWeeklyAttendance(weekAttendances, weekStart, workDayNumbers),
            TeacherId = student.QuranCircle?.TeacherId,
            TeacherName = student.QuranCircle?.Teacher?.Name,
        };
    }

    public async Task<StudentProfileDto> BuildProfileAsync(
        RegisterForm student,
        List<CircleAttendance> weekAttendances,
        CircleDeparture? todayDeparture,
        DateTime weekStart)
    {
        var listItem = await BuildListItemAsync(student, weekAttendances, todayDeparture, weekStart);
        var followup = student.ParentFollowup;

        var memorizing = await _db.StudentPlanMemorizings
            .Include(m => m.Surah)
            .Where(m => m.StudentId == student.Id)
            .OrderByDescending(m => m.PlanDate)
            .FirstOrDefaultAsync();

        var revise = await _db.StudentPlanRevises
            .Include(r => r.Surah)
            .Where(r => r.StudentId == student.Id)
            .OrderByDescending(r => r.PlanDate)
            .FirstOrDefaultAsync();

        var teacherNote = await _db.ParentNotes
            .Where(n => n.StudentId == student.Id)
            .OrderByDescending(n => n.CreatedDate)
            .Select(n => n.Notes)
            .FirstOrDefaultAsync();

        var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var monthAttendances = await _db.CircleAttendances
            .Where(a => a.StudentId == student.Id && a.AttendanceDateTime >= monthStart)
            .ToListAsync();

        var absentDays = monthAttendances
            .GroupBy(a => a.AttendanceDateTime.Date)
            .Count(g => g.All(a => !a.IsHere));

        return new StudentProfileDto
        {
            Id = listItem.Id,
            Name = listItem.Name,
            FullName = listItem.FullName,
            Level = listItem.Level,
            Group = listItem.Group,
            AvatarUrl = listItem.AvatarUrl,
            Status = listItem.Status,
            AttendTime = listItem.AttendTime,
            DepartureTime = listItem.DepartureTime,
            LogTime = listItem.LogTime,
            Notes = teacherNote ?? listItem.Notes,
            AttendancePercent = listItem.AttendancePercent,
            NextSession = listItem.NextSession,
            WeeklyAttendance = listItem.WeeklyAttendance,
            BirthDate = student.Birthdate,
            Address = followup?.Address,
            ParentName = student.FatherName,
            PhoneNumber = student.FatherPhone ?? student.StudentPhone,
            ParentMaritalStatus = followup?.MaritalStatus,
            HasHealthCondition = IsYes(followup?.HealthCondition) || !string.IsNullOrWhiteSpace(followup?.HealthDetails),
            HealthConditionDetails = followup?.HealthDetails,
            HasLearningDifficulties = IsYes(followup?.LearningDifficulties) || !string.IsNullOrWhiteSpace(followup?.LearningDifficultiesNotes),
            LearningDifficultiesDetails = followup?.LearningDifficultiesNotes,
            MemorizationProgress = memorizing == null
                ? null
                : FormatPlanProgress(memorizing.Surah?.NameAr, memorizing.FromAyahNumber, memorizing.ToAyahNumber),
            RevisionProgress = revise == null
                ? null
                : FormatPlanProgress(revise.Surah?.NameAr, revise.FromAyahNumber, revise.ToAyahNumber),
            AbsentDaysThisMonth = absentDays,
            LateCount = 0,
            TeacherId = listItem.TeacherId,
            TeacherName = listItem.TeacherName ?? student.QuranCircle?.Teacher?.Name,
        };
    }

    public static int CalculateAge(DateTime? birthDate)
    {
        if (!birthDate.HasValue) return 0;
        var today = DateTime.Today;
        var age = today.Year - birthDate.Value.Year;
        if (birthDate.Value.Date > today.AddYears(-age)) age--;
        return Math.Max(age, 0);
    }

    private async Task<int> CalcAttendancePercentAsync(int studentId)
    {
        var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var records = await _db.CircleAttendances
            .Where(a => a.StudentId == studentId && a.AttendanceDateTime >= monthStart)
            .ToListAsync();

        if (records.Count == 0) return 0;

        var present = records.Count(a => a.IsHere);
        return (int)Math.Round(100.0 * present / records.Count);
    }

    public async Task<ParentFollowupDto> GetParentFollowupAsync(string fatherPhone)
    {
        var students = await GetParentStudentsAsync(fatherPhone);
        if (students.Count == 0)
        {
            return new ParentFollowupDto();
        }

        var reference = students.FirstOrDefault(s => s.ParentFollowup != null) ?? students[0];
        var followup = reference.ParentFollowup;

        return new ParentFollowupDto
        {
            ParentName = reference.FatherName,
            Address = followup?.Address,
            MaritalStatus = followup?.MaritalStatus,
        };
    }

    public async Task<ParentFollowupDto> UpdateParentFollowupAsync(
        string fatherPhone,
        UpdateParentFollowupRequest request)
    {
        var students = await GetParentStudentsAsync(fatherPhone);
        if (students.Count == 0)
            throw new InvalidOperationException("لا يوجد أبناء مسجلون");

        if (!string.IsNullOrWhiteSpace(request.ParentName))
        {
            var parentName = request.ParentName.Trim();
            foreach (var student in students)
                student.FatherName = parentName;
        }

        foreach (var student in students)
        {
            var followup = student.ParentFollowup ?? await _db.ParentFollowups.FindAsync(student.Id);
            if (followup == null)
            {
                followup = new ParentFollowup { StudentId = student.Id };
                _db.ParentFollowups.Add(followup);
            }

            if (request.Address != null) followup.Address = request.Address;
            if (request.MaritalStatus != null) followup.MaritalStatus = request.MaritalStatus;

            student.ParentFollowup = followup;
        }

        await _db.SaveChangesAsync();
        return await GetParentFollowupAsync(fatherPhone);
    }

    private static bool IsYes(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        (value.Contains('ن') || value.Equals("yes", StringComparison.OrdinalIgnoreCase) || value == "1");

    private static string? FormatPlanProgress(string? surahName, int fromAyah, int toAyah)
    {
        if (string.IsNullOrWhiteSpace(surahName)) return null;
        if (fromAyah > 0 && toAyah > 0)
            return $"{surahName} - آية {fromAyah} إلى {toAyah}";
        return surahName;
    }

    public async Task<StudentPlan?> GetCurrentPlanAsync(int studentId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var plans = await _db.StudentPlans
            .AsNoTracking()
            .Where(p => p.StudentId == studentId && !p.IsArchived)
            .OrderBy(p => p.PlanFromDate)
            .ToListAsync();

        return plans.FirstOrDefault(p => today >= p.PlanFromDate && today <= p.PlanToDate)
               ?? plans.FirstOrDefault();
    }

    public async Task<List<int>> GetWorkDayNumbersAsync() =>
        (await _workDayService.GetWorkDayNumbersAsync()).ToList();

    public async Task<ParentStudentPlanOverviewDto> GetPlanOverviewAsync(RegisterForm student)
    {
        var plan = await GetCurrentPlanAsync(student.Id);
        if (plan is null)
            return new ParentStudentPlanOverviewDto();

        var workDayNumbers = await GetWorkDayNumbersAsync();

        var memStatuses = await _db.StudentPlanMemorizings
            .AsNoTracking()
            .Where(x => x.StudentId == student.Id && x.PlanId == plan.Id)
            .Select(x => x.Status)
            .ToListAsync();

        var revStatuses = await _db.StudentPlanRevises
            .AsNoTracking()
            .Where(x => x.StudentId == student.Id && x.PlanId == plan.Id)
            .Select(x => x.Status)
            .ToListAsync();

        var allStatuses = memStatuses.Concat(revStatuses).ToList();
        var progress = StudentPlan2Helper.BuildProgress(
            allStatuses,
            plan.PlanFromDate.ToDateTime(TimeOnly.MinValue),
            plan.PlanToDate.ToDateTime(TimeOnly.MinValue),
            workDayNumbers);

        var level = await _db.StudentPlanMemorizings
            .AsNoTracking()
            .Where(x => x.StudentId == student.Id && x.PlanId == plan.Id)
            .OrderByDescending(x => x.PlanDate)
            .Select(x => x.MemorizationLevel)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(level))
        {
            level = await _db.StudentPlanRevises
                .AsNoTracking()
                .Where(x => x.StudentId == student.Id && x.PlanId == plan.Id)
                .OrderByDescending(x => x.PlanDate)
                .Select(x => x.MemorizationLevel)
                .FirstOrDefaultAsync();
        }

        return new ParentStudentPlanOverviewDto
        {
            PlanId = plan.Id,
            PlanName = plan.Name,
            PlanFromDate = plan.PlanFromDate.ToDateTime(TimeOnly.MinValue),
            PlanToDate = plan.PlanToDate.ToDateTime(TimeOnly.MinValue),
            MemorizationLevel = level,
            Progress = new ParentPlanProgressDto
            {
                Passed = progress.Passed,
                Failed = progress.Failed,
                Pending = progress.Pending,
                Total = progress.Total,
                ProgressPercent = progress.ProgressPercent,
                DaysRemaining = progress.DaysRemaining,
                TotalPlanDays = progress.TotalPlanDays,
            },
        };
    }

    public async Task<PagedResultDto<ParentPlanRowDto>> GetPlanRowsAsync(
        int studentId,
        int planId,
        string planType,
        int page,
        int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var isRevise = planType.Trim() == "مراجعة";
        var skip = (page - 1) * pageSize;

        if (isRevise)
        {
            var query = _db.StudentPlanRevises
                .AsNoTracking()
                .Include(x => x.Surah)
                .Where(x => x.StudentId == studentId && x.PlanId == planId)
                .OrderBy(x => x.PlanDate);

            var totalCount = await query.CountAsync();
            var rows = await query.Skip(skip).Take(pageSize).ToListAsync();

            return new PagedResultDto<ParentPlanRowDto>
            {
                Items = rows.Select(MapReviseRow).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize),
            };
        }

        var memQuery = _db.StudentPlanMemorizings
            .AsNoTracking()
            .Include(x => x.Surah)
            .Where(x => x.StudentId == studentId && x.PlanId == planId)
            .OrderBy(x => x.PlanDate);

        var memTotal = await memQuery.CountAsync();
        var memRows = await memQuery.Skip(skip).Take(pageSize).ToListAsync();

        return new PagedResultDto<ParentPlanRowDto>
        {
            Items = memRows.Select(MapMemorizingRow).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = memTotal,
            TotalPages = memTotal == 0 ? 0 : (int)Math.Ceiling(memTotal / (double)pageSize),
        };
    }

    private static ParentPlanRowDto MapMemorizingRow(StudentPlanMemorizing x) =>
        new()
        {
            SurahName = x.Surah?.NameAr ?? "—",
            FromAyahNumber = x.FromAyahNumber,
            ToAyahNumber = x.ToAyahNumber,
            Status = PlanRowStatus.Normalize(x.Status),
            StatusDisplay = PlanRowStatus.GetDisplayLabel("memorizing_" + x.Id, x.Status),
            PlanType = "حفظ",
        };

    private static ParentPlanRowDto MapReviseRow(StudentPlanRevise x) =>
        new()
        {
            SurahName = x.Surah?.NameAr ?? "—",
            FromAyahNumber = x.FromAyahNumber,
            ToAyahNumber = x.ToAyahNumber,
            Status = PlanRowStatus.Normalize(x.Status),
            StatusDisplay = PlanRowStatus.GetDisplayLabel("revise_" + x.Id, x.Status),
            PlanType = "مراجعة",
        };
}
