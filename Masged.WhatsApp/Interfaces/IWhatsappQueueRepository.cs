using Masged.WhatsApp.Models;

namespace Masged.WhatsApp.Interfaces;

public interface IWhatsappQueueRepository
{
    Task<IReadOnlyList<WhatsappQueueItem>> DequeueBatchAsync(
        int batchSize,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(int id, CancellationToken cancellationToken = default);
}
