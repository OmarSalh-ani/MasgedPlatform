using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface IExpensiveRepository
{
    Task<List<Expensive>> GetListAsync(bool forGirls, CancellationToken cancellationToken = default);

    Task<Expensive?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Expensive?> GetByIdScopedAsync(int id, bool forGirls, CancellationToken cancellationToken = default);

    Task<Expensive> AddAsync(Expensive entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, bool forGirls, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
