using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface ICompetitionRepository
{
    Task<(List<Competition> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Competition?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);

    Task<Competition> AddAsync(Competition entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
