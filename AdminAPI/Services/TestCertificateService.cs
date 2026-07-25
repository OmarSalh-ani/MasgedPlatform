using AdminAPI.DTOs.TestCertificate;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;

namespace AdminAPI.Services;

public class TestCertificateService(
    ITestCertificateRepository repository,
    ICurrentUserContext currentUser) : ITestCertificateService
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

    private static decimal ResolveScore(decimal? score) => score ?? 0m;
}
