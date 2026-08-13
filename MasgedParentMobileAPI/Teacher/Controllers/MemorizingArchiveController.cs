using System.Security.Claims;
using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Services;
using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasgedTeacherMobileAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/memorizing-archive")]
public class MemorizingArchiveController(
    AppDbContext db,
    MemorizingArchiveService archiveService) : ControllerBase
{
    [HttpGet("{studentId:int}")]
    public async Task<IActionResult> GetArchive(
        int studentId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? surahSearch = null,
        [FromQuery] string? typeFilter = null,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
            return this.ToActionResult(GlobalResponse.BadRequest("رقم الصفحة غير صالح"));

        if (!TryGetCircleId(out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("حلقة المعلم غير محددة"));

        var result = await archiveService.GetForTeacherAsync(
            db,
            studentId,
            circleId,
            surahSearch,
            typeFilter,
            page,
            pageSize,
            cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(result));
    }

    [HttpPost("{studentId:int}/review")]
    public async Task<IActionResult> CreateJuzHizbReview(
        int studentId,
        [FromBody] CreateJuzHizbReviewDto request,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetTeacherContext(out var teacherId, out var circleId))
            return this.ToActionResult(GlobalResponse.Unauthorized());

        var (item, error) = await archiveService.CreateJuzHizbReviewAsync(
            db,
            studentId,
            teacherId,
            circleId,
            request,
            cancellationToken);

        if (error is not null)
            return this.ToActionResult(GlobalResponse.BadRequest(error));

        return this.ToActionResult(GlobalResponse.Created(item, "تم حفظ المراجعة بنجاح"));
    }

    private bool TryGetCircleId(out int circleId)
    {
        circleId = 0;
        var circleIdClaim = User.FindFirstValue("circleId");
        return int.TryParse(circleIdClaim, out circleId) && circleId > 0;
    }

    private bool TryGetTeacherContext(out int teacherId, out int circleId)
    {
        teacherId = 0;
        circleId = 0;

        var idClaim = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        var circleIdClaim = User.FindFirstValue("circleId");

        return int.TryParse(idClaim, out teacherId) && teacherId > 0
               && int.TryParse(circleIdClaim, out circleId) && circleId > 0;
    }
}
