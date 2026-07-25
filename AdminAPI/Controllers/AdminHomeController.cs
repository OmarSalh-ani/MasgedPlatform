using AdminAPI.DTOs.AttendanceReport;
using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Home;
using AdminAPI.Services;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminhome")]
public class AdminHomeController(IHomeService homeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<HomeStudentListItemDto>>> GetList(
        [FromQuery] HomeListFiltersDto filters,
        CancellationToken cancellationToken = default)
    {
        var data = await homeService.GetListAsync(filters, cancellationToken);
        return Ok(data);
    }

    [HttpGet("filter-options")]
    public async Task<ActionResult<ApiResponseDto<HomeFilterOptionsDto>>> GetFilterOptions(
        CancellationToken cancellationToken = default)
    {
        var data = await homeService.GetFilterOptionsAsync(cancellationToken);
        return Ok(new ApiResponseDto<HomeFilterOptionsDto> { Success = true, Message = "OK", Data = data });
    }

    [HttpGet("student-names")]
    public async Task<ActionResult<PagedResultDto<HomeStudentNameLookupDto>>> GetStudentNames(
        [FromQuery] HomeStudentNameLookupFiltersDto filters,
        CancellationToken cancellationToken = default)
    {
        var data = await homeService.GetStudentNamesAsync(filters, cancellationToken);
        return Ok(data);
    }

    [HttpGet("circle-title/{circleId:int}")]
    public async Task<ActionResult<ApiResponseDto<string>>> GetCircleTitle(
        int circleId,
        CancellationToken cancellationToken = default)
    {
        var name = await homeService.GetPageTitleCircleNameAsync(circleId, cancellationToken);
        return Ok(new ApiResponseDto<string>
        {
            Success = true,
            Message = "OK",
            Data = name == null ? string.Empty : $"قائمة طلاب حلقة {name}",
        });
    }

    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportExcel(
        [FromQuery] HomeListFiltersDto filters,
        CancellationToken cancellationToken = default)
    {
        var bytes = await homeService.ExportExcelAsync(filters, cancellationToken);
        var fileName = $"Students_{KuwaitTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpPost("whatsapp")]
    public async Task<ActionResult<ApiResponseDto<string>>> SendWhatsapp(
        [FromForm] string studentIds,
        [FromForm] string message,
        IFormFile? image,
        CancellationToken cancellationToken = default)
    {
        var request = new SendAttendanceWhatsappRequestDto
        {
            StudentIds = ParseStudentIds(studentIds),
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

        var result = await homeService.SendWhatsappAsync(request, base64Image, cancellationToken);
        return Ok(new ApiResponseDto<string> { Success = true, Message = result, Data = result });
    }

    [HttpPost("transfer")]
    public async Task<ActionResult<ApiResponseDto<int>>> TransferStudents(
        [FromBody] TransferHomeStudentsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var count = await homeService.TransferStudentsAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<int> { Success = true, Message = $"تم نقل {count} طالب بنجاح", Data = count });
    }

    [HttpPost("remove-from-circle")]
    public async Task<ActionResult<ApiResponseDto<int>>> RemoveFromCircle(
        [FromBody] RemoveHomeStudentsFromCircleRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var count = await homeService.RemoveFromCircleAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<int> { Success = true, Message = $"تم إزالة {count} طالب من الحلقات بنجاح", Data = count });
    }

    [HttpPost("create-circle")]
    public async Task<ActionResult<ApiResponseDto<int>>> CreateCircle(
        [FromBody] CreateHomeCircleRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var count = await homeService.CreateCircleAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<int> { Success = true, Message = "تم إنشاء الحلقة بنجاح", Data = count });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(
        int id,
        CancellationToken cancellationToken = default)
    {
        await homeService.DeleteStudentAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<bool> { Success = true, Message = "تم حذف الطالب بنجاح", Data = true });
    }

    [HttpGet("{id:int}/tests")]
    public async Task<ActionResult<ApiResponseDto<List<HomeStudentTestDto>>>> GetTests(
        int id,
        CancellationToken cancellationToken = default)
    {
        var data = await homeService.GetStudentTestsAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<List<HomeStudentTestDto>> { Success = true, Message = "OK", Data = data });
    }

    [HttpGet("{id:int}/reviews")]
    public async Task<ActionResult<ApiResponseDto<List<HomeStudentReviewDto>>>> GetReviews(
        int id,
        CancellationToken cancellationToken = default)
    {
        var data = await homeService.GetStudentReviewsAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<List<HomeStudentReviewDto>> { Success = true, Message = "OK", Data = data });
    }

    [HttpGet("{id:int}/qr-token")]
    public async Task<ActionResult<ApiResponseDto<StudentQrTokenDto>>> GetQrToken(
        int id,
        CancellationToken cancellationToken = default)
    {
        var data = await homeService.GetStudentQrTokenAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<StudentQrTokenDto> { Success = true, Message = "OK", Data = data });
    }

    [HttpGet("registration-settings")]
    public async Task<ActionResult<ApiResponseDto<HomeRegistrationSettingsDto>>> GetRegistrationSettings(
        CancellationToken cancellationToken = default)
    {
        var data = await homeService.GetRegistrationSettingsAsync(cancellationToken);
        return Ok(new ApiResponseDto<HomeRegistrationSettingsDto> { Success = true, Message = "OK", Data = data });
    }

    [HttpPut("registration-settings")]
    public async Task<ActionResult<ApiResponseDto<bool>>> UpdateRegistrationSettings(
        [FromBody] UpdateHomeRegistrationRequestDto request,
        CancellationToken cancellationToken = default)
    {
        await homeService.UpdateRegistrationSettingsAsync(request, cancellationToken);
        var message = request.ForGirl
            ? "تم تحديث حالة تسجيل النساء بنجاح"
            : "تم تحديث حالة تسجيل الرجال بنجاح";
        return Ok(new ApiResponseDto<bool> { Success = true, Message = message, Data = true });
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
