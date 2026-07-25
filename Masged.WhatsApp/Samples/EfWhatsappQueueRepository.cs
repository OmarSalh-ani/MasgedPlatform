// Copy into your app. Replace YourAppDbContext with your EF DbContext type.
using Masged.WhatsApp.Interfaces;
using Masged.WhatsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace YourApp.WhatsApp;

public class EfWhatsappQueueRepository(YourAppDbContext db) : IWhatsappQueueRepository
{
    public async Task<IReadOnlyList<WhatsappQueueItem>> DequeueBatchAsync(
        int batchSize,
        CancellationToken cancellationToken = default)
    {
        var rows = await db.WhatsappTempTables
            .OrderBy(x => x.Id)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        return rows
            .Select(x => new WhatsappQueueItem(x.Id, x.Mobile, x.Message, x.Image))
            .ToList();
    }

    public async Task RemoveAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.WhatsappTempTables
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
            return;

        db.WhatsappTempTables.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }
}

// Placeholder — rename to your DbContext:
public class YourAppDbContext : DbContext
{
    public DbSet<WhatsappTempTableEntity> WhatsappTempTables => Set<WhatsappTempTableEntity>();
    public YourAppDbContext(DbContextOptions<YourAppDbContext> options) : base(options) { }
}
