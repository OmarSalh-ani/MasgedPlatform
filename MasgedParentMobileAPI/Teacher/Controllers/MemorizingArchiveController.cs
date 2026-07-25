using System.Security.Claims;
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
            page,
            pageSize,
            cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(result));
    }

    private bool TryGetCircleId(out int circleId)
    {
        circleId = 0;
        var circleIdClaim = User.FindFirstValue("circleId");
        return int.TryParse(circleIdClaim, out circleId) && circleId > 0;
    }
}
