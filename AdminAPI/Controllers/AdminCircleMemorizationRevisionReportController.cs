using AdminAPI.DTOs.CircleMemorizationRevisionReport;
using AdminAPI.DTOs.Common;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/admincirclememorizationrevisionreport")]
public class AdminCircleMemorizationRevisionReportController(
    ICircleMemorizationRevisionReportService reportService) : ControllerBase
{
    [HttpGet("teachers")]
    public async Task<ActionResult<ApiResponseDto<List<CircleMemorizationTeacherOptionDto>>>> GetTeachers(
        CancellationToken cancellationToken)
    {
        var data = await reportService.GetTeachersAsync(cancellationToken);
        return Ok(new ApiResponseDto<List<CircleMemorizationTeacherOptionDto>>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export(
        [FromQuery] int teacherId,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] string format = "pdf",
        CancellationToken cancellationToken = default)
    {
        if (teacherId <= 0)
            return BadRequest("يرجى اختيار المعلم.");
        if (fromDate == default || toDate == default)
            return BadRequest("يرجى تحديد من تاريخ والى تاريخ.");
        if (toDate.Date < fromDate.Date)
            return BadRequest("تاريخ النهاية يجب أن يكون بعد أو يساوي تاريخ البداية.");
        if ((toDate.Date - fromDate.Date).TotalDays > 366)
            return BadRequest("الحد الأقصى لفترة التقرير هو 365 يوم.");

        var formatKey = (format ?? "pdf").Trim().ToLowerInvariant();
        if (formatKey is not ("pdf" or "excel" or "xlsx"))
            return BadRequest("صيغة التقرير غير صالحة. استخدم pdf أو excel.");

        var result = await reportService.ExportAsync(
            teacherId, fromDate, toDate, formatKey, cancellationToken);

        if (result is null)
            return BadRequest("لا توجد بيانات حفظ أو مراجعة في الفترة المحددة");

        return File(result.Value.Bytes, result.Value.ContentType, result.Value.FileName);
    }
}
