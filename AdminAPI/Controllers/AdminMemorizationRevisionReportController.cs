using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.MemorizationRevisionReport;
using AdminAPI.Services;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminmemorizationrevisionreport")]
public class AdminMemorizationRevisionReportController(
    IMemorizationRevisionReportService memorizationRevisionReportService) : ControllerBase
{
    [HttpGet("students")]
    public async Task<ActionResult<ApiResponseDto<List<MemorizationRevisionStudentPickDto>>>> GetStudents(
        CancellationToken cancellationToken)
    {
        var data = await memorizationRevisionReportService.GetStudentsAsync(cancellationToken);
        return Ok(new ApiResponseDto<List<MemorizationRevisionStudentPickDto>>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("{studentId:int}")]
    public async Task<ActionResult<ApiResponseDto<MemorizationRevisionReportResponseDto>>> GetReport(
        int studentId,
        CancellationToken cancellationToken)
    {
        if (studentId <= 0)
        {
            return BadRequest(new ApiResponseDto<MemorizationRevisionReportResponseDto>
            {
                Success = false,
                Message = "معرّف الطالب غير صالح.",
            });
        }

        var data = await memorizationRevisionReportService.GetReportAsync(studentId, cancellationToken);
        if (data is null)
        {
            return NotFound(new ApiResponseDto<MemorizationRevisionReportResponseDto>
            {
                Success = false,
                Message = "الطالب غير موجود.",
            });
        }

        return Ok(new ApiResponseDto<MemorizationRevisionReportResponseDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("{studentId:int}/export")]
    public async Task<IActionResult> ExportFullReport(int studentId, CancellationToken cancellationToken)
    {
        if (studentId <= 0)
            return BadRequest("معرّف الطالب غير صالح.");

        if (!await memorizationRevisionReportService.StudentExistsAsync(studentId, cancellationToken))
            return NotFound("الطالب غير موجود.");

        var result = await memorizationRevisionReportService.ExportFullReportAsync(studentId, cancellationToken);
        if (result is null)
            return BadRequest("لا توجد بيانات حفظ أو مراجعة لهذا الطالب.");

        return File(
            result.Value.Bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            result.Value.FileName);
    }

    [HttpGet("{studentId:int}/export-completed-surahs")]
    public async Task<IActionResult> ExportCompletedSurahs(int studentId, CancellationToken cancellationToken)
    {
        if (studentId <= 0)
            return BadRequest("معرّف الطالب غير صالح.");

        if (!await memorizationRevisionReportService.StudentExistsAsync(studentId, cancellationToken))
            return NotFound("الطالب غير موجود.");

        var result = await memorizationRevisionReportService.ExportCompletedSurahsAsync(
            studentId,
            cancellationToken);

        if (result is null)
            return BadRequest("لا توجد سجلات بحالة «تم» لهذا الطالب.");

        return File(
            result.Value.Bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            result.Value.FileName);
    }
}
