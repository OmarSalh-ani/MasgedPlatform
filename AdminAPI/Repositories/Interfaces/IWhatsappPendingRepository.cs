using AdminAPI.DTOs.WhatsappPending;

namespace AdminAPI.Repositories.Interfaces;

public interface IWhatsappPendingRepository
{
    Task<List<WhatsappPendingMessageDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> DeleteByIdsAsync(IReadOnlyList<int> ids, CancellationToken cancellationToken = default);
    Task<int> DeleteAllAsync(CancellationToken cancellationToken = default);
}
