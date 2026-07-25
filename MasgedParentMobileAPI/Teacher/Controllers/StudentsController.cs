using System.Security.Claims;
using MasgedParentMobileAPI.Services;
using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Entities;
using MasgedTeacherMobileAPI.Enums;
using MasgedTeacherMobileAPI.Extensions;
using MasgedTeacherMobileAPI.Helpers;
using MediaUrlHelper = MasgedTeacherMobileAPI.Helpers.MediaUrlHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StudentsController(AppDbContext db, IWorkDayService workDayService) : ControllerBase
{
    private const int MaxPageSize = 100;

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out var teacherId, out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("حلقة المعلم غير محددة"));

        var isWorkDayToday = await workDayService.IsWorkDayAsync(KuwaitTime.Today, cancellationToken);
        var statistics = await StudentHomeHelper.ComputeCircleStatisticsAsync(
            db, circleId, isWorkDayToday, cancellationToken);

        var planLevels = await db.PlanLevels
            .AsNoTracking()
            .OrderBy(x => x.LevelName)
            .Select(x => new IdNameDto { Id = x.Id, Name = x.LevelName })
            .ToListAsync(cancellationToken);

        var unreadAdminNotes = await db.TeachersAdminNotes
            .CountAsync(n => n.TeacherId == teacherId && !n.IsRead, cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(new IndexDashboardDto
        {
            TeacherName = User.FindFirstValue("name") ?? "المعلم",
            Statistics = statistics,
            PlanLevels = planLevels,
            UnreadAdminNotesCount = unreadAdminNotes
        }));
    }

    [HttpGet("{studentId:int}/notes")]
    public async Task<IActionResult> GetStudentNotes(
        int studentId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCircleId(out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("حلقة المعلم غير محددة"));

        var inCircle = await db.RegisterForms
            .AnyAsync(s => s.Id == studentId && s.QuranCircleId == circleId, cancellationToken);

        if (!inCircle)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود في الحلقة الحالية"));

        var notes = await db.TeacherNotes
            .AsNoTracking()
            .Where(n => n.StudentId == studentId)
            .OrderByDescending(n => n.CreatedDate)
            .Select(n => new TeacherNoteDto
            {
                Id = n.Id,
                Notes = n.Notes,
                CreatedDate = n.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                IsWarning = n.IsWarning,
                IsRead = n.IsRead
            })
            .ToListAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(notes));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllStudents(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool showOnlySpecial = false,
        [FromQuery] bool showOnlyElite = false,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
            return this.ToActionResult(GlobalResponse.BadRequest("رقم الصفحة غير صالح"));

        if (pageSize < 1 || pageSize > MaxPageSize)
            return this.ToActionResult(GlobalResponse.BadRequest($"حجم الصفحة يجب أن يكون بين 1 و {MaxPageSize}"));

        var circleIdClaim = User.FindFirstValue("circleId");
        if (!int.TryParse(circleIdClaim, out var circleId) || circleId <= 0)
            return this.ToActionResult(GlobalResponse.BadRequest("حلقة المعلم غير محددة"));

        var query = db.RegisterForms
            .AsNoTracking()
            .Where(x => x.QuranCircleId == circleId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(x => x.StudentName.StartsWith(searchTerm));

        if (showOnlySpecial)
            query = query.Where(x => x.IsSpecial);

        if (showOnlyElite)
            query = query.Where(x => x.IsElite);

        var nowDate = KuwaitTime.Today;
        var tomorrow = nowDate.AddDays(1);

        var isWorkDayToday = await workDayService.IsWorkDayAsync(KuwaitTime.Today, cancellationToken);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        var stats = await StudentHomeHelper.ComputeCircleStatisticsAsync(
            db, circleId, isWorkDayToday, cancellationToken, query);
        var presentStudents = stats.PresentStudents;
        var departedStudents = stats.DepartedStudents;

        var studentsPage = await query
            .Include(x => x.QuranCircle)
            .Include(x => x.ParentFollowup)
            .Include(x => x.PlanLevel)
            .Include(x => x.CircleAttendances)
            .OrderBy(x => x.StudentName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var pageStudentIds = studentsPage.Select(s => s.Id).ToList();

        var warningCounts = await db.TeacherNotes
            .AsNoTracking()
            .Where(n => pageStudentIds.Contains(n.StudentId) && n.IsWarning)
            .GroupBy(n => n.StudentId)
            .Select(g => new { StudentId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.StudentId, x => x.Count, cancellationToken);

        var parentQuestionCounts = await db.ParentNotes
            .AsNoTracking()
            .Where(n => pageStudentIds.Contains(n.StudentId) && !n.IsRead)
            .GroupBy(n => n.StudentId)
            .Select(g => new { StudentId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.StudentId, x => x.Count, cancellationToken);

        var todayDepartures = await db.CircleAttendances
            .AsNoTracking()
            .Where(ca => pageStudentIds.Contains(ca.StudentId)
                         && ca.DepartureDate.HasValue
                         && ca.DepartureDate >= nowDate
                         && ca.DepartureDate < tomorrow)
            .GroupBy(ca => ca.StudentId)
            .Select(g => g.OrderByDescending(ca => ca.DepartureDate).First())
            .ToDictionaryAsync(x => x.StudentId, cancellationToken);

        var studentsNeedingPlanFallback = studentsPage
            .Where(s => string.IsNullOrWhiteSpace(s.PlanLevel?.LevelName))
            .Select(s => s.Id)
            .ToList();

        var planLevelFallbacks = await StudentHomeHelper.GetLatestPlanMemorizationLevelsAsync(
            db,
            studentsNeedingPlanFallback,
            cancellationToken);

        var items = studentsPage.Select(x =>
        {
            var todayAttendance = x.CircleAttendances
                .FirstOrDefault(c => c.AttendanceDateTime >= nowDate
                                     && c.AttendanceDateTime < tomorrow
                                     && c.StudentId == x.Id);

            todayDepartures.TryGetValue(x.Id, out var todayDeparture);

            var departureDate = todayDeparture?.DepartureDate ?? todayAttendance?.DepartureDate;
            var (isPresentToday, departureStatusToday, departureTimeToday) =
                StudentHomeHelper.GetTodayAttendanceDisplay(todayAttendance, departureDate, isWorkDayToday);

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
                PlanLevelName = StudentHomeHelper.ResolvePlanLevelName(x, planLevelFallbacks),
                PlanLevelId = x.PlanLevelId
            };
        }).ToList();

        var data = new StudentsListResponseDto
        {
            Students = new PagedResultDto<StudentDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            },
            Statistics = new StudentsStatisticsDto
            {
                TotalStudents = stats.TotalStudents,
                PresentStudents = presentStudents,
                AbsentStudents = stats.AbsentStudents,
                DepartedStudents = departedStudents
            }
        };

        return this.ToActionResult(GlobalResponse.Ok(data));
    }

    [HttpGet("filtered")]
    public async Task<IActionResult> GetFilteredStudents(
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool showOnlySpecial = false,
        [FromQuery] bool showOnlyElite = false,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetCircleId(out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("حلقة المعلم غير محددة"));

        var query = db.RegisterForms
            .AsNoTracking()
            .Where(x => x.QuranCircleId == circleId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(x => x.StudentName.StartsWith(searchTerm));

        if (showOnlySpecial)
            query = query.Where(x => x.IsSpecial);

        if (showOnlyElite)
            query = query.Where(x => x.IsElite);

        var students = await query
            .Include(x => x.QuranCircle)
            .Include(x => x.ParentFollowup)
            .Include(x => x.PlanLevel)
            .Include(x => x.CircleAttendances)
            .OrderBy(x => x.StudentName)
            .ToListAsync(cancellationToken);

        var isWorkDayToday = await workDayService.IsWorkDayAsync(KuwaitTime.Today, cancellationToken);
        var items = await StudentHomeHelper.MapToStudentDtosAsync(db, students, isWorkDayToday, cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(new FilteredStudentsResponseDto
        {
            Students = items,
            Count = items.Count,
            ShowOnlySpecial = showOnlySpecial,
            ShowOnlyElite = showOnlyElite,
            SearchTerm = searchTerm ?? ""
        }));
    }

    [HttpPost("{studentId:int}/notes")]
    public async Task<IActionResult> SaveNote(
        int studentId,
        [FromBody] SaveStudentNoteRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out var teacherId, out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("حلقة المعلم غير محددة"));

        if (string.IsNullOrWhiteSpace(request.NotesText))
            return this.ToActionResult(GlobalResponse.BadRequest("نص الملاحظة مطلوب"));

        var studentExists = await db.RegisterForms
            .AnyAsync(s => s.Id == studentId && s.QuranCircleId == circleId, cancellationToken);

        if (!studentExists)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود في الحلقة الحالية"));

        db.TeacherNotes.Add(new TeacherNote
        {
            StudentId = studentId,
            TeacherId = teacherId,
            Notes = request.NotesText.Trim(),
            CreatedDate = KuwaitTime.Now,
            IsWarning = request.IsWarning,
            IsRead = false
        });

        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(message: "تم حفظ الملاحظة بنجاح"));
    }

    [HttpPost("notes/bulk")]
    public async Task<IActionResult> SaveNotesToMultipleStudents(
        [FromBody] SaveBulkNotesRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out var teacherId, out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("حلقة المعلم غير محددة"));

        if (request.StudentIds.Count == 0)
            return this.ToActionResult(GlobalResponse.BadRequest("يجب تحديد طالب واحد على الأقل"));

        if (string.IsNullOrWhiteSpace(request.NoteText))
            return this.ToActionResult(GlobalResponse.BadRequest("نص الملاحظة مطلوب"));

        var validStudentIds = await db.RegisterForms
            .AsNoTracking()
            .Where(s => request.StudentIds.Contains(s.Id) && s.QuranCircleId == circleId)
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        var errors = new List<string>();
        var now = KuwaitTime.Now;

        foreach (var studentId in request.StudentIds)
        {
            if (!validStudentIds.Contains(studentId))
                errors.Add($"الطالب رقم {studentId} غير موجود في الحلقة الحالية");
        }

        foreach (var studentId in validStudentIds)
        {
            db.TeacherNotes.Add(new TeacherNote
            {
                StudentId = studentId,
                TeacherId = teacherId,
                Notes = request.NoteText.Trim(),
                CreatedDate = now,
                IsWarning = request.IsWarning,
                IsRead = false
            });
        }

        if (validStudentIds.Count > 0)
            await db.SaveChangesAsync(cancellationToken);

        if (validStudentIds.Count == 0)
        {
            return this.ToActionResult(GlobalResponse.BadRequest(
                "لم يتم إرسال الملاحظة لأي طالب: " + string.Join("، ", errors)));
        }

        var message = errors.Count == 0
            ? $"تم إرسال الملاحظة بنجاح لـ {validStudentIds.Count} طالب"
            : $"تم إرسال الملاحظة لـ {validStudentIds.Count} طالب. أخطاء: " + string.Join("، ", errors);

        return this.ToActionResult(GlobalResponse.Ok(
            new SaveBulkNotesResponseDto
            {
                SentCount = validStudentIds.Count,
                ErrorCount = errors.Count,
                Errors = errors
            },
            message));
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableStudents(
        [FromQuery] string? searchTerm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
            return this.ToActionResult(GlobalResponse.BadRequest("رقم الصفحة غير صالح"));

        if (pageSize < 1 || pageSize > MaxPageSize)
            return this.ToActionResult(GlobalResponse.BadRequest($"حجم الصفحة يجب أن يكون بين 1 و {MaxPageSize}"));

        if (!TryGetCircleId(out _))
            return this.ToActionResult(GlobalResponse.BadRequest("حلقة المعلم غير محددة"));

        var isGirlTeacher = bool.TryParse(User.FindFirstValue("isGirlTeacher"), out var girl) && girl;
        var gender = isGirlTeacher ? "أنثى" : "ذكر";

        var query = db.RegisterForms
            .AsNoTracking()
            .Where(s => s.QuranCircleId == null && s.StudentGender == gender);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(s => s.StudentName.StartsWith(searchTerm));

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        var students = await query
            .OrderBy(s => s.StudentName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new AvailableStudentDto
            {
                Id = s.Id,
                StudentName = s.StudentName,
                FatherPhone = s.FatherPhone,
                Age = s.Age
            })
            .ToListAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(new PagedResultDto<AvailableStudentDto>
        {
            Items = students,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        }));
    }

    [HttpGet("former")]
    public async Task<IActionResult> GetFormerStudents(CancellationToken cancellationToken)
    {
        if (!TryGetCircleId(out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("حلقة المعلم غير محددة"));

        var students = await db.StudentCircleEnrollments
            .AsNoTracking()
            .Where(e => e.CircleId == circleId && e.EndDate != null)
            .Where(e => e.RegisterForm!.QuranCircleId != circleId || e.RegisterForm.QuranCircleId == null)
            .Select(e => new FormerStudentDto
            {
                Id = e.StudentId,
                StudentName = e.RegisterForm!.StudentName,
                FatherPhone = e.RegisterForm.FatherPhone,
                Age = e.RegisterForm.Age,
                LeftDate = e.EndDate
            })
            .Distinct()
            .OrderByDescending(s => s.LeftDate)
            .ThenBy(s => s.StudentName)
            .ToListAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(students));
    }

    [HttpPost("add-to-circle")]
    public async Task<IActionResult> AddStudentsToCircle(
        [FromBody] AddStudentsToCircleRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out var teacherId, out var circleId))
        {
            return this.ToActionResult(GlobalResponse.BadRequest(
                "المعلم لا يدير أي حلقة, يرجى أسناد حلقة تحفيظ قرآن كريم للمعلم ليتمكن من أضافة طلاب إلى حلقته"));
        }

        if (request.StudentIds.Count == 0)
            return this.ToActionResult(GlobalResponse.BadRequest("يجب تحديد طالب واحد على الأقل"));

        var students = await db.RegisterForms
            .Where(s => request.StudentIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        await StudentCircleTransferHelper.AssignStudentsToCircleAsync(
            db, students, circleId, teacherId, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(
            new { addedCount = students.Count },
            "تم إضافة الطلاب إلى الحلقة"));
    }

    [HttpPost("{studentId:int}/remove-from-circle")]
    public async Task<IActionResult> RemoveStudentFromCircle(
        int studentId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCircleId(out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("حلقة المعلم غير محددة"));

        var student = await db.RegisterForms
            .FirstOrDefaultAsync(s => s.Id == studentId && s.QuranCircleId == circleId, cancellationToken);

        if (student is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود في الحلقة الحالية"));

        await StudentCircleTransferHelper.RemoveStudentFromCircleAsync(db, student, circleId, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(message: "تم إزالة الطالب من الحلقة"));
    }

    [HttpGet("{studentId:int}/circle-history")]
    public async Task<IActionResult> GetStudentCircleHistory(
        int studentId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCircleId(out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("حلقة المعلم غير محددة"));

        if (!await StudentCircleAccessHelper.CanReadStudentAsync(db, studentId, circleId, cancellationToken))
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var history = await db.StudentCircleEnrollments
            .AsNoTracking()
            .Where(e => e.StudentId == studentId)
            .OrderByDescending(e => e.StartDate)
            .Select(e => new StudentCircleHistoryItemDto
            {
                CircleId = e.CircleId,
                CircleName = e.QuranCircle != null ? e.QuranCircle.Name : "",
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                IsActive = e.EndDate == null
            })
            .ToListAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(history));
    }

    [HttpGet("{studentId:int}/info")]
    public async Task<IActionResult> GetStudentInfo(
        int studentId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCircleId(out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("حلقة المعلم غير محددة"));

        if (!await StudentCircleAccessHelper.CanReadStudentAsync(db, studentId, circleId, cancellationToken))
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        var teacherCircleName = await db.QuranCircles
            .AsNoTracking()
            .Where(c => c.Id == circleId)
            .Select(c => c.Name)
            .FirstOrDefaultAsync(cancellationToken) ?? "";

        var student = await db.RegisterForms
            .AsNoTracking()
            .Include(s => s.QuranCircle)
            .Include(s => s.ParentFollowup)
            .Where(s => s.Id == studentId)
            .Select(s => new StudentInfoDto
            {
                Id = s.Id,
                StudentName = s.StudentName,
                Age = s.Age,
                FatherPhone = s.FatherPhone,
                CircleName = s.QuranCircleId == circleId && s.QuranCircle != null
                    ? s.QuranCircle.Name
                    : teacherCircleName,
                RegistrationDate = s.CreatedAt,
                ImageUrl = s.ParentFollowup != null && s.ParentFollowup.photoPath != null
                    ? MediaUrlHelper.Resolve(s.ParentFollowup.photoPath)
                    : "",
                Address = s.ParentFollowup != null ? s.ParentFollowup.address ?? "" : "",
                FatherName = s.FatherName,
                StudentGender = s.StudentGender,
                BirthDate = s.Birthdate,
                MaritalStatus = s.ParentFollowup != null ? s.ParentFollowup.maritalStatus ?? "" : "",
                HealthCondition = s.ParentFollowup != null ? s.ParentFollowup.healthCondition ?? "" : "",
                HealthDetails = s.ParentFollowup != null ? s.ParentFollowup.healthDetails ?? "" : "",
                LearningDifficulties = s.ParentFollowup != null ? s.ParentFollowup.learningDifficulties ?? "" : "",
                LearningDifficultiesNotes = s.ParentFollowup != null ? s.ParentFollowup.learningDifficultiesNotes ?? "" : ""
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (student is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        return this.ToActionResult(GlobalResponse.Ok(student));
    }

    [HttpPut("{studentId:int}/plan-level")]
    public async Task<IActionResult> UpdatePlanLevel(
        int studentId,
        [FromBody] UpdateStudentPlanLevelRequestDto request,
        CancellationToken cancellationToken)
    {
        var circleIdClaim = User.FindFirstValue("circleId");
        if (!int.TryParse(circleIdClaim, out var circleId) || circleId <= 0)
            return this.ToActionResult(GlobalResponse.BadRequest("حلقة المعلم غير محددة"));

        var student = await db.RegisterForms
            .FirstOrDefaultAsync(x => x.Id == studentId && x.QuranCircleId == circleId, cancellationToken);

        if (student is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود"));

        if (request.PlanLevelId.HasValue && request.PlanLevelId.Value > 0)
        {
            var levelExists = await db.PlanLevels
                .AnyAsync(x => x.Id == request.PlanLevelId.Value, cancellationToken);

            if (!levelExists)
                return this.ToActionResult(GlobalResponse.NotFound("مستوى الخطة غير موجود"));
        }

        student.PlanLevelId = request.PlanLevelId is > 0 ? request.PlanLevelId : null;
        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(message: "تم تحديث مستوى الخطة"));
    }

    [HttpPost("{studentId:int}/toggle-special")]
    public async Task<IActionResult> ToggleSpecialStatus(
        int studentId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCircleId(out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("حلقة المعلم غير محددة"));

        var student = await db.RegisterForms
            .Include(s => s.QuranCircle)!
            .ThenInclude(c => c!.Teacher)
            .FirstOrDefaultAsync(s => s.Id == studentId && s.QuranCircleId == circleId, cancellationToken);

        if (student is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود في الحلقة الحالية"));

        student.IsSpecial = !student.IsSpecial;

        if (student.IsSpecial)
            await QueueWhatsAppIfEnabledAsync(
                student,
                WhatsappMessageEvent.StudentMarkAsSpecial,
                cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        var message = student.IsSpecial
            ? "تم تمييز الطالب كطالب مميز"
            : "تم إلغاء تمييز الطالب";

        return this.ToActionResult(GlobalResponse.Ok(
            new ToggleStudentStatusResponseDto { IsSpecial = student.IsSpecial },
            message));
    }

    [HttpPost("{studentId:int}/toggle-elite")]
    public async Task<IActionResult> ToggleEliteStatus(
        int studentId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCircleId(out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("حلقة المعلم غير محددة"));

        var student = await db.RegisterForms
            .Include(s => s.QuranCircle)!
            .ThenInclude(c => c!.Teacher)
            .FirstOrDefaultAsync(s => s.Id == studentId && s.QuranCircleId == circleId, cancellationToken);

        if (student is null)
            return this.ToActionResult(GlobalResponse.NotFound("الطالب غير موجود في الحلقة الحالية"));

        student.IsElite = !student.IsElite;

        if (student.IsElite)
            await QueueWhatsAppIfEnabledAsync(
                student,
                WhatsappMessageEvent.StudentMarkedAsElite,
                cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        var message = student.IsElite
            ? "تم تمييز الطالب كطالب نخبة"
            : "تم إلغاء تمييز الطالب كطالب نخبة";

        return this.ToActionResult(GlobalResponse.Ok(
            new ToggleStudentStatusResponseDto { IsElite = student.IsElite },
            message));
    }

    private async Task QueueWhatsAppIfEnabledAsync(
        RegisterForm student,
        WhatsappMessageEvent eventType,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(student.FatherPhone))
            return;

        var message = await WhatsappMessageHelper.GetFormattedMessageAsync(
            db,
            eventType,
            student,
            student.QuranCircle?.Name,
            student.QuranCircle?.Teacher?.Name,
            cancellationToken);

        if (string.IsNullOrEmpty(message))
            return;

        var isGirl = bool.TryParse(User.FindFirstValue("isGirlTeacher"), out var girl) && girl;

        db.WhatsappTempTables.Add(new WhatsappTempTable
        {
            mobile = student.FatherPhone,
            IsGirl = isGirl ? 1 : 0,
            message = message
        });
    }

    private bool TryGetTeacherContext(out int teacherId, out int circleId)
    {
        teacherId = 0;
        circleId = 0;
        var idClaim = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        var circleIdClaim = User.FindFirstValue("circleId");
        return int.TryParse(idClaim, out teacherId) && teacherId > 0
               && int.TryParse(circleIdClaim, out circleId) && circleId > 0;
    }

    private bool TryGetCircleId(out int circleId)
    {
        circleId = 0;
        var circleIdClaim = User.FindFirstValue("circleId");
        return int.TryParse(circleIdClaim, out circleId) && circleId > 0;
    }
}
