using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Tests;
using AdminAPI.Services;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/admintests")]
public class AdminTestsController(ITestsReportService testsReportService) : ControllerBase
{
    [HttpGet("filter-options")]
    public async Task<ActionResult<ApiResponseDto<TestsReportFilterOptionsDto>>> GetFilterOptions(
        CancellationToken cancellationToken)
    {
        var data = await testsReportService.GetFilterOptionsAsync(cancellationToken);
        return Ok(new ApiResponseDto<TestsReportFilterOptionsDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<TestsReportListResponseDto>>> GetReport(
        [FromQuery] string fromDate,
        [FromQuery] string toDate,
        [FromQuery] int? circleId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var data = await testsReportService.GetReportAsync(
                fromDate, toDate, circleId, pageNumber, pageSize, cancellationToken);

            return Ok(new ApiResponseDto<TestsReportListResponseDto>
            {
                Success = true,
                Message = "OK",
                Data = data,
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<TestsReportListResponseDto>
            {
                Success = false,
                Message = ex.Message,
            });
        }
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportReport(
        [FromQuery] string fromDate,
        [FromQuery] string toDate,
        [FromQuery] int? circleId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var bytes = await testsReportService.ExportReportExcelAsync(
                fromDate, toDate, circleId, cancellationToken);

            var fileName = $"تقرير_الاختبارات_{KuwaitTime.Now:yyyy-MM-dd}.xlsx";
            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
