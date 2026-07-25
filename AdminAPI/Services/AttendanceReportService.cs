using AdminAPI.DTOs.AttendanceReport;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class AttendanceReportService(
    Data.AdminDbContext db,
    IAttendanceReportRepository repository,
    ICurrentUserContext currentUser,
    IWorkDayService workDayService,
    IOptions<PublicSiteOptions> publicSiteOptions) : IAttendanceReportService
{
    public Task<AttendanceReportFilterOptionsDto> GetFilterOptionsAsync(
        CancellationToken cancellationToken = default) =>
        repository.GetFilterOptionsAsync(currentUser.IsGirlTeacher, cancellationToken);

    public async Task<AttendanceReportListResponseDto> GetReportAsync(
        DateTime fromDate,
        DateTime toDate,
        int? circleId,
        int? teacherId,
        string attendanceFilter,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        ValidateDateRange(fromDate, toDate, 365);

        var query = repository.BuildStudentQuery(currentUser.IsGirlTeacher, circleId, teacherId);
        var workDayNumbers = await workDayService.GetWorkDayNumbersAsync(cancellationToken);
        var rows = await AttendanceReportRowBuilder.BuildRowsAsync(
            query,
            fromDate,
            toDate,
            attendanceFilter,
            workDayNumbers,
            cancellationToken);

        var summary = AttendanceReportRowBuilder.BuildSummary(rows);
        var page = pageNumber < 1 ? 1 : pageNumber;
        var size = pageSize < 1 ? 50 : pageSize;
        var totalCount = rows.Count;
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)size);
        var items = rows.Skip((page - 1) * size).Take(size).ToList();

        return new AttendanceReportListResponseDto
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = size,
            TotalPages = totalPages,
            Summary = summary,
        };
    }

    public async Task<byte[]> ExportReportExcelAsync(
        DateTime fromDate,
        DateTime toDate,
        int? circleId,
        int? teacherId,
        string attendanceFilter,
        CancellationToken cancellationToken = default)
    {
        ValidateDateRange(fromDate, toDate, 365);

        var query = repository.BuildStudentQuery(currentUser.IsGirlTeacher, circleId, teacherId);
        var workDayNumbers = await workDayService.GetWorkDayNumbersAsync(cancellationToken);
        var rows = await AttendanceReportRowBuilder.BuildRowsAsync(
            query,
            fromDate,
            toDate,
            attendanceFilter,
            workDayNumbers,
            cancellationToken);

        return AttendanceReportExcelExporter.Build(rows);
    }

    public async Task<string> SendWhatsappAsync(
        SendAttendanceWhatsappRequestDto request,
        string? base64Image,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();

        if (request.StudentIds.Count == 0)
            throw new ValidationException("يرجى تحديد السجلات المراد إرسال الرسائل لها");

        if (string.IsNullOrWhiteSpace(request.Message))
            throw new ValidationException("يرجى كتابة الرسالة أولاً");

        var students = await db.RegisterForms
            .AsNoTracking()
            .Where(x => request.StudentIds.Contains(x.Id))
            .Select(x => new
            {
                x.Id,
                x.StudentName,
                x.FatherName,
                x.FatherPhone,
                CircleName = x.QuranCircle!.Name,
            })
            .ToListAsync(cancellationToken);

        var baseUrl = publicSiteOptions.Value.BaseUrl.TrimEnd('/');
        var isGirl = currentUser.IsGirlTeacher ? 1 : 0;

        foreach (var student in students)
        {
            var personalizedMessage = request.Message
                .Replace("{أسم الطالب}", student.StudentName)
                .Replace("{أسم الأب}", student.FatherName ?? string.Empty)
                .Replace("{أسم الحلقة}", student.CircleName)
                .Replace("{الرابط}", $"{baseUrl}/parents-followup?id={student.Id}");

            db.WhatsappTempTables.Add(new WhatsappTempTable
            {
                Image = base64Image,
                Message = personalizedMessage,
                Mobile = student.FatherPhone,
                IsGirl = isGirl,
            });
        }

        await db.SaveChangesAsync(cancellationToken);
        return $"تم إرسال {students.Count} رسالة بنجاح";
    }

    public async Task<SaveDepartureResultDto> SaveDeparturesAsync(
        IReadOnlyList<SaveDepartureItemDto> items,
        CancellationToken cancellationToken = default)
    {
        if (items.Count == 0)
            throw new ValidationException("يرجى تحديد السجلات المراد تسجيل انصرافها");

        var now = KuwaitTime.Now;
        var savedCount = 0;
        var skippedCount = 0;
        var errorCount = 0;

        foreach (var item in items)
        {
            if (!DateTime.TryParse(item.Date, out var targetDate))
            {
                errorCount++;
                continue;
            }

            var targetDateOnly = targetDate.Date;
            var attendance = await db.CircleAttendances
                .Where(a => a.StudentId == item.StudentId
                    && a.AttendanceDateTime.Date == targetDateOnly
                    && a.IsHere)
                .OrderByDescending(a => a.AttendanceDateTime)
                .FirstOrDefaultAsync(cancellationToken);

            if (attendance != null)
            {
                if (attendance.DepartureDate == null)
                {
                    attendance.DepartureDate = targetDateOnly < KuwaitTime.Today
                        ? targetDateOnly.AddHours(now.Hour).AddMinutes(now.Minute)
                        : now;
                    savedCount++;
                }
                else
                {
                    skippedCount++;
                }

                continue;
            }

            var studentExists = await db.RegisterForms
                .AnyAsync(s => s.Id == item.StudentId, cancellationToken);

            if (!studentExists)
            {
                errorCount++;
                continue;
            }

            db.CircleAttendances.Add(new CircleAttendance
            {
                StudentId = item.StudentId,
                AttendanceDateTime = targetDateOnly,
                IsHere = true,
                DepartureDate = targetDateOnly < KuwaitTime.Today
                    ? targetDateOnly.AddHours(now.Hour).AddMinutes(now.Minute)
                    : now,
            });
            savedCount++;
        }

        await db.SaveChangesAsync(cancellationToken);

        var message = $"تم تسجيل انصراف {savedCount} طالب بنجاح";
        if (skippedCount > 0)
            message += $" ({skippedCount} طالب لديهم انصراف مسجل مسبقاً)";
        if (errorCount > 0)
            message += $" ({errorCount} سجل لم يتم حفظه)";

        return new SaveDepartureResultDto
        {
            Message = message,
            SavedCount = savedCount,
            SkippedCount = skippedCount,
            ErrorCount = errorCount,
        };
    }

    private static void ValidateDateRange(DateTime fromDate, DateTime toDate, int maxDays)
    {
        if (fromDate.Date > toDate.Date)
            throw new InvalidOperationException("تاريخ البداية يجب أن يكون قبل تاريخ النهاية");

        var daysDiff = (toDate.Date - fromDate.Date).Days + 1;
        if (daysDiff > maxDays)
            throw new InvalidOperationException(
                $"تاريخ الفترة كبير جداً. الحد الأقصى هو {maxDays} يوم. يرجى تقليل فترة التقرير.");
    }

    private void EnsureCanModify()
    {
        if (!currentUser.CanModify)
            throw new UnauthorizedAccessException("ليس لديك صلاحية لإرسال رسائل الواتساب");
    }
}
