using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class CurrentStudentPlanRepository(AdminDbContext db) : ICurrentStudentPlanRepository
{
    public async Task<(List<StudentPlan> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? studentId = null,
        CancellationToken cancellationToken = default)
    {
        var query = db.StudentPlans.AsNoTracking()
            .Include(p => p.RegisterForm)
            .ThenInclude(r => r!.QuranCircle)
            .AsQueryable();

        if (studentId is > 0)
            query = query.Where(p => p.StudentId == studentId.Value);

        query = query.OrderByDescending(p => p.Id);

        var totalCount = await query.CountAsync(cancellationToken);

        if (pageSize <= 0)
        {
            var all = await query.ToListAsync(cancellationToken);
            return (all, totalCount);
        }

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(List<(int Id, string Name)> Items, int TotalCount)> GetStudentLookupAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var studentIds = db.StudentPlans.AsNoTracking().Select(p => p.StudentId).Distinct();

        var query = db.RegisterForms.AsNoTracking()
            .Where(r => studentIds.Contains(r.Id));

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(r =>
                r.StudentName.Contains(term) ||
                (r.FullName != null && r.FullName.Contains(term)));
        }

        var projected = query.Select(r => new
        {
            r.Id,
            Name = r.FullName != null && r.FullName != string.Empty ? r.FullName : r.StudentName,
        });

        var page = pageNumber < 1 ? 1 : pageNumber;
        var size = pageSize < 1 ? 20 : pageSize;
        var totalCount = await projected.CountAsync(cancellationToken);
        var items = await projected
            .OrderBy(x => x.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(x => new ValueTuple<int, string>(x.Id, x.Name))
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<HashSet<string>> GetDuplicateStudentNameSetAsync(
        string? search,
        CancellationToken cancellationToken = default)
    {
        var studentIds = db.StudentPlans.AsNoTracking().Select(p => p.StudentId).Distinct();
        var query = db.RegisterForms.AsNoTracking()
            .Where(r => studentIds.Contains(r.Id));

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(r =>
                r.StudentName.Contains(term) ||
                (r.FullName != null && r.FullName.Contains(term)));
        }

        var duplicateNames = await query
            .Select(r => r.FullName != null && r.FullName != string.Empty ? r.FullName : r.StudentName)
            .GroupBy(name => name)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToListAsync(cancellationToken);

        return duplicateNames.ToHashSet(StringComparer.Ordinal);
    }

    public async Task<Dictionary<int, List<int>>> GetCircleDaysLookupAsync(
        CancellationToken cancellationToken = default) =>
        await db.CircleDays.AsNoTracking()
            .GroupBy(d => d.CircleId)
            .ToDictionaryAsync(
                g => g.Key,
                g => g.Select(d => d.DayNumber).ToList(),
                cancellationToken);

    public async Task<bool> DeleteWithRelatedAsync(int planId, CancellationToken cancellationToken = default)
    {
        var plan = await db.StudentPlans.FirstOrDefaultAsync(p => p.Id == planId, cancellationToken);
        if (plan is null)
            return false;

        var memorizings = await db.StudentPlanMemorizings
            .Where(m => m.PlanId == planId)
            .ToListAsync(cancellationToken);
        db.StudentPlanMemorizings.RemoveRange(memorizings);

        var revises = await db.StudentPlanRevises
            .Where(r => r.PlanId == planId)
            .ToListAsync(cancellationToken);
        db.StudentPlanRevises.RemoveRange(revises);

        var logs = await db.StudentPlanItemLogs
            .Where(l => l.PlanId == planId)
            .ToListAsync(cancellationToken);
        db.StudentPlanItemLogs.RemoveRange(logs);

        db.StudentPlans.Remove(plan);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
