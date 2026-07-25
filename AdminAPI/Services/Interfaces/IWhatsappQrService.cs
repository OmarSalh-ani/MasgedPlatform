using AdminAPI.DTOs.WhatsappQr;

namespace AdminAPI.Services.Interfaces;

public interface IWhatsappQrService
{
    Task<WhatsappQrStatusDto> GetStatusAsync(CancellationToken cancellationToken = default);
    Task<WhatsappQrStatusDto> RefreshAsync(CancellationToken cancellationToken = default);
    Task<WhatsappQrStatusDto> CheckHealthAsync(CancellationToken cancellationToken = default);
    Task<WhatsappQrStatusDto> CreateSessionAsync(
        CreateWhatsappSessionRequestDto request,
        CancellationToken cancellationToken = default);
    Task<WhatsappQrStatusDto> DisconnectAsync(CancellationToken cancellationToken = default);
    Task<WhatsappQrStatusDto> ReconnectAsync(CancellationToken cancellationToken = default);
}
