using AdminAPI.DTOs.Teachers;
using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface ITeacherRepository
{
    Task<List<TeacherListItemDto>> GetListAsync(bool forGirls, CancellationToken cancellationToken = default);

    Task<List<TeacherListItemDto>> GetExportListAsync(bool forGirls, CancellationToken cancellationToken = default);

    Task<Teacher?> GetEntityByIdAsync(int id, CancellationToken cancellationToken = default);

    Task AddEntityAsync(Teacher teacher, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<string?> DeleteWithRelatedAsync(
        int id,
        bool forGirls,
        bool restrictCirclesToForGirls,
        CancellationToken cancellationToken = default);
}