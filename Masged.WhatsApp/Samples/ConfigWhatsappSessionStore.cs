// Copy into your app. Stores session id/key in appsettings (no DB required).
using Masged.WhatsApp.Interfaces;
using Masged.WhatsApp.Options;
using Microsoft.Extensions.Configuration;

namespace YourApp.WhatsApp;

public class ConfigWhatsappSessionStore(IConfiguration configuration) : IWhatsappSessionStore
{
    private const string SessionIdKey = "WhatsappQR_SessionId";
    private const string DefaultSessionName = "Masged";

    public string SessionName => DefaultSessionName;

    public Task<int> GetSessionIdAsync(CancellationToken cancellationToken = default)
    {
        var value = configuration[SessionIdKey];
        return Task.FromResult(int.TryParse(value, out var id) ? id : 1);
    }

    public Task SetSessionIdAsync(int sessionId, CancellationToken cancellationToken = default)
    {
        // For production: persist to DB or file. Config is read-only at runtime.
        // AdminAPI uses AppSettings table instead.
        return Task.CompletedTask;
    }

    public Task<string?> GetSessionApiKeyAsync(CancellationToken cancellationToken = default)
    {
        var key = configuration[$"{WasenderApiOptions.SectionName}:SessionApiKey"];
        return Task.FromResult(string.IsNullOrWhiteSpace(key) ? null : key.Trim());
    }

    public Task SetSessionApiKeyAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        // For production: persist to DB. AdminAPI uses AppSettings table.
        return Task.CompletedTask;
    }
}
