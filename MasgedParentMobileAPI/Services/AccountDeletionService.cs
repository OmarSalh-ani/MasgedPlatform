using MasgedParentMobileAPI.Models;
using MasgedTeacherMobileAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace MasgedParentMobileAPI.Services;

public sealed class AccountDeletionService(
    NewMasgedTeacherAPIDBContext parentDb,
    AppDbContext teacherDb)
{
    private const string DeletedLabel = "محذوف";

    public async Task<(bool Success, string Message)> DeleteParentAccountAsync(
        string fatherPhoneFromJwt,
        string password,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(password))
            return (false, "يرجى إدخال كلمة المرور");

        var canonical = PhoneNormalizer.ToCanonical(fatherPhoneFromJwt);
        var variants = PhoneNormalizer.GetVariants(fatherPhoneFromJwt).ToList();

        var rows = await parentDb.RegisterForms
            .Include(r => r.ParentFollowup)
            .Where(r =>
                variants.Contains(r.FatherPhone!) ||
                (r.FatherPhone2 != null && variants.Contains(r.FatherPhone2)))
            .ToListAsync(cancellationToken);

        if (rows.Count == 0)
            return (false, "لم يتم العثور على الحساب");

        if (!rows.Any(r => r.ThePassword == password))
            return (false, "كلمة المرور غير صحيحة");

        var studentIds = rows.Select(r => r.Id).ToList();

        foreach (var row in rows)
        {
            row.ThePassword = null;
            row.FatherName = DeletedLabel;
            row.FatherPhone2 = null;

            if (row.ParentFollowup is null)
                continue;

            row.ParentFollowup.Address = null;
            row.ParentFollowup.MaritalStatus = null;
            row.ParentFollowup.HealthCondition = null;
            row.ParentFollowup.HealthDetails = null;
            row.ParentFollowup.LearningDifficulties = null;
            row.ParentFollowup.LearningDifficultiesNotes = null;
            row.ParentFollowup.PhotoPath = null;
        }

        var deviceTokens = await parentDb.ParentDeviceTokens
            .Where(t => t.ParentPhone == canonical || variants.Contains(t.ParentPhone))
            .ToListAsync(cancellationToken);
        parentDb.ParentDeviceTokens.RemoveRange(deviceTokens);

        var chatMessages = await parentDb.ParentTeacherChatMessages
            .Where(m => m.StudentId != null && studentIds.Contains(m.StudentId.Value))
            .ToListAsync(cancellationToken);
        parentDb.ParentTeacherChatMessages.RemoveRange(chatMessages);

        var otps = await parentDb.ParentRegistrationOtps
            .Where(o => o.CanonicalPhone == canonical)
            .ToListAsync(cancellationToken);
        parentDb.ParentRegistrationOtps.RemoveRange(otps);

        await parentDb.SaveChangesAsync(cancellationToken);
        return (true, "تم حذف حسابك بنجاح");
    }

    public async Task<(bool Success, string Message)> DeleteTeacherAccountAsync(
        int teacherId,
        string password,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(password))
            return (false, "يرجى إدخال كلمة المرور");

        var teacher = await teacherDb.Teachers
            .FirstOrDefaultAsync(t => t.Id == teacherId && !t.UsersManage, cancellationToken);

        if (teacher is null)
            return (false, "المعلم غير موجود");

        if (teacher.Password != password.Trim())
            return (false, "كلمة المرور غير صحيحة");

        teacher.Name = DeletedLabel;
        teacher.Email = $"deleted_{teacherId}@deleted.local";
        teacher.Password = Guid.NewGuid().ToString("N");

        var deviceTokens = await teacherDb.TeacherDeviceTokens
            .Where(t => t.TeacherId == teacherId)
            .ToListAsync(cancellationToken);
        teacherDb.TeacherDeviceTokens.RemoveRange(deviceTokens);

        await teacherDb.SaveChangesAsync(cancellationToken);
        return (true, "تم حذف حسابك بنجاح");
    }
}
