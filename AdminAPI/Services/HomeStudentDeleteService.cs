using AdminAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public static class HomeStudentDeleteService
{
    public static async Task DeleteAsync(
        Data.AdminDbContext db,
        int studentId,
        CancellationToken cancellationToken = default)
    {
        var student = await db.RegisterForms.FirstOrDefaultAsync(x => x.Id == studentId, cancellationToken);
        if (student == null)
            throw new InvalidOperationException($"الطالب رقم {studentId} غير موجود");

        var tests = await db.StudentTests.Where(x => x.StudentId == studentId).ToListAsync(cancellationToken);
        db.StudentTests.RemoveRange(tests);

        var attendances = await db.CircleAttendances.Where(x => x.StudentId == studentId).ToListAsync(cancellationToken);
        db.CircleAttendances.RemoveRange(attendances);

        var cards = await db.StudentMemorizingCards.Where(x => x.StudentId == studentId).ToListAsync(cancellationToken);
        db.StudentMemorizingCards.RemoveRange(cards);

        var followups = await db.ParentFollowups.Where(x => x.StudentId == studentId).ToListAsync(cancellationToken);
        db.ParentFollowups.RemoveRange(followups);

        try
        {
            await db.Database.ExecuteSqlRawAsync(
                "DELETE FROM FormResponse WHERE StudentId = {0}",
                [studentId],
                cancellationToken);
        }
        catch
        {
            // Table may not exist in some environments.
        }

        try
        {
            await db.Database.ExecuteSqlRawAsync(
                "DELETE FROM ParentPanelLog WHERE StudentId = {0}",
                [studentId],
                cancellationToken);
        }
        catch
        {
            // Table may not exist in some environments.
        }

        db.RegisterForms.Remove(student);
        await db.SaveChangesAsync(cancellationToken);
    }
}
