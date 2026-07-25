using AdminAPI.DTOs.WhatsappPending;

namespace AdminAPI.Services.Interfaces;

public interface IWhatsappPendingService
{
    Task<List<WhatsappPendingMessageDto>> GetListAsync(CancellationToken cancellationToken = default);
    Task<int> DeleteSelectedAsync(DeleteWhatsappPendingRequestDto request, CancellationToken cancellationToken = default);
    Task<int> DeleteAllAsync(CancellationToken cancellationToken = default);
}
