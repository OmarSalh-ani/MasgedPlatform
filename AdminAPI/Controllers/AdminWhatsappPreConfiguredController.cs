using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.WhatsappPreConfigured;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminwhatsapppreconfigured")]
public class AdminWhatsappPreConfiguredController(IWhatsappPreConfiguredService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<WhatsappPreConfiguredMessageDto>>>> GetList(
        CancellationToken cancellationToken)
    {
        var data = await service.GetListAsync(cancellationToken);
        return Ok(new ApiResponseDto<List<WhatsappPreConfiguredMessageDto>>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<WhatsappPreConfiguredMessageDto>>> Update(
        int id,
        [FromBody] UpdateWhatsappPreConfiguredRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await service.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponseDto<WhatsappPreConfiguredMessageDto>
        {
            Success = true,
            Message = "تم حفظ التغييرات بنجاح",
            Data = data,
        });
    }

    [HttpPut("{id:int}/enabled")]
    public async Task<ActionResult<ApiResponseDto<WhatsappPreConfiguredMessageDto>>> SetEnabled(
        int id,
        [FromBody] SetWhatsappPreConfiguredEnabledRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await service.SetEnabledAsync(id, request, cancellationToken);
        return Ok(new ApiResponseDto<WhatsappPreConfiguredMessageDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("{id:int}/test-preview")]
    public async Task<ActionResult<ApiResponseDto<string>>> GetTestPreview(
        int id,
        CancellationToken cancellationToken)
    {
        var data = await service.GetTestPreviewAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<string> { Success = true, Message = "OK", Data = data });
    }
}
