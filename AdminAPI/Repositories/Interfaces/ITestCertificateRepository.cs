using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface ITestCertificateRepository
{
    Task<TestHead?> GetByIdAsync(int testId, CancellationToken cancellationToken = default);
}
