using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface IActivityRepository
{
    Task<(List<Activity> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Activity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);
    Task<Activity> AddAsync(Activity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
