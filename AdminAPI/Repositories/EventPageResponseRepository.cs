using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class EventPageResponseRepository(AdminDbContext db) : IEventPageResponseRepository
{
    private IQueryable<EventPageResponse> FilteredQuery(string? activityName)
    {
        var query = db.EventPageResponses.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(activityName))
            query = query.Where(r => r.ActivityName == activityName.Trim());

        return query.OrderByDescending(r => r.SubmittedAt).ThenByDescending(r => r.Id);
    }

    public async Task<(List<EventPageResponse> Items, int TotalCount)> GetPagedAsync(
        string? activityName,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = FilteredQuery(activityName);
        var totalCount = await query.CountAsync(cancellationToken);
        var page = pageNumber < 1 ? 1 : pageNumber;
        var size = pageSize <= 0 ? 20 : pageSize;

        var items = await query
            .Include(r => r.Values)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<List<EventPageResponse>> GetForExportAsync(
        string? activityName,
        CancellationToken cancellationToken = default) =>
        FilteredQuery(activityName)
            .Include(r => r.Values)
            .ToListAsync(cancellationToken);

    public async Task<List<string>> GetFieldLabelsAsync(
        string? activityName,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(activityName))
        {
            var fromPage = await db.EventPageFormFields.AsNoTracking()
                .Where(f => f.EventPage!.ActivityName == activityName.Trim())
                .OrderBy(f => f.SortOrder)
                .ThenBy(f => f.Id)
                .Select(f => f.Label)
                .ToListAsync(cancellationToken);

            if (fromPage.Count > 0)
                return fromPage;
        }

        return await db.EventPageResponseValues.AsNoTracking()
            .Where(v => string.IsNullOrWhiteSpace(activityName)
                || v.Response!.ActivityName == activityName.Trim())
            .Select(v => v.FieldLabel)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(EventPageResponse entity, CancellationToken cancellationToken = default) =>
        await db.EventPageResponses.AddAsync(entity, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
