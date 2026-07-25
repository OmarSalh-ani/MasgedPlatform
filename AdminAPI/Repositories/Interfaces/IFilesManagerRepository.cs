using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface IFilesManagerRepository
{
    Task<(List<FilesManager> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<FilesManager?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<FilesManager> AddAsync(FilesManager entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
