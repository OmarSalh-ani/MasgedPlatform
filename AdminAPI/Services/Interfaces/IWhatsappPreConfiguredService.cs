using AdminAPI.DTOs.WhatsappPreConfigured;

namespace AdminAPI.Services.Interfaces;

public interface IWhatsappPreConfiguredService
{
    Task<List<WhatsappPreConfiguredMessageDto>> GetListAsync(CancellationToken cancellationToken = default);
    Task<WhatsappPreConfiguredMessageDto> UpdateAsync(
        int id,
        UpdateWhatsappPreConfiguredRequestDto request,
        CancellationToken cancellationToken = default);
    Task<WhatsappPreConfiguredMessageDto> SetEnabledAsync(
        int id,
        SetWhatsappPreConfiguredEnabledRequestDto request,
        CancellationToken cancellationToken = default);
    Task<string> GetTestPreviewAsync(int id, CancellationToken cancellationToken = default);
}
