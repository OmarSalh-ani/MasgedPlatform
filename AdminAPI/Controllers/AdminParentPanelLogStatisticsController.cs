using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.ParentPanelLogStatistics;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminparentpanellogstatistics")]
public class AdminParentPanelLogStatisticsController(
    IParentPanelLogStatisticsService parentPanelLogStatisticsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<ParentPanelLogStatisticsResponseDto>>> GetStatistics(
        [FromQuery] string? fromDate,
        [FromQuery] string? toDate,
        CancellationToken cancellationToken = default)
    {
        var data = await parentPanelLogStatisticsService.GetStatisticsAsync(
            fromDate,
            toDate,
            cancellationToken);

        return Ok(new ApiResponseDto<ParentPanelLogStatisticsResponseDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }
}
