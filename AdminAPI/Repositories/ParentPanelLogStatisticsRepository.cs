using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class ParentPanelLogStatisticsRepository(AdminDbContext db) : IParentPanelLogStatisticsRepository
{
    public Task<List<ParentPanelLog>> GetLogEntriesAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default) =>
        db.ParentPanelLogs
            .AsNoTracking()
            .Include(x => x.RegisterForm)
            .Where(x => x.AccessDateTime >= fromDate && x.AccessDateTime <= toDate)
            .ToListAsync(cancellationToken);

    public Task<List<string>> GetAllParentMobilesAsync(CancellationToken cancellationToken = default) =>
        db.RegisterForms
            .AsNoTracking()
            .Where(x => x.FatherPhone != null && x.FatherPhone != string.Empty)
            .Select(x => x.FatherPhone)
            .Distinct()
            .ToListAsync(cancellationToken);
}
