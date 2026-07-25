using AdminAPI.Data;
using AdminAPI.DTOs.MemorizationRevisionReport;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class MemorizationRevisionReportRepository(AdminDbContext db) : IMemorizationRevisionReportRepository
{
    private const string DefaultStatus = "قيد الأنتظار";

    public async Task<List<(int Id, string? StudentName)>> GetStudentPickListAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await db.RegisterForms
            .AsNoTracking()
            .Select(x => new { x.Id, x.StudentName })
            .ToListAsync(cancellationToken);

        return list
            .GroupBy(x => x.Id)
            .Select(g => g.First())
            .OrderBy(x => x.StudentName)
            .Select(x => (x.Id, (string?)x.StudentName))
            .ToList();
    }

    public Task<bool> StudentExistsAsync(int studentId, CancellationToken cancellationToken = default) =>
        db.RegisterForms.AsNoTracking().AnyAsync(r => r.Id == studentId, cancellationToken);

    public Task<string?> GetStudentNameAsync(int studentId, CancellationToken cancellationToken = default) =>
        db.RegisterForms
            .AsNoTracking()
            .Where(r => r.Id == studentId)
            .Select(r => r.StudentName)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<List<MemorizationRevisionPlanRowDto>> GetPlanRowsAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        var memList = await db.StudentPlanMemorizings
            .AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .Select(x => new MemorizationRevisionPlanRowDto
            {
                Status = x.Status ?? DefaultStatus,
                SurahNameAr = x.QuranSurah.NameAr,
                StudentName = x.RegisterForm.StudentName,
                FromAyah = x.FromAyahNumber,
                ToAyah = x.ToAyahNumber,
                PlanType = "خطة الحفظ",
            })
            .ToListAsync(cancellationToken);

        var revList = await db.StudentPlanRevises
            .AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .Select(x => new MemorizationRevisionPlanRowDto
            {
                Status = x.Status ?? DefaultStatus,
                SurahNameAr = x.QuranSurah.NameAr,
                StudentName = x.RegisterForm.StudentName,
                FromAyah = x.FromAyahNumber,
                ToAyah = x.ToAyahNumber,
                PlanType = "خطة المراجعة",
            })
            .ToListAsync(cancellationToken);

        return memList.Concat(revList).ToList();
    }

    public Task<List<StudentPlanItemLog>> GetCompletedLogsAsync(
        int studentId,
        IEnumerable<string> statuses,
        CancellationToken cancellationToken = default) =>
        db.StudentPlanItemLogs
            .AsNoTracking()
            .Where(x => x.StudentId == studentId && statuses.Contains(x.Status))
            .ToListAsync(cancellationToken);

    public Task<StudentPlanMemorizing?> GetMemorizingByIdAsync(
        int id,
        int studentId,
        CancellationToken cancellationToken = default) =>
        db.StudentPlanMemorizings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.StudentId == studentId, cancellationToken);

    public Task<StudentPlanRevise?> GetReviseByIdAsync(
        int id,
        int studentId,
        CancellationToken cancellationToken = default) =>
        db.StudentPlanRevises
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.StudentId == studentId, cancellationToken);

    public async Task<Dictionary<int, string>> GetSurahNamesAsync(
        IEnumerable<int> surahIds,
        CancellationToken cancellationToken = default)
    {
        var ids = surahIds.Distinct().ToList();
        if (ids.Count == 0)
            return [];

        return await db.QuranSurahs
            .AsNoTracking()
            .Where(q => ids.Contains(q.Id))
            .ToDictionaryAsync(q => q.Id, q => q.NameAr ?? string.Empty, cancellationToken);
    }

    public async Task<Dictionary<int, int>> GetSurahSortOrdersAsync(
        IEnumerable<int> surahIds,
        CancellationToken cancellationToken = default)
    {
        var ids = surahIds.Distinct().ToList();
        if (ids.Count == 0)
            return [];

        return await db.QuranSurahs
            .AsNoTracking()
            .Where(q => ids.Contains(q.Id))
            .ToDictionaryAsync(q => q.Id, q => q.SortOrder ?? int.MaxValue, cancellationToken);
    }
}
