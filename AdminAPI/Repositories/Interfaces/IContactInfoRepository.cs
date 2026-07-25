using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface IContactInfoRepository
{
    Task<(List<ContactInfo> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ContactInfo?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);

    Task<ContactInfo> AddAsync(ContactInfo entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
