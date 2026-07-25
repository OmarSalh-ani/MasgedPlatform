using AdminAPI.DTOs.WhatsappPending;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using FluentValidation;

namespace AdminAPI.Services;

public class WhatsappPendingService(IWhatsappPendingRepository repository) : IWhatsappPendingService
{
    public Task<List<WhatsappPendingMessageDto>> GetListAsync(CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(cancellationToken);

    public async Task<int> DeleteSelectedAsync(
        DeleteWhatsappPendingRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (request.Ids.Count == 0)
            throw new ValidationException("لم تحدد أي رسائل.");

        return await repository.DeleteByIdsAsync(request.Ids, cancellationToken);
    }

    public Task<int> DeleteAllAsync(CancellationToken cancellationToken = default) =>
        repository.DeleteAllAsync(cancellationToken);
}
