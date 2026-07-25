using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface IAboutRepository
{
    Task<AboutAssociation?> GetFirstAsync(CancellationToken cancellationToken = default);
    Task<AboutAssociation> AddAsync(AboutAssociation entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
