using System.Security.Claims;
using MasgedParentMobileAPI.Services;
using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Entities;
using MasgedTeacherMobileAPI.Enums;
using MasgedTeacherMobileAPI.Extensions;
using MasgedTeacherMobileAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public partial class StudentsAttendanceController(
    AppDbContext db,
    StudentQrTokenService qrTokenService,
    IWorkDayService workDayService) : ControllerBase
{
    [HttpPost("mark-attendance")]
    public async Task<IActionResult> SaveAttendanceForMultipleStudents(
        [FromBody] SaveAttendanceRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out var teacherId, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (!await workDayService.IsWorkDayAsync(KuwaitTime.Today, cancellationToken))
            return this.ToActionResult(GlobalResponse.BadRequest(WorkDayGuard.NonWorkDayMessage));

        if (!await MosqueLocationHelper.IsWithinMosqueRadiusAsync(
                db, teacherId, request.Latitude, request.Longitude, cancellationToken))
        {
            return this.ToActionResult(GlobalResponse.BadRequest(
                MosqueLocationHelper.OutsideMosqueMessageForStudentAttendance()));
        }

        if (request.StudentIds.Count == 0)
            return this.ToActionResult(GlobalResponse.BadRequest("يجب تحديد طالب واحد على الأقل"));

        var attendanceDateTime = DateTime.TryParse(request.AttendanceDate, out var parsedDate)
            ? parsedDate.Date
            : KuwaitTime.Today;

        var allStudentsInCircle = await db.RegisterForms
            .Where(s => s.QuranCircleId == circleId)
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        var alreadyAttendedStudents = new List<int>();
        var validStudentsToAttend = new List<int>();

        foreach (var studentId in request.StudentIds)
        {
            var existingAttendanceForStudent = await db.CircleAttendances
                .FirstOrDefaultAsync(
                    a => a.StudentId == studentId
                         && a.AttendanceDateTime == attendanceDateTime
                         && a.IsHere,
                    cancellationToken);

            if (existingAttendanceForStudent is not null)
                alreadyAttendedStudents.Add(studentId);
            else
                validStudentsToAttend.Add(studentId);
        }

        foreach (var studentId in validStudentsToAttend)
        {
            var existingRecord = await db.CircleAttendances
                .FirstOrDefaultAsync(
                    a => a.StudentId == studentId && a.AttendanceDateTime == attendanceDateTime,
                    cancellationToken);

            if (existingRecord is not null)
                db.CircleAttendances.Remove(existingRecord);

            var student = await db.RegisterForms
                .Include(s => s.QuranCircle)!
                .ThenInclude(c => c!.Teacher)
                .FirstOrDefaultAsync(x => x.Id == studentId, cancellationToken);

            if (student is null)
                continue;

            var formattedMessage = await WhatsappMessageHelper.GetFormattedMessageAsync(
                db,
                WhatsappMessageEvent.StudentAttendance,
                student,
                student.QuranCircle?.Name,
                student.QuranCircle?.Teacher?.Name,
                cancellationToken);

            if (!string.IsNullOrEmpty(formattedMessage))
            {
                db.WhatsappTempTables.Add(new WhatsappTempTable
                {
                    mobile = student.FatherPhone,
                    IsGirl = 0,
                    message = formattedMessage
                });
            }

            db.CircleAttendances.Add(new CircleAttendance
            {
                StudentId = studentId,
                TeacherId = teacherId,
                AttendanceDateTime = attendanceDateTime,
                IsHere = true,
                CircleId = circleId
            });
        }

        await db.SaveChangesAsync(cancellationToken);

        var currentAttendedCount = await db.CircleAttendances
            .CountAsync(
                a => a.AttendanceDateTime == attendanceDateTime
                       && allStudentsInCircle.Contains(a.StudentId)
                       && a.IsHere,
                cancellationToken);

        var currentAbsentCount = allStudentsInCircle.Count - currentAttendedCount;

        string message;
        if (alreadyAttendedStudents.Count == 0)
        {
            message = $"تم حفظ التحضير بنجاح. إجمالي الحاضرين: {currentAttendedCount}, الغائبين: {currentAbsentCount}";
        }
        else if (validStudentsToAttend.Count == 0)
        {
            message = $"جميع الطلاب المحددين ({alreadyAttendedStudents.Count} طالب) محضرين بالفعل اليوم. إجمالي الحاضرين: {currentAttendedCount}";
        }
        else
        {
            message = $"تم حفظ التحضير لـ {validStudentsToAttend.Count} طالب جديد. {alreadyAttendedStudents.Count} طالب كانوا محضرين بالفعل. إجمالي الحاضرين: {currentAttendedCount}";
        }

        return this.ToActionResult(GlobalResponse.Ok(new
        {
            message,
            newAttendanceCount = validStudentsToAttend.Count,
            alreadyAttendedCount = alreadyAttendedStudents.Count,
            totalAttendedCount = currentAttendedCount
        }));
    }

    [HttpPost("mark-departure")]
    public async Task<IActionResult> SaveDepartureForMultipleStudents(
        [FromBody] StudentIdsRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out var teacherId, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (!await workDayService.IsWorkDayAsync(KuwaitTime.Today, cancellationToken))
            return this.ToActionResult(GlobalResponse.BadRequest(WorkDayGuard.NonWorkDayMessage));

        if (!await MosqueLocationHelper.IsWithinMosqueRadiusAsync(
                db, teacherId, request.Latitude, request.Longitude, cancellationToken))
        {
            return this.ToActionResult(GlobalResponse.BadRequest(
                MosqueLocationHelper.OutsideMosqueMessageForStudentAttendance(isDeparture: true)));
        }

        if (request.StudentIds.Count == 0)
            return this.ToActionResult(GlobalResponse.BadRequest("يجب تحديد طالب واحد على الأقل"));

        var currentDateTime = KuwaitTime.Now;

        if (currentDateTime < new DateTime(1753, 1, 1) || currentDateTime > new DateTime(9999, 12, 31))
            return this.ToActionResult(GlobalResponse.BadRequest("Invalid date range"));

        var today = currentDateTime.Date;
        var tomorrow = today.AddDays(1);

        var validStudentsToDepart = new List<int>();
        var notAttendedStudents = new List<int>();
        var alreadyDepartedStudents = new List<int>();

        foreach (var studentId in request.StudentIds)
        {
            var attendanceToday = await db.CircleAttendances
                .FirstOrDefaultAsync(
                    a => a.StudentId == studentId
                         && a.AttendanceDateTime >= today
                         && a.AttendanceDateTime < tomorrow
                         && a.IsHere,
                    cancellationToken);

            if (attendanceToday is null)
            {
                notAttendedStudents.Add(studentId);
                continue;
            }

            var existingDeparture = await db.CircleAttendances
                .FirstOrDefaultAsync(
                    d => d.StudentId == studentId
                         && d.DepartureDate.HasValue
                         && d.DepartureDate >= today
                         && d.DepartureDate < tomorrow,
                    cancellationToken);

            if (existingDeparture is not null)
                alreadyDepartedStudents.Add(studentId);
            else
                validStudentsToDepart.Add(studentId);
        }

        foreach (var studentId in validStudentsToDepart)
        {
            var attendanceRecord = await db.CircleAttendances
                .FirstOrDefaultAsync(
                    a => a.StudentId == studentId
                         && a.AttendanceDateTime >= today
                         && a.AttendanceDateTime < tomorrow
                         && a.IsHere,
                    cancellationToken);

            if (attendanceRecord is not null)
                attendanceRecord.DepartureDate = currentDateTime;
        }

        if (validStudentsToDepart.Count > 0)
        {
            var allStudentIdsInCircle = await db.RegisterForms
                .Where(x => x.QuranCircleId == circleId)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            var departedStudentsToday = await db.CircleAttendances
                .CountAsync(
                    ca => allStudentIdsInCircle.Contains(ca.StudentId)
                          && ca.DepartureDate.HasValue
                          && ca.DepartureDate >= today
                          && ca.DepartureDate < tomorrow,
                    cancellationToken);

            var presentStudentsToday = await db.CircleAttendances
                .CountAsync(
                    ca => allStudentIdsInCircle.Contains(ca.StudentId)
                          && ca.AttendanceDateTime >= today
                          && ca.AttendanceDateTime < tomorrow
                          && ca.IsHere,
                    cancellationToken);

            if (departedStudentsToday >= presentStudentsToday)
            {
                var absentStudents = await db.RegisterForms
                    .Include(s => s.QuranCircle)!
                    .ThenInclude(c => c!.Teacher)
                    .Where(s => s.QuranCircleId == circleId
                                && !db.CircleAttendances.Any(ca =>
                                    ca.StudentId == s.Id
                                    && ca.AttendanceDateTime >= today
                                    && ca.AttendanceDateTime < tomorrow
                                    && ca.IsHere))
                    .ToListAsync(cancellationToken);

                foreach (var absentStudent in absentStudents)
                {
                    if (string.IsNullOrEmpty(absentStudent.FatherPhone))
                        continue;

                    var formattedMessage = await WhatsappMessageHelper.GetFormattedMessageAsync(
                        db,
                        WhatsappMessageEvent.StudentAbsence,
                        absentStudent,
                        absentStudent.QuranCircle?.Name,
                        absentStudent.QuranCircle?.Teacher?.Name,
                        cancellationToken);

                    if (!string.IsNullOrEmpty(formattedMessage))
                    {
                        db.WhatsappTempTables.Add(new WhatsappTempTable
                        {
                            mobile = absentStudent.FatherPhone,
                            IsGirl = 0,
                            message = formattedMessage
                        });
                    }
                }
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        var totalIssues = notAttendedStudents.Count + alreadyDepartedStudents.Count;

        if (totalIssues == 0)
        {
            return this.ToActionResult(GlobalResponse.Ok(new
            {
                message = $"تم حفظ الانصراف بنجاح لـ {validStudentsToDepart.Count} طالب في الساعة {currentDateTime:HH:mm}",
                validDepartureCount = validStudentsToDepart.Count,
                notAttendedCount = notAttendedStudents.Count,
                alreadyDepartedCount = alreadyDepartedStudents.Count
            }));
        }

        if (validStudentsToDepart.Count == 0)
        {
            var errorParts = new List<string>();
            if (notAttendedStudents.Count > 0)
                errorParts.Add($"{notAttendedStudents.Count} طالب لم يتم تسجيل حضورهم");
            if (alreadyDepartedStudents.Count > 0)
                errorParts.Add($"{alreadyDepartedStudents.Count} طالب منصرفين بالفعل");

            return this.ToActionResult(GlobalResponse.BadRequest(
                $"لا يمكن انصراف الطلاب المحددين: {string.Join("، ", errorParts)}"));
        }

        var messageParts = new List<string> { $"تم حفظ الانصراف لـ {validStudentsToDepart.Count} طالب" };

        if (notAttendedStudents.Count > 0)
            messageParts.Add($"{notAttendedStudents.Count} طالب لم يتم انصرافهم لعدم تسجيل حضورهم");
        if (alreadyDepartedStudents.Count > 0)
            messageParts.Add($"{alreadyDepartedStudents.Count} طالب كانوا منصرفين بالفعل");

        return this.ToActionResult(GlobalResponse.Ok(new
        {
            message = string.Join(". ", messageParts),
            validDepartureCount = validStudentsToDepart.Count,
            notAttendedCount = notAttendedStudents.Count,
            alreadyDepartedCount = alreadyDepartedStudents.Count
        }));
    }

    [HttpPost("undo-attendance/{studentId:int}")]
    public async Task<IActionResult> UndoAttendance(
        int studentId,
        [FromBody] CoordinatesRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out var teacherId, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (!await workDayService.IsWorkDayAsync(KuwaitTime.Today, cancellationToken))
            return this.ToActionResult(GlobalResponse.BadRequest(WorkDayGuard.NonWorkDayMessage));

        if (!await MosqueLocationHelper.IsWithinMosqueRadiusAsync(
                db, teacherId, request.Latitude, request.Longitude, cancellationToken))
        {
            return this.ToActionResult(GlobalResponse.BadRequest(
                MosqueLocationHelper.OutsideMosqueMessageForStudentAttendance()));
        }

        var today = KuwaitTime.Today;

        var attendanceRecord = await db.CircleAttendances
            .FirstOrDefaultAsync(
                a => a.StudentId == studentId
                     && a.AttendanceDateTime == today
                     && a.CircleId == circleId,
                cancellationToken);

        if (attendanceRecord is null)
        {
            return this.ToActionResult(GlobalResponse.BadRequest(
                "لم يتم العثور على تسجيل حضور لهذا الطالب اليوم"));
        }

        db.CircleAttendances.Remove(attendanceRecord);

        var student = await db.RegisterForms
            .Include(s => s.QuranCircle)!
            .ThenInclude(c => c!.Teacher)
            .FirstOrDefaultAsync(x => x.Id == studentId, cancellationToken);

        if (student is not null)
        {
            var formattedMessage = await WhatsappMessageHelper.GetFormattedMessageAsync(
                db,
                WhatsappMessageEvent.StudentAttendance,
                student,
                student.QuranCircle?.Name,
                student.QuranCircle?.Teacher?.Name,
                cancellationToken);

            if (!string.IsNullOrEmpty(formattedMessage))
            {
                db.WhatsappTempTables.Add(new WhatsappTempTable
                {
                    mobile = student.FatherPhone,
                    IsGirl = 0,
                    message = formattedMessage
                });
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(message: "تم إلغاء تسجيل الحضور بنجاح"));
    }

    [HttpPost("undo-departure/{studentId:int}")]
    public async Task<IActionResult> UndoDeparture(
        int studentId,
        [FromBody] CoordinatesRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherContext(out var teacherId, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (!await workDayService.IsWorkDayAsync(KuwaitTime.Today, cancellationToken))
            return this.ToActionResult(GlobalResponse.BadRequest(WorkDayGuard.NonWorkDayMessage));

        if (!await MosqueLocationHelper.IsWithinMosqueRadiusAsync(
                db, teacherId, request.Latitude, request.Longitude, cancellationToken))
        {
            return this.ToActionResult(GlobalResponse.BadRequest(
                MosqueLocationHelper.OutsideMosqueMessageForStudentAttendance(isDeparture: true)));
        }

        var today = KuwaitTime.Today;
        var tomorrow = today.AddDays(1);

        var departureRecord = await db.CircleAttendances
            .FirstOrDefaultAsync(
                d => d.StudentId == studentId
                     && d.CircleId == circleId
                     && d.DepartureDate.HasValue
                     && d.DepartureDate >= today
                     && d.DepartureDate < tomorrow,
                cancellationToken);

        if (departureRecord is null)
        {
            return this.ToActionResult(GlobalResponse.BadRequest(
                "لم يتم العثور على تسجيل انصراف لهذا الطالب اليوم"));
        }

        departureRecord.DepartureDate = null;
        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(message: "تم إلغاء تسجيل الانصراف بنجاح"));
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
}
