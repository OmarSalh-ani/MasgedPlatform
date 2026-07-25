using System.Globalization;
using System.Security.Claims;
using MasgedParentMobileAPI.Services;
using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Entities;
using MasgedTeacherMobileAPI.Extensions;
using MasgedTeacherMobileAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TeacherAttendanceController(AppDbContext db, IWorkDayService workDayService) : ControllerBase
{
    private const double MaxDistanceMeters = MosqueLocationHelper.MaxDistanceMeters;
    private static readonly string[] DateFormats = { "yyyy-MM-dd", "MM/dd/yyyy", "dd/MM/yyyy", "yyyy/MM/dd" };
    private const int MaxLogRangeDays = 366;

    [HttpGet("status")]
    public async Task<IActionResult> GetTeacherAttendanceStatus(CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var hasFingerprint = await HasFingerprintRegisteredAsync(teacherId, cancellationToken);
        var today = KuwaitTime.Today;

        if (!await workDayService.IsWorkDayAsync(today, cancellationToken))
        {
            return this.ToActionResult(GlobalResponse.Ok(new TeacherAttendanceStatusDto
            {
                Status = "vacation",
                Message = "اليوم إجازة",
                HasFingerprintRegistered = hasFingerprint
            }));
        }

        var tomorrow = today.AddDays(1);

        var attendance = await db.TeacherAttendances
            .AsNoTracking()
            .FirstOrDefaultAsync(
                a => a.TeacherId == teacherId
                     && a.AttendanceDateTime >= today
                     && a.AttendanceDateTime < tomorrow,
                cancellationToken);

        if (attendance is null)
        {
            return this.ToActionResult(GlobalResponse.Ok(new TeacherAttendanceStatusDto
            {
                Status = "not_attended",
                Message = "لم يتم تسجيل الحضور",
                HasFingerprintRegistered = hasFingerprint
            }));
        }

        if (attendance.DepartureDateTime.HasValue)
        {
            return this.ToActionResult(GlobalResponse.Ok(new TeacherAttendanceStatusDto
            {
                Status = "departed",
                Message = "تم تسجيل الانصراف",
                AttendanceTime = attendance.AttendanceDateTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                DepartureTime = attendance.DepartureDateTime.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                HasFingerprintRegistered = hasFingerprint
            }));
        }

        return this.ToActionResult(GlobalResponse.Ok(new TeacherAttendanceStatusDto
        {
            Status = "attended",
            Message = "تم تسجيل الحضور",
            AttendanceTime = attendance.AttendanceDateTime.ToString("yyyy-MM-ddTHH:mm:ss"),
            HasFingerprintRegistered = hasFingerprint
        }));
    }

    [HttpGet("fingerprint-status")]
    public async Task<IActionResult> GetFingerprintStatus(CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var hasFingerprint = await HasFingerprintRegisteredAsync(teacherId, cancellationToken);
        return this.ToActionResult(GlobalResponse.Ok(new TeacherFingerprintStatusDto
        {
            HasFingerprintRegistered = hasFingerprint
        }));
    }

    [HttpPost("register-fingerprint")]
    public async Task<IActionResult> RegisterFingerprint(
        [FromBody] RegisterTeacherFingerprintRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var hash = request.FingerprintHash.Trim().ToLowerInvariant();
        if (!TeacherFingerprintHashHelper.IsValidHashFormat(hash))
        {
            return this.ToActionResult(GlobalResponse.BadRequest("صيغة بصمة الحضور غير صالحة"));
        }

        var teacher = await db.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId, cancellationToken);
        if (teacher is null)
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (!string.IsNullOrEmpty(teacher.AttendanceFingerprintHash))
        {
            return this.ToActionResult(GlobalResponse.BadRequest(
                "تم تسجيل بصمة الحضور مسبقاً. تواصل مع الإدارة لإعادة التسجيل."));
        }

        teacher.AttendanceFingerprintHash = hash;
        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(new
        {
            message = "تم تسجيل بصمة الحضور بنجاح",
            hasFingerprintRegistered = true
        }));
    }

    [HttpPost("re-register-fingerprint")]
    public async Task<IActionResult> ReRegisterFingerprint(
        [FromBody] ReRegisterTeacherFingerprintRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return this.ToActionResult(GlobalResponse.BadRequest("يرجى إدخال كلمة المرور"));
        }

        var hash = request.FingerprintHash.Trim().ToLowerInvariant();
        if (!TeacherFingerprintHashHelper.IsValidHashFormat(hash))
        {
            return this.ToActionResult(GlobalResponse.BadRequest("صيغة بصمة الحضور غير صالحة"));
        }

        var teacher = await db.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId, cancellationToken);
        if (teacher is null)
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (teacher.Password != request.Password.Trim())
        {
            return this.ToActionResult(GlobalResponse.Unauthorized("كلمة المرور غير صحيحة"));
        }

        teacher.AttendanceFingerprintHash = hash;
        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(new
        {
            message = "تم تسجيل بصمة الحضور على هذا الجهاز بنجاح",
            hasFingerprintRegistered = true
        }));
    }

    [HttpPost("mark-attendance")]
    public async Task<IActionResult> MarkTeacherAttendance(
        [FromBody] LocationRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var fingerprintError = await ValidateFingerprintAsync(
            teacherId,
            request.FingerprintHash,
            cancellationToken);
        if (fingerprintError is not null)
            return this.ToActionResult(GlobalResponse.BadRequest(fingerprintError));

        if (!await workDayService.IsWorkDayAsync(KuwaitTime.Today, cancellationToken))
            return this.ToActionResult(GlobalResponse.BadRequest(WorkDayGuard.NonWorkDayMessage));

        if (!await MosqueLocationHelper.IsWithinMosqueRadiusAsync(
                db, teacherId, request.Latitude, request.Longitude, cancellationToken))
        {
            return this.ToActionResult(GlobalResponse.BadRequest(
                "يجب أن تكون داخل نطاق المسجد المحدد لك لتسجيل الحضور. المسافة المسموحة: 200 متر"));
        }

        var today = KuwaitTime.Today;
        var tomorrow = today.AddDays(1);

        await AutoCloseStaleOpenAttendancesAsync(teacherId, today, cancellationToken);

        var existingAttendance = await db.TeacherAttendances
            .FirstOrDefaultAsync(
                a => a.TeacherId == teacherId
                     && a.AttendanceDateTime >= today
                     && a.AttendanceDateTime < tomorrow,
                cancellationToken);

        if (existingAttendance is not null)
            return this.ToActionResult(GlobalResponse.BadRequest("تم تسجيل الحضور بالفعل اليوم"));

        var attendance = new TeacherAttendance
        {
            TeacherId = teacherId,
            AttendanceDateTime = KuwaitTime.Now,
            AttendanceLatitude = request.Latitude,
            AttendanceLongitude = request.Longitude
        };

        db.TeacherAttendances.Add(attendance);
        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(new
        {
            message = $"تم تسجيل الحضور بنجاح في الساعة {KuwaitTime.Now.ToString("hh:mm tt", CultureInfo.InvariantCulture)}",
            attendanceTime = attendance.AttendanceDateTime.ToString("yyyy-MM-ddTHH:mm:ss")
        }));
    }

    [HttpGet("proximity")]
    public async Task<IActionResult> GetMosqueProximity(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var proximity = await BuildMosqueProximityAsync(
            teacherId,
            latitude,
            longitude,
            cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(proximity));
    }

    [HttpPost("mark-departure")]
    public async Task<IActionResult> MarkTeacherDeparture(
        [FromBody] LocationRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var fingerprintError = await ValidateFingerprintAsync(
            teacherId,
            request.FingerprintHash,
            cancellationToken);
        if (fingerprintError is not null)
            return this.ToActionResult(GlobalResponse.BadRequest(fingerprintError));

        if (!await workDayService.IsWorkDayAsync(KuwaitTime.Today, cancellationToken))
            return this.ToActionResult(GlobalResponse.BadRequest(WorkDayGuard.NonWorkDayMessage));

        if (!await MosqueLocationHelper.IsWithinMosqueRadiusAsync(
                db, teacherId, request.Latitude, request.Longitude, cancellationToken))
        {
            return this.ToActionResult(GlobalResponse.BadRequest(
                "يجب أن تكون داخل نطاق المسجد المحدد لك لتسجيل الانصراف. المسافة المسموحة: 200 متر"));
        }

        var today = KuwaitTime.Today;
        var tomorrow = today.AddDays(1);

        var attendance = await db.TeacherAttendances
            .FirstOrDefaultAsync(
                a => a.TeacherId == teacherId
                     && a.AttendanceDateTime >= today
                     && a.AttendanceDateTime < tomorrow,
                cancellationToken);

        if (attendance is null)
            return this.ToActionResult(GlobalResponse.BadRequest("يجب تسجيل الحضور أولاً قبل تسجيل الانصراف"));

        if (attendance.DepartureDateTime.HasValue)
            return this.ToActionResult(GlobalResponse.BadRequest("تم تسجيل الانصراف بالفعل اليوم"));

        attendance.DepartureDateTime = KuwaitTime.Now;
        attendance.DepartureLatitude = request.Latitude;
        attendance.DepartureLongitude = request.Longitude;

        await db.SaveChangesAsync(cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(new
        {
            message = $"تم تسجيل الانصراف بنجاح في الساعة {KuwaitTime.Now.ToString("hh:mm tt", CultureInfo.InvariantCulture)}",
            departureTime = attendance.DepartureDateTime.Value.ToString("yyyy-MM-ddTHH:mm:ss")
        }));
    }

    [HttpGet("log")]
    public async Task<IActionResult> GetTeacherAttendanceLog(
        [FromQuery] string fromDate,
        [FromQuery] string toDate,
        CancellationToken cancellationToken)
    {
        if (!TryGetTeacherId(out var teacherId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        if (!TryParseDateRange(fromDate, toDate, out var fromDateTime, out var toDateTime, out var dateError))
            return this.ToActionResult(GlobalResponse.BadRequest(dateError));

        if (fromDateTime > toDateTime)
            return this.ToActionResult(GlobalResponse.BadRequest("تاريخ البداية يجب أن يكون قبل تاريخ النهاية"));

        if ((toDateTime - fromDateTime).TotalDays > MaxLogRangeDays)
            return this.ToActionResult(GlobalResponse.BadRequest($"الحد الأقصى للفترة هو {MaxLogRangeDays} يوماً"));

        var rangeEndExclusive = toDateTime.AddDays(1);
        var arabicCulture = CultureInfo.GetCultureInfo("ar-KW");

        var attendances = await db.TeacherAttendances
            .AsNoTracking()
            .Where(a => a.TeacherId == teacherId
                        && a.AttendanceDateTime >= fromDateTime
                        && a.AttendanceDateTime < rangeEndExclusive)
            .OrderByDescending(a => a.AttendanceDateTime)
            .ToListAsync(cancellationToken);

        var workDayNumbers = await workDayService.GetWorkDayNumbersAsync(cancellationToken);
        var workDaySet = workDayNumbers.ToHashSet();

        var records = attendances.Select(a =>
        {
            var isWorkDay = workDaySet.Contains((int)a.AttendanceDateTime.DayOfWeek);
            if (!isWorkDay)
            {
                return new TeacherAttendanceLogItemDto
                {
                    Id = a.Id,
                    Date = a.AttendanceDateTime.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture),
                    Day = arabicCulture.DateTimeFormat.GetDayName(a.AttendanceDateTime.DayOfWeek),
                    StatusKey = AttendanceHelper.VacationStatusKey,
                    Status = AttendanceHelper.VacationStatusAr,
                    AttendanceTime = a.AttendanceDateTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    DepartureTime = null
                };
            }

            var hasDeparture = a.DepartureDateTime.HasValue;
            return new TeacherAttendanceLogItemDto
            {
                Id = a.Id,
                Date = a.AttendanceDateTime.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture),
                Day = arabicCulture.DateTimeFormat.GetDayName(a.AttendanceDateTime.DayOfWeek),
                StatusKey = hasDeparture ? "departed" : "attended",
                Status = hasDeparture ? "تم تسجيل الانصراف" : "تم تسجيل الحضور",
                AttendanceTime = a.AttendanceDateTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                DepartureTime = hasDeparture
                    ? a.DepartureDateTime!.Value.ToString("yyyy-MM-ddTHH:mm:ss")
                    : null
            };
        }).ToList();

        var summary = new TeacherAttendanceLogSummaryDto
        {
            TotalRecords = records.Count,
            TotalWithDeparture = records.Count(r => r.StatusKey == "departed"),
            TotalAttendanceOnly = records.Count(r => r.StatusKey == "attended")
        };

        return this.ToActionResult(GlobalResponse.Ok(new TeacherAttendanceLogResponseDto
        {
            FromDate = fromDateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            ToDate = toDateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            Summary = summary,
            Records = records
        }));
    }

    private bool TryGetTeacherId(out int teacherId)
    {
        teacherId = 0;
        var idClaim = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idClaim, out teacherId) && teacherId > 0;
    }

    private async Task<bool> HasFingerprintRegisteredAsync(
        int teacherId,
        CancellationToken cancellationToken) =>
        await db.Teachers
            .AsNoTracking()
            .Where(t => t.Id == teacherId && t.AttendanceFingerprintHash != null && t.AttendanceFingerprintHash != "")
            .AnyAsync(cancellationToken);

    private async Task<string?> ValidateFingerprintAsync(
        int teacherId,
        string? fingerprintHash,
        CancellationToken cancellationToken)
    {
        var hash = fingerprintHash?.Trim().ToLowerInvariant();
        if (!TeacherFingerprintHashHelper.IsValidHashFormat(hash))
            return "مطلوب التحقق بالبصمة قبل تسجيل الحضور أو الانصراف";

        var teacher = await db.Teachers
            .AsNoTracking()
            .Where(t => t.Id == teacherId)
            .Select(t => t.AttendanceFingerprintHash)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrEmpty(teacher))
            return "يجب تسجيل بصمة الحضور أولاً من التطبيق";

        if (!TeacherFingerprintHashHelper.HashesMatch(teacher, hash!))
            return "فشل التحقق من البصمة. حاول مرة أخرى";

        return null;
    }

    /// Closes any prior-day attendance rows that were never checked out (23:59 same day).
    private async Task AutoCloseStaleOpenAttendancesAsync(
        int teacherId,
        DateTime today,
        CancellationToken cancellationToken)
    {
        var openPastAttendances = await db.TeacherAttendances
            .Where(a => a.TeacherId == teacherId
                        && a.AttendanceDateTime < today
                        && a.DepartureDateTime == null)
            .ToListAsync(cancellationToken);

        if (openPastAttendances.Count == 0)
            return;

        foreach (var open in openPastAttendances)
        {
            open.DepartureDateTime = KuwaitTime.EndOfDay(open.AttendanceDateTime);
            open.DepartureLatitude ??= open.AttendanceLatitude;
            open.DepartureLongitude ??= open.AttendanceLongitude;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task<MosqueProximityDto> BuildMosqueProximityAsync(
        int teacherId,
        double latitude,
        double longitude,
        CancellationToken cancellationToken)
    {
        var nearestDistance = await MosqueLocationHelper.GetNearestMosqueDistanceMetersAsync(
            db,
            teacherId,
            latitude,
            longitude,
            cancellationToken);

        if (nearestDistance is null)
        {
            return new MosqueProximityDto
            {
                HasMosqueLocation = false,
                Message = "لم يتم تحديد موقع المسجد لحساب المسافة",
                MaxAllowedMeters = MaxDistanceMeters
            };
        }

        var distanceMeters = nearestDistance.Value;
        var display = MosqueLocationHelper.FormatDistanceArabic(distanceMeters);
        var isWithin = distanceMeters <= MaxDistanceMeters;

        return new MosqueProximityDto
        {
            HasMosqueLocation = true,
            DistanceMeters = Math.Round(distanceMeters, 1),
            DistanceDisplay = display,
            IsWithinRadius = isWithin,
            MaxAllowedMeters = MaxDistanceMeters,
            Message = isWithin
                ? $"أنت داخل نطاق المسجد ({display})"
                : $"أنت بعيد {display} عن المسجد"
        };
    }

    private static bool TryParseDateRange(
        string fromDate,
        string toDate,
        out DateTime fromDateTime,
        out DateTime toDateTime,
        out string error)
    {
        fromDateTime = default;
        toDateTime = default;
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(fromDate) || string.IsNullOrWhiteSpace(toDate))
        {
            error = "تواريخ غير صالحة";
            return false;
        }

        if (!DateTime.TryParseExact(fromDate, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateTime) ||
            !DateTime.TryParseExact(toDate, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out toDateTime))
        {
            if (!DateTime.TryParse(fromDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateTime) ||
                !DateTime.TryParse(toDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out toDateTime))
            {
                error = $"تنسيق التاريخ غير صالح. التاريخ المستلم: من '{fromDate}' إلى '{toDate}'. يرجى استخدام تنسيق صالح.";
                return false;
            }
        }

        fromDateTime = fromDateTime.Date;
        toDateTime = toDateTime.Date;
        return true;
    }
}
