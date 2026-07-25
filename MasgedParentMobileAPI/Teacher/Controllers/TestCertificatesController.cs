using System.Security.Claims;
using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Extensions;
using MasgedTeacherMobileAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/test-certificates")]
public class TestCertificatesController(AppDbContext db, IConfiguration configuration) : ControllerBase
{
    private static readonly string[] ValidTestPeriods =
    [
        "الفصل الأول",
        "الفصل الثاني",
        "الفصل الثالث"
    ];

    [HttpGet("{testId:int}")]
    public async Task<IActionResult> GetCertificate(
        int testId,
        [FromQuery] string? testPeriod,
        CancellationToken cancellationToken)
    {
        var result = await LoadCertificateAsync(testId, testPeriod, cancellationToken);
        if (result.Error is not null)
            return result.Error;

        return this.ToActionResult(GlobalResponse.Ok(result.Dto));
    }

    [HttpGet("{testId:int}/html")]
    [Produces("text/html")]
    public async Task<IActionResult> GetCertificateHtml(
        int testId,
        [FromQuery] string? testPeriod,
        CancellationToken cancellationToken)
    {
        var result = await LoadCertificateAsync(testId, testPeriod, cancellationToken);
        if (result.Error is not null)
            return result.Error;

        var logoBaseUrl = configuration["CertificateAssets:LogoBaseUrl"];
        var html = TestCertificateHtmlBuilder.Build(result.Dto!, logoBaseUrl);
        return Content(html, "text/html; charset=utf-8");
    }

    private async Task<CertificateLoadResult> LoadCertificateAsync(
        int testId,
        string? testPeriod,
        CancellationToken cancellationToken)
    {
        if (!TryGetCircleId(out var circleId))
            return CertificateLoadResult.Fail(this.ToActionResult(GlobalResponse.Unauthorized()));

        var period = string.IsNullOrWhiteSpace(testPeriod) ? "الفصل الأول" : testPeriod.Trim();
        if (!ValidTestPeriods.Contains(period))
            return CertificateLoadResult.Fail(this.ToActionResult(GlobalResponse.BadRequest("فترة الاختبار غير صالحة")));

        var testHead = await db.TestHeads
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == testId, cancellationToken);

        if (testHead is null)
            return CertificateLoadResult.Fail(this.ToActionResult(GlobalResponse.NotFound("لم يتم العثور على الاختبار المطلوب")));

        if (!await StudentCircleAccessHelper.CanReadRecordAsync(
                db, testHead.StudentId, circleId, testHead.CircleId, cancellationToken))
            return CertificateLoadResult.Fail(this.ToActionResult(GlobalResponse.NotFound("لم يتم العثور على الاختبار المطلوب")));

        var student = await db.RegisterForms
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == testHead.StudentId, cancellationToken);

        if (student is null)
            return CertificateLoadResult.Fail(this.ToActionResult(GlobalResponse.NotFound("لم يتم العثور على بيانات الطالب")));

        var circle = await db.QuranCircles
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == testHead.CircleId, cancellationToken);

        if (circle is null)
            return CertificateLoadResult.Fail(this.ToActionResult(GlobalResponse.NotFound("لم يتم العثور على بيانات الحلقة")));

        var dto = TestCertificateHelper.BuildDto(testHead, student, circle, period);
        return CertificateLoadResult.Ok(dto);
    }

    private bool TryGetCircleId(out int circleId)
    {
        circleId = 0;
        var circleIdClaim = User.FindFirstValue("circleId");
        return int.TryParse(circleIdClaim, out circleId) && circleId > 0;
    }

    private sealed class CertificateLoadResult
    {
        public TestCertificateDto? Dto { get; init; }
        public IActionResult? Error { get; init; }

        public static CertificateLoadResult Ok(TestCertificateDto dto) =>
            new() { Dto = dto };

        public static CertificateLoadResult Fail(IActionResult error) =>
            new() { Error = error };
    }
}
