using AdminAPI.DTOs.AttendanceReport;
using AdminAPI.DTOs.Home;
using AdminAPI.Models;
using AdminAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public partial class HomeService
{
    public async Task<string> SendWhatsappAsync(
        SendAttendanceWhatsappRequestDto request,
        string? base64Image,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();

        if (request.StudentIds.Count == 0)
            throw new ValidationException("يرجى تحديد الطلاب المراد إرسال الرسائل لهم");

        if (string.IsNullOrWhiteSpace(request.Message))
            throw new ValidationException("يرجى كتابة الرسالة أولاً");

        var students = await db.RegisterForms
            .AsNoTracking()
            .Where(x => request.StudentIds.Contains(x.Id))
            .Select(x => new
            {
                x.Id,
                StudentName = x.FullName != null && x.FullName != string.Empty ? x.FullName : x.StudentName,
                x.FatherName,
                x.FatherPhone,
                CircleName = x.QuranCircle != null ? x.QuranCircle.Name : string.Empty,
            })
            .ToListAsync(cancellationToken);

        var baseUrl = publicSiteOptions.Value.BaseUrl.TrimEnd('/');
        var isGirl = currentUser.IsGirlTeacher ? 1 : 0;

        foreach (var student in students.GroupBy(x => x.FatherPhone).Select(g => g.First()))
        {
            var message = request.Message
                .Replace("{أسم الطالب}", student.StudentName)
                .Replace("{أسم الأب}", student.FatherName ?? string.Empty)
                .Replace("{أسم الحلقة}", student.CircleName)
                .Replace("{الرابط}", $"{baseUrl}/parents-followup?id={student.Id}");

            db.WhatsappTempTables.Add(new WhatsappTempTable
            {
                Image = base64Image,
                Message = message,
                Mobile = student.FatherPhone,
                IsGirl = isGirl,
            });
        }

        await db.SaveChangesAsync(cancellationToken);
        return "تم إرسال الرسائل بنجاح";
    }

    public async Task<int> TransferStudentsAsync(
        TransferHomeStudentsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();

        var newCircle = await db.QuranCircles.FirstOrDefaultAsync(
            c => c.Id == request.CircleId && c.ForGirls == currentUser.IsGirlTeacher,
            cancellationToken);

        if (newCircle == null)
            throw new ValidationException("الحلقة المختارة غير صحيحة");

        var students = await db.RegisterForms
            .Where(s => request.StudentIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        foreach (var student in students)
            student.QuranCircleId = request.CircleId;

        await db.SaveChangesAsync(cancellationToken);
        return students.Count;
    }

    private void EnsureCanModify()
    {
        if (!currentUser.CanModify)
            throw new UnauthorizedAccessException("ليس لديك صلاحية لتنفيذ هذا الإجراء");
    }
}
