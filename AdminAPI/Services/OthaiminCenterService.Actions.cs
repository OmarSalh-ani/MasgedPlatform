using AdminAPI.DTOs.Home;
using AdminAPI.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public partial class OthaiminCenterService
{
    public async Task<byte[]> ExportExcelAsync(
        HomeListFiltersDto filters,
        CancellationToken cancellationToken = default)
    {
        var teacherCircleIds = await GetTeacherCircleIdsAsync(cancellationToken);
        var query = MrkzStudentQueryBuilder.Build(db, currentUser, filters, teacherCircleIds);

        var students = await query.Select(x => new HomeExportRow
        {
            Id = x.Id,
            StudentName = x.FullName != null && x.FullName != string.Empty ? x.FullName : x.StudentName,
            FatherName = x.FatherName,
            Birthdate = x.Birthdate,
            Age = x.Age,
            StudentGender = x.StudentGender,
            FatherPhone = x.FatherPhone,
            FatherPhone2 = x.FatherPhone2,
            StudentPhone = x.StudentPhone,
            CreatedAt = x.CreatedAt,
            CircleName = x.QuranCircle != null ? x.QuranCircle.Name : string.Empty,
            IsSpecial = x.IsSpecial ? "نعم" : "لا",
            WomanActivityType = x.WomanActivity != null ? x.WomanActivity.Name : string.Empty,
            LearnCertificate = x.LearnCertificate,
            ThePassword = x.ThePassword,
            LeaveCount = 0,
            CompleteFollowup = "لا",
            IsElite = x.IsElite,
        }).ToListAsync(cancellationToken);

        return HomeExcelExporter.Build(students);
    }

    public async Task<string> SendWhatsappAsync(
        DTOs.AttendanceReport.SendAttendanceWhatsappRequestDto request,
        string? base64Image,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();

        if (request.StudentIds.Count == 0)
            throw new ValidationException("يرجى تحديد الطلاب المراد إرسال الرسائل لهم");

        if (string.IsNullOrWhiteSpace(request.Message))
            throw new ValidationException("يرجى كتابة الرسالة أولاً");

        var students = await db.MrkzStudents
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

        var isGirl = currentUser.IsGirlTeacher ? 1 : 0;

        foreach (var student in students.GroupBy(x => x.FatherPhone).Select(g => g.First()))
        {
            var message = request.Message
                .Replace("{أسم الطالب}", student.StudentName)
                .Replace("{أسم الأب}", student.FatherName ?? string.Empty)
                .Replace("{أسم الحلقة}", student.CircleName);

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

        var students = await db.MrkzStudents
            .Where(s => request.StudentIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        foreach (var student in students)
            student.QuranCircleId = request.CircleId;

        await db.SaveChangesAsync(cancellationToken);
        return students.Count;
    }

    public async Task<int> CreateCircleAsync(
        CreateHomeCircleRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();

        var circleName = request.CircleName.Trim();
        var circle = await db.QuranCircles.FirstOrDefaultAsync(
            x => x.Name == circleName && x.ForGirls == currentUser.IsGirlTeacher,
            cancellationToken);

        if (circle == null)
        {
            circle = new QuranCircle
            {
                CreatedAt = KuwaitTime.Now,
                CreatedBy = currentUser.TeacherId,
                Name = circleName,
                TeacherId = request.TeacherId,
                ForGirls = currentUser.IsGirlTeacher,
            };
            db.QuranCircles.Add(circle);
            await db.SaveChangesAsync(cancellationToken);
        }

        if (request.StudentIds.Count == 0)
            return 0;

        var students = await db.MrkzStudents
            .Where(s => request.StudentIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        foreach (var student in students)
            student.QuranCircleId = circle.Id;

        await db.SaveChangesAsync(cancellationToken);
        return students.Count;
    }

    public async Task DeleteStudentAsync(int studentId, CancellationToken cancellationToken = default)
    {
        EnsureCanModify();

        var student = await db.MrkzStudents.FirstOrDefaultAsync(x => x.Id == studentId, cancellationToken);
        if (student == null)
            throw new InvalidOperationException($"الطالب رقم {studentId} غير موجود");

        db.MrkzStudents.Remove(student);
        await db.SaveChangesAsync(cancellationToken);
    }
}
