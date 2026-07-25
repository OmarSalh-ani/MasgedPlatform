using AdminAPI.DTOs.Home;
using AdminAPI.Models;
using AdminAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public partial class HomeService
{
    public async Task<int> RemoveFromCircleAsync(
        RemoveHomeStudentsFromCircleRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();

        var students = await db.RegisterForms
            .Where(s => request.StudentIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        foreach (var student in students)
            student.QuranCircleId = null;

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

        var students = await db.RegisterForms
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
        await HomeStudentDeleteService.DeleteAsync(db, studentId, cancellationToken);
    }
}
