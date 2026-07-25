using AdminAPI.DTOs.WomansActivities;
using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface IWomansActivityRepository
{
    Task<List<WomanActivityListItemDto>> GetListAsync(
        bool forGirl,
        CancellationToken cancellationToken = default);

    Task<WomanActivity?> GetByIdAsync(
        int id,
        bool forGirl,
        CancellationToken cancellationToken = default);

    Task<WomanActivity> AddAsync(WomanActivity entity, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
