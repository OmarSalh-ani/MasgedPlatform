using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Helpers;

public static class StudentCircleAccessHelper
{
    public static async Task<bool> IsCurrentMemberAsync(
        AppDbContext db,
        int studentId,
        int circleId,
        CancellationToken cancellationToken) =>
        await db.RegisterForms
            .AnyAsync(s => s.Id == studentId && s.QuranCircleId == circleId, cancellationToken);

    public static async Task<bool> HasEnrollmentInCircleAsync(
        AppDbContext db,
        int studentId,
        int circleId,
        CancellationToken cancellationToken) =>
        await db.StudentCircleEnrollments
            .AnyAsync(e => e.StudentId == studentId && e.CircleId == circleId, cancellationToken);

    public static async Task<bool> HasFormerEnrollmentAsync(
        AppDbContext db,
        int studentId,
        int circleId,
        CancellationToken cancellationToken) =>
        await db.StudentCircleEnrollments
            .AnyAsync(e => e.StudentId == studentId && e.CircleId == circleId && e.EndDate != null, cancellationToken);

    public static async Task<bool> CanReadStudentAsync(
        AppDbContext db,
        int studentId,
        int circleId,
        CancellationToken cancellationToken) =>
        await IsCurrentMemberAsync(db, studentId, circleId, cancellationToken)
        || await HasFormerEnrollmentAsync(db, studentId, circleId, cancellationToken);

    public static async Task<bool> CanWriteStudentAsync(
        AppDbContext db,
        int studentId,
        int circleId,
        CancellationToken cancellationToken) =>
        await IsCurrentMemberAsync(db, studentId, circleId, cancellationToken);

    public static async Task<bool> CanReadRecordAsync(
        AppDbContext db,
        int studentId,
        int circleId,
        int recordCircleId,
        CancellationToken cancellationToken)
    {
        if (await IsCurrentMemberAsync(db, studentId, circleId, cancellationToken))
            return true;

        return await HasFormerEnrollmentAsync(db, studentId, circleId, cancellationToken)
               && recordCircleId == circleId;
    }

    public static async Task<bool> CanWriteRecordAsync(
        AppDbContext db,
        int studentId,
        int circleId,
        int recordCircleId,
        CancellationToken cancellationToken) =>
        await IsCurrentMemberAsync(db, studentId, circleId, cancellationToken)
        && recordCircleId == circleId;

    public static async Task<RegisterForm?> GetStudentIfReadableAsync(
        AppDbContext db,
        int studentId,
        int circleId,
        CancellationToken cancellationToken,
        bool track = false)
    {
        if (!await CanReadStudentAsync(db, studentId, circleId, cancellationToken))
            return null;

        var query = track ? db.RegisterForms : db.RegisterForms.AsNoTracking();
        return await query.FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
    }

    public static async Task<RegisterForm?> GetStudentIfWritableAsync(
        AppDbContext db,
        int studentId,
        int circleId,
        CancellationToken cancellationToken,
        bool track = false)
    {
        if (!await CanWriteStudentAsync(db, studentId, circleId, cancellationToken))
            return null;

        var query = track ? db.RegisterForms : db.RegisterForms.AsNoTracking();
        return await query.FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
    }
}
