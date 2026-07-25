using Masged.WhatsApp.Interfaces;

namespace Masged.WhatsApp.Services;

public class WasenderSessionKeySyncService(
    IWhatsappSessionStore sessionStore,
    IWasenderSessionClient sessionClient)
{
    public async Task EnsureSessionApiKeyAsync(CancellationToken cancellationToken = default)
    {
        var existing = await sessionStore.GetSessionApiKeyAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(existing))
            return;

        await SyncFromSessionDetailsAsync(cancellationToken);
    }

    public async Task SyncFromSessionDetailsAsync(CancellationToken cancellationToken = default)
    {
        var sessionId = await sessionStore.GetSessionIdAsync(cancellationToken);
        var details = await sessionClient.GetSessionDetailsAsync(sessionId, cancellationToken);
        if (details.Success && !string.IsNullOrWhiteSpace(details.ApiKey))
            await sessionStore.SetSessionApiKeyAsync(details.ApiKey, cancellationToken);
    }
}
