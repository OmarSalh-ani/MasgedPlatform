using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.WorkDays;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/adminworkdays")]
public class AdminWorkDaysController(IWorkDayService workDayService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<WorkDaysDto>>> Get(CancellationToken cancellationToken)
    {
        var data = await workDayService.GetAsync(cancellationToken);
        return Ok(new ApiResponseDto<WorkDaysDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<ApiResponseDto<WorkDaysDto>>> Update(
        [FromBody] UpdateWorkDaysRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await workDayService.UpdateAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<WorkDaysDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data,
        });
    }
}
