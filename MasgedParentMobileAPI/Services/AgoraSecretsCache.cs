namespace MasgedParentMobileAPI.Services;

public sealed class AgoraSecretsCache
{
    private readonly object _lock = new();
    private string? _appId;
    private string? _appCertificate;

    public string? AppId
    {
        get { lock (_lock) return _appId; }
    }

    public string? AppCertificate
    {
        get { lock (_lock) return _appCertificate; }
    }

    public void Replace(string? appId, string? appCertificate)
    {
        lock (_lock)
        {
            _appId = string.IsNullOrWhiteSpace(appId) ? null : appId.Trim();
            _appCertificate = string.IsNullOrWhiteSpace(appCertificate) ? null : appCertificate.Trim();
        }
    }
}
