using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Statistics;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminstatistics")]
public class AdminStatisticsController(IStatisticsService statisticsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<StatisticsResponseDto>>> GetStatistics(
        CancellationToken cancellationToken = default)
    {
        var data = await statisticsService.GetStatisticsAsync(cancellationToken);

        return Ok(new ApiResponseDto<StatisticsResponseDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }
}
