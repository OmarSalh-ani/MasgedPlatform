// Copy into your app. Inserts a row into whatsapp_temp_table for the background worker.
using Masged.WhatsApp;

namespace YourApp.WhatsApp;

public static class WhatsappEnqueueHelper
{
    public static async Task EnqueueAsync(
        YourAppDbContext db,
        string phone,
        string message,
        string? imageBase64 = null,
        int isGirl = 0,
        CancellationToken cancellationToken = default)
    {
        db.WhatsappTempTables.Add(new WhatsappTempTableEntity
        {
            Message = message,
            Mobile = PhoneNormalizer.ToWhatsappE164(phone),
            Image = imageBase64,
            IsGirl = isGirl,
        });

        await db.SaveChangesAsync(cancellationToken);
    }

    public static async Task EnqueueAsync(
        YourAppDbContext db,
        WhatsappTempTableEntity item,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(item.Mobile))
            item.Mobile = PhoneNormalizer.ToWhatsappE164(item.Mobile);

        db.WhatsappTempTables.Add(item);
        await db.SaveChangesAsync(cancellationToken);
    }
}
