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

        var logs = await db.ParentPanelLogs.Where(x => x.StudentId == studentId).ToListAsync(cancellationToken);
        db.ParentPanelLogs.RemoveRange(logs);

        try
        {
            // Child values first (FK_ResponseValues_Response), then responses (FK_FormResponses_RegisterForm).
            // Table names match dbo.FormResponses / dbo.FormResponseValues (not singular FormResponse).
            await db.Database.ExecuteSqlRawAsync(
                """
                DELETE FROM FormResponseValues
                WHERE ResponseId IN (SELECT Id FROM FormResponses WHERE StudentId = {0});
                DELETE FROM FormResponses WHERE StudentId = {0}
                """,
                [studentId],
                cancellationToken);
        }
        catch
        {
            // Tables may not exist in some environments.
        }

        db.RegisterForms.Remove(student);
        await db.SaveChangesAsync(cancellationToken);
    }
}
