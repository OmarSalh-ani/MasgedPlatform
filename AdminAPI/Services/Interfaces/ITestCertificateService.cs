using AdminAPI.DTOs.TestCertificate;

namespace AdminAPI.Services.Interfaces;

public interface ITestCertificateService
{
    Task<TestCertificateDto> GetByTestIdAsync(int testId, CancellationToken cancellationToken = default);
}
