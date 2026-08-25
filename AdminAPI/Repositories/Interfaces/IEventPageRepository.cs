using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface IEventPageRepository
{
    Task<(List<EventPage> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<EventPage?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EventPage?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<List<EventPage>> GetLookupsAsync(CancellationToken cancellationToken = default);
    Task<bool> ActivityNameExistsAsync(string activityName, int? excludeId, CancellationToken cancellationToken = default);
    Task<bool> SlugExistsAsync(string slug, int? excludeId, CancellationToken cancellationToken = default);
    Task<EventPage> AddAsync(EventPage entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
