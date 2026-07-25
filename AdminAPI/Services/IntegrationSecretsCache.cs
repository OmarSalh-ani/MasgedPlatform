using Masged.WhatsApp.Options;

namespace AdminAPI.Services;

/// <summary>In-memory DB overrides for Wasender/Agora. Env/appsettings remain fallback.</summary>
public sealed class IntegrationSecretsCache : IWasenderRuntimeOverride
{
    private readonly object _lock = new();
    private string? _wasenderApiToken;
    private string? _wasenderSessionApiKey;
    private string? _agoraAppId;
    private string? _agoraAppCertificate;

    public string? ApiToken
    {
        get { lock (_lock) return _wasenderApiToken; }
    }

    public string? SessionApiKey
    {
        get { lock (_lock) return _wasenderSessionApiKey; }
    }

    public string? AgoraAppId
    {
        get { lock (_lock) return _agoraAppId; }
    }

    public string? AgoraAppCertificate
    {
        get { lock (_lock) return _agoraAppCertificate; }
    }

    public void Replace(
        string? wasenderApiToken,
        string? wasenderSessionApiKey,
        string? agoraAppId,
        string? agoraAppCertificate)
    {
        lock (_lock)
        {
            _wasenderApiToken = Normalize(wasenderApiToken);
            _wasenderSessionApiKey = Normalize(wasenderSessionApiKey);
            _agoraAppId = Normalize(agoraAppId);
            _agoraAppCertificate = Normalize(agoraAppCertificate);
        }
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
