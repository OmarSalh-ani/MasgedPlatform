using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class TestCertificateRepository(AdminDbContext db) : ITestCertificateRepository
{
    public Task<TestHead?> GetByIdAsync(int testId, CancellationToken cancellationToken = default) =>
        db.TestHeads
            .AsNoTracking()
            .Include(t => t.Student!)
                .ThenInclude(s => s.QuranCircle)
            .Include(t => t.Teacher)
            .FirstOrDefaultAsync(t => t.Id == testId, cancellationToken);
}
