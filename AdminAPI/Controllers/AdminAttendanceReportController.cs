using AdminAPI.DTOs.AttendanceReport;
using AdminAPI.DTOs.Common;
using AdminAPI.Services;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminattendancereport")]
public class AdminAttendanceReportController(IAttendanceReportService attendanceReportService) : ControllerBase
{
    [HttpGet("filter-options")]
    public async Task<ActionResult<ApiResponseDto<AttendanceReportFilterOptionsDto>>> GetFilterOptions(
        CancellationToken cancellationToken)
    {
        var data = await attendanceReportService.GetFilterOptionsAsync(cancellationToken);
        return Ok(new ApiResponseDto<AttendanceReportFilterOptionsDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<AttendanceReportListResponseDto>>> GetReport(
        [FromQuery] string fromDate,
        [FromQuery] string toDate,
        [FromQuery] int? circleId,
        [FromQuery] int? teacherId,
        [FromQuery] string attendanceFilter = "all",
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        if (!DateTime.TryParse(fromDate, out var from) || !DateTime.TryParse(toDate, out var to))
            return BadRequest(new ApiResponseDto<AttendanceReportListResponseDto>
            {
                Success = false,
                Message = "يرجى تحديد تاريخ البداية والنهاية",
            });

        var data = await attendanceReportService.GetReportAsync(
            from, to, circleId, teacherId, attendanceFilter, pageNumber, pageSize, cancellationToken);

        return Ok(new ApiResponseDto<AttendanceReportListResponseDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportReport(
        [FromQuery] string fromDate,
        [FromQuery] string toDate,
        [FromQuery] int? circleId,
        [FromQuery] int? teacherId,
        [FromQuery] string attendanceFilter = "all",
        CancellationToken cancellationToken = default)
    {
        if (!DateTime.TryParse(fromDate, out var from) || !DateTime.TryParse(toDate, out var to))
            return BadRequest("يرجى تحديد تاريخ البداية والنهاية");

        var bytes = await attendanceReportService.ExportReportExcelAsync(
            from, to, circleId, teacherId, attendanceFilter, cancellationToken);

        var fileName = $"AttendanceReport_{KuwaitTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [HttpPost("whatsapp")]
    public async Task<ActionResult<ApiResponseDto<string>>> SendWhatsapp(
        [FromForm] string studentIds,
        [FromForm] string message,
        IFormFile? image,
        CancellationToken cancellationToken = default)
    {
        var ids = ParseStudentIds(studentIds);
        var request = new SendAttendanceWhatsappRequestDto
        {
            StudentIds = ids,
            Message = message ?? string.Empty,
        };

        string? base64Image = null;
        if (image is { Length: > 0 } && IsImageFile(image.FileName))
        {
            await using var stream = image.OpenReadStream();
            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory, cancellationToken);
            base64Image = Convert.ToBase64String(memory.ToArray());
        }

        var result = await attendanceReportService.SendWhatsappAsync(request, base64Image, cancellationToken);
        return Ok(new ApiResponseDto<string>
        {
            Success = true,
            Message = result,
            Data = result,
        });
    }

    [HttpPost("departures")]
    public async Task<ActionResult<ApiResponseDto<SaveDepartureResultDto>>> SaveDepartures(
        [FromBody] List<SaveDepartureItemDto> items,
        CancellationToken cancellationToken = default)
    {
        var data = await attendanceReportService.SaveDeparturesAsync(items, cancellationToken);
        return Ok(new ApiResponseDto<SaveDepartureResultDto>
        {
            Success = true,
            Message = data.Message,
            Data = data,
        });
    }

    private static List<int> ParseStudentIds(string selectedStudentIds)
    {
        if (string.IsNullOrWhiteSpace(selectedStudentIds))
            return [];

        return selectedStudentIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(id => int.TryParse(id, out var parsed) ? parsed : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();
    }

    private static bool IsImageFile(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext is ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp";
    }
}
