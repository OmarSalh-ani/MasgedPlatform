using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Home;
using AdminAPI.Services;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminothaimincenter")]
public class AdminOthaiminCenterController(IOthaiminCenterService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<HomeStudentListItemDto>>> GetList(
        [FromQuery] HomeListFiltersDto filters,
        CancellationToken cancellationToken = default)
    {
        var data = await service.GetListAsync(filters, cancellationToken);
        return Ok(data);
    }

    [HttpGet("filter-options")]
    public async Task<ActionResult<ApiResponseDto<HomeFilterOptionsDto>>> GetFilterOptions(
        CancellationToken cancellationToken = default)
    {
        var data = await service.GetFilterOptionsAsync(cancellationToken);
        return Ok(new ApiResponseDto<HomeFilterOptionsDto> { Success = true, Message = "OK", Data = data });
    }

    [HttpGet("student-names")]
    public async Task<ActionResult<PagedResultDto<HomeStudentNameLookupDto>>> GetStudentNames(
        [FromQuery] HomeStudentNameLookupFiltersDto filters,
        CancellationToken cancellationToken = default)
    {
        var data = await service.GetStudentNamesAsync(filters, cancellationToken);
        return Ok(data);
    }

    [HttpGet("circle-title/{circleId:int}")]
    public async Task<ActionResult<ApiResponseDto<string>>> GetCircleTitle(
        int circleId,
        CancellationToken cancellationToken = default)
    {
        var name = await service.GetPageTitleCircleNameAsync(circleId, cancellationToken);
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
        var bytes = await service.ExportExcelAsync(filters, cancellationToken);
        var fileName = $"MrkzStudents_{KuwaitTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpPost("whatsapp")]
    public async Task<ActionResult<ApiResponseDto<string>>> SendWhatsapp(
        [FromForm] string studentIds,
        [FromForm] string message,
        IFormFile? image,
        CancellationToken cancellationToken = default)
    {
        var request = new DTOs.AttendanceReport.SendAttendanceWhatsappRequestDto
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

        var result = await service.SendWhatsappAsync(request, base64Image, cancellationToken);
        return Ok(new ApiResponseDto<string> { Success = true, Message = result, Data = result });
    }

    [HttpPost("transfer")]
    public async Task<ActionResult<ApiResponseDto<int>>> TransferStudents(
        [FromBody] TransferHomeStudentsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var count = await service.TransferStudentsAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<int> { Success = true, Message = $"تم نقل {count} طالب بنجاح", Data = count });
    }

    [HttpPost("create-circle")]
    public async Task<ActionResult<ApiResponseDto<int>>> CreateCircle(
        [FromBody] CreateHomeCircleRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var count = await service.CreateCircleAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<int> { Success = true, Message = "تم إنشاء الحلقة بنجاح", Data = count });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(
        int id,
        CancellationToken cancellationToken = default)
    {
        await service.DeleteStudentAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<bool> { Success = true, Message = "تم حذف الطالب بنجاح", Data = true });
    }

    [HttpGet("{id:int}/tests")]
    public async Task<ActionResult<ApiResponseDto<List<HomeStudentTestDto>>>> GetTests(
        int id,
        CancellationToken cancellationToken = default)
    {
        var data = await service.GetStudentTestsAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<List<HomeStudentTestDto>> { Success = true, Message = "OK", Data = data });
    }

    [HttpGet("{id:int}/reviews")]
    public async Task<ActionResult<ApiResponseDto<List<HomeStudentReviewDto>>>> GetReviews(
        int id,
        CancellationToken cancellationToken = default)
    {
        var data = await service.GetStudentReviewsAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<List<HomeStudentReviewDto>> { Success = true, Message = "OK", Data = data });
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
