using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.PlanLevels;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/adminplanlevel")]
public class AdminPlanLevelController(IPlanLevelService planLevelService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<PlanLevelDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var data = await planLevelService.GetByIdAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<PlanLevelDto>
        {
            Success = true,
            Message = "OK",
            Data = data
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<PlanLevelDto>>> Create(
        [FromBody] SavePlanLevelRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await planLevelService.CreateAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<PlanLevelDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<PlanLevelDto>>> Update(
        int id,
        [FromBody] SavePlanLevelRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await planLevelService.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponseDto<PlanLevelDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(
        int id,
        CancellationToken cancellationToken)
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
