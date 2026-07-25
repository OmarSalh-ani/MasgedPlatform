using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Helpers;

public static class StudentCircleTransferHelper
{
    public static async Task AssignStudentsToCircleAsync(
        AppDbContext db,
        IReadOnlyList<RegisterForm> students,
        int circleId,
        int? assignedByTeacherId,
        CancellationToken cancellationToken)
    {
        if (students.Count == 0)
            return;

        var now = DateTime.UtcNow;
        var transferringStudents = students.Where(s => s.QuranCircleId != circleId).ToList();
        if (transferringStudents.Count == 0)
            return;

        var transferringIds = transferringStudents.Select(s => s.Id).ToList();

        var activeEnrollments = await db.StudentCircleEnrollments
            .Where(e => transferringIds.Contains(e.StudentId) && e.EndDate == null)
            .ToListAsync(cancellationToken);

        foreach (var enrollment in activeEnrollments)
            enrollment.EndDate = now;

        foreach (var student in transferringStudents)
        {
            await ArchiveActivePlansAsync(db, student.Id, cancellationToken);

            db.StudentCircleEnrollments.Add(new StudentCircleEnrollment
            {
                StudentId = student.Id,
                CircleId = circleId,
                StartDate = now,
                AssignedByTeacherId = assignedByTeacherId
            });

            student.QuranCircleId = circleId;
        }
    }

    public static async Task<bool> RemoveStudentFromCircleAsync(
        AppDbContext db,
        RegisterForm student,
        int circleId,
        CancellationToken cancellationToken)
    {
        if (student.QuranCircleId != circleId)
            return false;

        var now = DateTime.UtcNow;

        var activeEnrollments = await db.StudentCircleEnrollments
            .Where(e => e.StudentId == student.Id && e.CircleId == circleId && e.EndDate == null)
            .ToListAsync(cancellationToken);

        foreach (var enrollment in activeEnrollments)
            enrollment.EndDate = now;

        await ArchiveActivePlansAsync(db, student.Id, cancellationToken);

        student.QuranCircleId = null;
        return true;
    }

    private static async Task ArchiveActivePlansAsync(
        AppDbContext db,
        int studentId,
        CancellationToken cancellationToken)
    {
        var activePlans = await db.StudentPlans
            .Where(p => p.StudentId == studentId && !p.IsArchived)
            .ToListAsync(cancellationToken);

        foreach (var plan in activePlans)
            plan.IsArchived = true;
    }
}
