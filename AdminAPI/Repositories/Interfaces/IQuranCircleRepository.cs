using AdminAPI.DTOs.QuranCircles;
using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface IQuranCircleRepository
{
    Task<List<QuranCircleListItemDto>> GetListAsync(
        bool girlOnly,
        bool isAdmin,
        int teacherId,
        int? filterTeacherId,
        CancellationToken cancellationToken = default);

    Task<List<QuranCircleListItemDto>> GetExportListAsync(
        bool girlOnly,
        CancellationToken cancellationToken = default);

    Task<QuranCircle?> GetByIdWithDaysAsync(int id, CancellationToken cancellationToken = default);
    Task<QuranCircle> AddAsync(QuranCircle entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteWithRelatedAsync(int id, int? teacherId, CancellationToken cancellationToken = default);
    Task ReplaceDaysAsync(int circleId, IEnumerable<int> dayNumbers, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
