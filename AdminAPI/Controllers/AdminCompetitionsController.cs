using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Competitions;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/admincompetitions")]
public class AdminCompetitionsController(ICompetitionService competitionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<CompetitionListItemDto>>> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 0,
        CancellationToken cancellationToken = default)
    {
        var data = await competitionService.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        return Ok(data);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(
        int id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await competitionService.DeleteAsync(id, deleteImageFile: false, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? "تم الحذف" : "المسابقة غير موجودة",
            Data = deleted
        });
    }
}
