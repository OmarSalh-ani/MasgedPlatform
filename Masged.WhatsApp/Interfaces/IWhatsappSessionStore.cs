namespace Masged.WhatsApp.Interfaces;

public interface IWhatsappSessionStore
{
    string SessionName { get; }

    Task<int> GetSessionIdAsync(CancellationToken cancellationToken = default);

    Task SetSessionIdAsync(int sessionId, CancellationToken cancellationToken = default);

    Task<string?> GetSessionApiKeyAsync(CancellationToken cancellationToken = default);

    Task SetSessionApiKeyAsync(string apiKey, CancellationToken cancellationToken = default);
}
