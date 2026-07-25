using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.PlanLevels;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/adminplanlevels")]
public class AdminPlanLevelsController(IPlanLevelService planLevelService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<PlanLevelListItemDto>>> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 0,
        CancellationToken cancellationToken = default)
    {
        var data = await planLevelService.GetListAsync(pageNumber, pageSize, cancellationToken);
        return Ok(data);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(
        int id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await planLevelService.DeleteAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? "تم الحذف" : "المستوى غير موجود",
            Data = deleted
        });
    }
}
