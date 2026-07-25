using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.SpecialStudentsReport;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminspecialstudentsreport")]
public class AdminSpecialStudentsReportController(
    ISpecialStudentsReportService specialStudentsReportService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<SpecialStudentsReportResponseDto>>> GetReport(
        CancellationToken cancellationToken)
    {
        var data = await specialStudentsReportService.GetReportAsync(cancellationToken);
        return Ok(new ApiResponseDto<SpecialStudentsReportResponseDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export(CancellationToken cancellationToken)
    {
        var result = await specialStudentsReportService.ExportAsync(cancellationToken);
        if (result is null)
            return BadRequest("لا يوجد طلاب مميزين لتصديرهم");

        return File(
            result.Value.Bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            result.Value.FileName);
    }
}
