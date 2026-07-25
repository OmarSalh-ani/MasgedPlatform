using AdminAPI.Data;
using AdminAPI.DTOs.StudentPlan;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class StudentPlanRepository(AdminDbContext db) : IStudentPlanRepository
{
    public Task<List<QuranSurah>> GetSurahsAsync(CancellationToken cancellationToken = default) =>
        db.QuranSurahs.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<List<StudentPlanCircleOptionDto>> GetCirclesAsync(
        bool isGirlTeacher,
        bool isAdmin,
        int teacherId,
        CancellationToken cancellationToken = default)
    {
        var query = db.QuranCircles.AsNoTracking().Where(x => x.ForGirls == isGirlTeacher);
        if (!isAdmin)
            query = query.Where(c => c.TeacherId == teacherId);

        return await query
            .OrderBy(x => x.Name)
            .Select(x => new StudentPlanCircleOptionDto { Id = x.Id, Name = x.Name })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<StudentPlanStudentOptionDto>> GetStudentsAsync(
        bool isGirlTeacher,
        bool isAdmin,
        int teacherId,
        CancellationToken cancellationToken = default)
    {
        var gender = isGirlTeacher ? "أنثى" : "ذكر";
        var query = db.RegisterForms.AsNoTracking().Where(x => x.StudentGender == gender);

        if (!isAdmin)
        {
            var circleIds = db.QuranCircles
                .Where(c => c.TeacherId == teacherId)
                .Select(c => c.Id);
            query = query.Where(x => x.QuranCircleId != null && circleIds.Contains(x.QuranCircleId.Value));
        }

        return await query
            .OrderBy(x => x.FullName ?? x.StudentName)
            .Select(x => new StudentPlanStudentOptionDto
            {
                Id = x.Id,
                Name = x.FullName ?? x.StudentName ?? "—",
                QuranCircleId = x.QuranCircleId,
            })
            .ToListAsync(cancellationToken);
    }

    public Task<List<int>> GetAyahNumbersAsync(int surahId, CancellationToken cancellationToken = default) =>
        db.QuranAyahs.AsNoTracking()
            .Where(x => x.SurahId == surahId)
            .OrderBy(x => x.AyahNumber)
            .Select(x => x.AyahNumber)
            .ToListAsync(cancellationToken);

    public Task<RegisterForm?> GetStudentAsync(int studentId, CancellationToken cancellationToken = default) =>
        db.RegisterForms.AsNoTracking()
            .Where(x => x.Id == studentId)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<List<StudentPlan>> GetPlansForStudentAsync(int studentId, CancellationToken cancellationToken = default) =>
        db.StudentPlans.AsNoTracking()
            .Where(p => p.StudentId == studentId)
            .OrderBy(p => p.PlanFromDate)
            .ToListAsync(cancellationToken);

    public Task<StudentPlan?> GetPlanAsync(int planId, CancellationToken cancellationToken = default) =>
        db.StudentPlans.FirstOrDefaultAsync(p => p.Id == planId, cancellationToken);

    public Task<List<StudentPlanMemorizing>> GetMemorizingsAsync(
        int studentId,
        int planId,
        CancellationToken cancellationToken = default) =>
        db.StudentPlanMemorizings.AsNoTracking()
            .Include(x => x.QuranSurah)
            .Where(x => x.StudentId == studentId && x.PlanId == planId)
            .ToListAsync(cancellationToken);

    public Task<List<StudentPlanRevise>> GetRevisesAsync(
        int studentId,
        int planId,
        CancellationToken cancellationToken = default) =>
        db.StudentPlanRevises.AsNoTracking()
            .Include(x => x.QuranSurah)
            .Where(x => x.StudentId == studentId && x.PlanId == planId)
            .ToListAsync(cancellationToken);

    public async Task<int?> ResolvePlanIdFromEditKeyAsync(
        int studentId,
        string editKey,
        CancellationToken cancellationToken = default)
    {
        if (editKey.StartsWith("memorizing_") && int.TryParse(editKey["memorizing_".Length..], out var memId))
        {
            var ent = await db.StudentPlanMemorizings.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == memId && x.StudentId == studentId, cancellationToken);
            return ent?.PlanId;
        }

        if (editKey.StartsWith("revise_") && int.TryParse(editKey["revise_".Length..], out var revId))
        {
            var ent = await db.StudentPlanRevises.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == revId && x.StudentId == studentId, cancellationToken);
            return ent?.PlanId;
        }

        return null;
    }

    public Task<StudentPlanMemorizing?> GetMemorizingByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.StudentPlanMemorizings.Include(x => x.QuranSurah).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<StudentPlanRevise?> GetReviseByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.StudentPlanRevises.Include(x => x.QuranSurah).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
