using AdminAPI.Data;
using AdminAPI.DTOs.WhatsappPending;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class WhatsappPendingRepository(AdminDbContext db) : IWhatsappPendingRepository
{
    public async Task<List<WhatsappPendingMessageDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rows = await db.WhatsappTempTables
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return rows.Select(x =>
        {
            var message = x.Message ?? string.Empty;
            return new WhatsappPendingMessageDto
            {
                Id = x.Id,
                Mobile = x.Mobile,
                MessagePreview = message.Length > 80 ? message[..80] + "..." : message,
                HasImage = !string.IsNullOrEmpty(x.Image),
            };
        }).ToList();
    }

    public async Task<int> DeleteByIdsAsync(
        IReadOnlyList<int> ids,
        CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
            return 0;

        var entities = await db.WhatsappTempTables
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

        db.WhatsappTempTables.RemoveRange(entities);
        await db.SaveChangesAsync(cancellationToken);
        return entities.Count;
    }

    public async Task<int> DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        var count = await db.WhatsappTempTables.CountAsync(cancellationToken);
        if (count == 0)
            return 0;

        db.WhatsappTempTables.RemoveRange(db.WhatsappTempTables);
        await db.SaveChangesAsync(cancellationToken);
        return count;
    }
}
