using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Home;
using AdminAPI.DTOs.WhatsappSender;
using AdminAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminwhatsappsender")]
public class AdminWhatsappSenderController(
    IWhatsappSenderService service,
    IValidator<SendWhatsappSenderRequestDto> sendValidator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<HomeStudentListItemDto>>> GetList(
        [FromQuery] HomeListFiltersDto filters,
        CancellationToken cancellationToken) =>
        Ok(await service.GetListAsync(filters, cancellationToken));

    [HttpGet("filter-options")]
    public async Task<ActionResult<ApiResponseDto<HomeFilterOptionsDto>>> GetFilterOptions(
        CancellationToken cancellationToken)
    {
        var data = await service.GetFilterOptionsAsync(cancellationToken);
        return Ok(new ApiResponseDto<HomeFilterOptionsDto> { Success = true, Message = "OK", Data = data });
    }

    [HttpGet("form-options")]
    public async Task<ActionResult<ApiResponseDto<List<WhatsappSenderFormOptionDto>>>> GetFormOptions(
        CancellationToken cancellationToken)
    {
        var data = await service.GetFormOptionsAsync(cancellationToken);
        return Ok(new ApiResponseDto<List<WhatsappSenderFormOptionDto>>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpPost("whatsapp")]
    public async Task<ActionResult<ApiResponseDto<string>>> SendWhatsapp(
        [FromForm] string studentIds,
        [FromForm] string message,
        [FromForm] int? formId,
        IFormFile? image,
        CancellationToken cancellationToken)
    {
        string? base64Image = null;
        if (image is { Length: > 0 } && IsImageFile(image.FileName))
        {
            await using var stream = image.OpenReadStream();
            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory, cancellationToken);
            base64Image = Convert.ToBase64String(memory.ToArray());
        }

        var request = new SendWhatsappSenderRequestDto
        {
            StudentIds = ParseStudentIds(studentIds),
            Message = message ?? string.Empty,
            FormId = formId,
        };

        await sendValidator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await service.SendWhatsappAsync(request, base64Image, cancellationToken);
        return Ok(new ApiResponseDto<string> { Success = true, Message = result, Data = result });
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
