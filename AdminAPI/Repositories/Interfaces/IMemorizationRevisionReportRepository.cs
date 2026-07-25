using AdminAPI.DTOs.MemorizationRevisionReport;
using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface IMemorizationRevisionReportRepository
{
    Task<List<(int Id, string? StudentName)>> GetStudentPickListAsync(CancellationToken cancellationToken = default);

    Task<bool> StudentExistsAsync(int studentId, CancellationToken cancellationToken = default);

    Task<string?> GetStudentNameAsync(int studentId, CancellationToken cancellationToken = default);

    Task<List<MemorizationRevisionPlanRowDto>> GetPlanRowsAsync(
        int studentId,
        CancellationToken cancellationToken = default);

    Task<List<StudentPlanItemLog>> GetCompletedLogsAsync(
        int studentId,
        IEnumerable<string> statuses,
        CancellationToken cancellationToken = default);

    Task<StudentPlanMemorizing?> GetMemorizingByIdAsync(
        int id,
        int studentId,
        CancellationToken cancellationToken = default);

    Task<StudentPlanRevise?> GetReviseByIdAsync(
        int id,
        int studentId,
        CancellationToken cancellationToken = default);

    Task<Dictionary<int, string>> GetSurahNamesAsync(
        IEnumerable<int> surahIds,
        CancellationToken cancellationToken = default);

    Task<Dictionary<int, int>> GetSurahSortOrdersAsync(
        IEnumerable<int> surahIds,
        CancellationToken cancellationToken = default);
}
