using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.WhatsappQr;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminwhatsappqr")]
public class AdminWhatsappQrController(IWhatsappQrService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<WhatsappQrStatusDto>>> GetStatus(
        CancellationToken cancellationToken) =>
        Ok(await WrapAsync(service.GetStatusAsync(cancellationToken)));

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponseDto<WhatsappQrStatusDto>>> Refresh(
        CancellationToken cancellationToken) =>
        Ok(await WrapAsync(service.RefreshAsync(cancellationToken)));

    [HttpPost("check-health")]
    public async Task<ActionResult<ApiResponseDto<WhatsappQrStatusDto>>> CheckHealth(
        CancellationToken cancellationToken) =>
        Ok(await WrapAsync(service.CheckHealthAsync(cancellationToken)));

    [HttpPost("create-session")]
    public async Task<ActionResult<ApiResponseDto<WhatsappQrStatusDto>>> CreateSession(
        [FromBody] CreateWhatsappSessionRequestDto request,
        CancellationToken cancellationToken) =>
        Ok(await WrapAsync(service.CreateSessionAsync(request, cancellationToken)));

    [HttpPost("disconnect")]
    public async Task<ActionResult<ApiResponseDto<WhatsappQrStatusDto>>> Disconnect(
        CancellationToken cancellationToken) =>
        Ok(await WrapAsync(service.DisconnectAsync(cancellationToken)));

    [HttpPost("reconnect")]
    public async Task<ActionResult<ApiResponseDto<WhatsappQrStatusDto>>> Reconnect(
        CancellationToken cancellationToken) =>
        Ok(await WrapAsync(service.ReconnectAsync(cancellationToken)));

    private static async Task<ApiResponseDto<WhatsappQrStatusDto>> WrapAsync(
        Task<WhatsappQrStatusDto> task)
    {
        var data = await task;
        return new ApiResponseDto<WhatsappQrStatusDto> { Success = true, Message = "OK", Data = data };
    }
}
