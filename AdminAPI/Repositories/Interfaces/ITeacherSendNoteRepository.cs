using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface ITeacherSendNoteRepository
{
    Task<(List<TeachersAdminNote> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<TeachersAdminNote?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<List<Teacher>> GetTeachersOrderedByNameAsync(CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<TeachersAdminNote> notes, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
