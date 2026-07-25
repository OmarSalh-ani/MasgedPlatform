using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface ISocialLinkRepository
{
    Task<(List<SocialLink> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<SocialLink?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);

    Task<SocialLink> AddAsync(SocialLink entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
