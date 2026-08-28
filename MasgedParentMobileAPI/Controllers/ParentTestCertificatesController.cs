using System.Security.Claims;
using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Helpers;
using MasgedParentMobileAPI.Models;
using MasgedTeacherMobileAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasgedParentMobileAPI.Controllers;

[ApiController]
[Route("api/parent/test-certificates")]
[Authorize]
public sealed class ParentTestCertificatesController(
    NewMasgedTeacherAPIDBContext db,
    IConfiguration configuration) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ParentTestCertificateListItemDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var studentIds = await ResolveParentStudentIdsAsync(cancellationToken);
        if (studentIds.Count == 0)
            return Ok(new List<ParentTestCertificateListItemDto>());

        var tests = await db.TestHeads
            .AsNoTracking()
            .Include(t => t.Student)
            .Where(t => studentIds.Contains(t.StudentId))
            .OrderByDescending(t => t.TestDate)
            .ThenByDescending(t => t.Id)
            .ToListAsync(cancellationToken);

        var items = tests
            .Where(t => t.Student is not null)
            .Select(t => ParentTestCertificateHelper.BuildListItem(t, t.Student!))
            .ToList();

        return Ok(items);
    }

    [HttpGet("{testId:int}")]
    public async Task<IActionResult> GetCertificate(
        int testId,
        [FromQuery] string? testPeriod,
        CancellationToken cancellationToken)
    {
        var result = await LoadCertificateAsync(testId, testPeriod, cancellationToken);
        if (result.Error is not null)
            return result.Error;

        return Ok(result.Dto);
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
        var studentIds = await ResolveParentStudentIdsAsync(cancellationToken);
        if (studentIds.Count == 0)
            return CertificateLoadResult.Fail(NotFound("لم يتم العثور على الاختبار المطلوب"));

        var period = ParentTestCertificateHelper.NormalizeTestPeriod(testPeriod);

        var testHead = await db.TestHeads
            .AsNoTracking()
            .Include(t => t.Student)
            .Include(t => t.Circle)
            .FirstOrDefaultAsync(t => t.Id == testId && studentIds.Contains(t.StudentId), cancellationToken);

        if (testHead?.Student is null || testHead.Circle is null)
            return CertificateLoadResult.Fail(NotFound("لم يتم العثور على الاختبار المطلوب"));

        var dto = ParentTestCertificateHelper.BuildCertificateDto(
            testHead,
            testHead.Student,
            testHead.Circle,
            period);

        return CertificateLoadResult.Ok(dto);
    }

    private async Task<HashSet<int>> ResolveParentStudentIdsAsync(CancellationToken cancellationToken)
    {
        var fatherPhone = User.FindFirstValue("fatherPhone");
        if (string.IsNullOrEmpty(fatherPhone))
            return [];

        var variants = Services.PhoneNormalizer.GetVariants(fatherPhone).ToList();
        var ids = await db.RegisterForms
            .AsNoTracking()
            .Where(r => variants.Contains(r.FatherPhone) || variants.Contains(r.FatherPhone2))
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);

        return ids.ToHashSet();
    }

    private sealed class CertificateLoadResult
    {
        public MasgedTeacherMobileAPI.Dtos.TestCertificateDto? Dto { get; init; }

        public IActionResult? Error { get; init; }

        public static CertificateLoadResult Ok(MasgedTeacherMobileAPI.Dtos.TestCertificateDto dto) =>
            new() { Dto = dto };

        public static CertificateLoadResult Fail(IActionResult error) =>
            new() { Error = error };
    }
}
