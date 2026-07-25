using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface IPlanLevelRepository
{
    Task<(List<PlanLevel> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PlanLevel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> HasReadyPlanDependencyAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> HasRegisterFormDependencyAsync(int id, CancellationToken cancellationToken = default);

    Task<PlanLevel> AddAsync(PlanLevel entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
