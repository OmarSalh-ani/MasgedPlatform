using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface IEventPageResponseRepository
{
    Task<(List<EventPageResponse> Items, int TotalCount)> GetPagedAsync(
        string? activityName,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<List<EventPageResponse>> GetForExportAsync(
        string? activityName,
        CancellationToken cancellationToken = default);

    Task<List<string>> GetFieldLabelsAsync(
        string? activityName,
        CancellationToken cancellationToken = default);

    Task AddAsync(EventPageResponse entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
