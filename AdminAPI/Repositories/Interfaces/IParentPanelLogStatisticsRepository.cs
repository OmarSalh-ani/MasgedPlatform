using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface IParentPanelLogStatisticsRepository
{
    Task<List<ParentPanelLog>> GetLogEntriesAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);

    Task<List<string>> GetAllParentMobilesAsync(CancellationToken cancellationToken = default);
}
