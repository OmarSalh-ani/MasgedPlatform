using AdminAPI.DTOs.PushNotifications;
using AdminAPI.DTOs.TestCertificate;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using FluentValidation;

namespace AdminAPI.Services;

public class TestCertificateService(
    ITestCertificateRepository repository,
    ICurrentUserContext currentUser,
    IAdminPushNotificationService pushNotificationService) : ITestCertificateService
{
    public async Task<TestCertificateDto> GetByTestIdAsync(
        int testId,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAdmin)
            throw new UnauthorizedAccessException("غير مصرح");

        var test = await repository.GetByIdAsync(testId, cancellationToken)
            ?? throw new KeyNotFoundException("الاختبار غير موجود");

        return new TestCertificateDto
        {
            TestId = test.Id,
            StudentId = test.StudentId,
            StudentName = test.Student?.StudentName ?? string.Empty,
            CircleName = test.Student?.QuranCircle?.Name ?? string.Empty,
            TeacherName = test.Teacher?.Name ?? string.Empty,
            TestDate = test.CreatedAt.ToString("yyyy/MM/dd"),
            TestFrom = test.TestFrom ?? string.Empty,
            TestTo = test.TestTo ?? string.Empty,
            MemorizationScore = ResolveScore(test.MemorizationScore),
            TajweedScore = ResolveScore(test.TajweedScore),
            RevisionScore = ResolveScore(test.RevisionScore),
        };
    }

    public async Task<SendAdminPushNotificationResultDto> SendNotificationAsync(
        int testId,
        SendTestCertificateNotificationRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.CanModify)
            throw new UnauthorizedAccessException("ليس لديك صلاحية لإرسال الإشعارات");

        var test = await repository.GetByIdAsync(testId, cancellationToken)
            ?? throw new KeyNotFoundException("الاختبار غير موجود");

        var phones = new List<string>();
        if (!string.IsNullOrWhiteSpace(test.Student?.FatherPhone))
            phones.Add(test.Student.FatherPhone);
        if (!string.IsNullOrWhiteSpace(test.Student?.FatherPhone2))
            phones.Add(test.Student.FatherPhone2!);

        if (phones.Count == 0)
            throw new ValidationException("لا يوجد رقم هاتف لولي الأمر");

        var title = request.Title.Trim();
        var body = request.Body.Trim();
        var data = new Dictionary<string, string>
        {
            ["kind"] = "test_certificate",
            ["testId"] = test.Id.ToString(),
            ["studentId"] = test.StudentId.ToString(),
            ["title"] = title,
            ["body"] = body,
        };

        return await pushNotificationService.SendToParentPhonesAsync(
            phones,
            title,
            body,
            data,
            $"test certificate {testId}",
            cancellationToken);
    }

    private static decimal ResolveScore(decimal? score) => score ?? 0m;
}
