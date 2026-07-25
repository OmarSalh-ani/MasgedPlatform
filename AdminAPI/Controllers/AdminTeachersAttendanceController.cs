using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.TeachersAttendance;
using AdminAPI.Services;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminteachersattendance")]
public class AdminTeachersAttendanceController(ITeachersAttendanceService teachersAttendanceService)
    : ControllerBase
{
    [HttpGet("filter-options")]
    public async Task<ActionResult<ApiResponseDto<TeachersAttendanceFilterOptionsDto>>> GetFilterOptions(
        CancellationToken cancellationToken)
    {
        var data = await teachersAttendanceService.GetFilterOptionsAsync(cancellationToken);
        return Ok(new ApiResponseDto<TeachersAttendanceFilterOptionsDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<TeachersAttendanceListResponseDto>>> GetList(
        [FromQuery] string? fromDate,
        [FromQuery] string? toDate,
        [FromQuery] int? teacherId,
        CancellationToken cancellationToken = default)
    {
        var data = await teachersAttendanceService.GetListAsync(
            fromDate,
            toDate,
            teacherId,
            cancellationToken);

        return Ok(new ApiResponseDto<TeachersAttendanceListResponseDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportExcel(
        [FromQuery] string? fromDate,
        [FromQuery] string? toDate,
        [FromQuery] int? teacherId,
        CancellationToken cancellationToken = default)
    {
        var bytes = await teachersAttendanceService.ExportExcelAsync(
            fromDate,
            toDate,
            teacherId,
            cancellationToken);

        var fileName = $"TeachersAttendance_{KuwaitTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}
