using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface ICurrentStudentPlanRepository
{
    Task<(List<StudentPlan> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? studentId = null,
        CancellationToken cancellationToken = default);

    Task<(List<(int Id, string Name)> Items, int TotalCount)> GetStudentLookupAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<HashSet<string>> GetDuplicateStudentNameSetAsync(
        string? search,
        CancellationToken cancellationToken = default);

    Task<Dictionary<int, List<int>>> GetCircleDaysLookupAsync(CancellationToken cancellationToken = default);
    Task<bool> DeleteWithRelatedAsync(int planId, CancellationToken cancellationToken = default);
}
