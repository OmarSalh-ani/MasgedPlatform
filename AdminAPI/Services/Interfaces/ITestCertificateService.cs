using AdminAPI.DTOs.PushNotifications;
using AdminAPI.DTOs.TestCertificate;

namespace AdminAPI.Services.Interfaces;

public interface ITestCertificateService
{
    Task<TestCertificateDto> GetByTestIdAsync(int testId, CancellationToken cancellationToken = default);

    Task<SendAdminPushNotificationResultDto> SendNotificationAsync(
        int testId,
        SendTestCertificateNotificationRequestDto request,
        CancellationToken cancellationToken = default);
}
