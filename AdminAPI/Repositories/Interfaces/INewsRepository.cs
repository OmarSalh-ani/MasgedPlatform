using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface INewsRepository
{
    Task<(List<NewsItem> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<NewsItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);
    Task<NewsItem> AddAsync(NewsItem entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
