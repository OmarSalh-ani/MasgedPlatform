namespace Masged.WhatsApp.Interfaces;

public interface IWasenderSessionClient
{
    Task<(bool Success, string? Status, string? ApiKey, string? Error)> GetSessionDetailsAsync(
        int sessionId,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string? QrCode, string? Status, string? Error)> ConnectSessionAsync(
        int sessionId,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string? QrCode, string? Error)> GetQrCodeAsync(
        int sessionId,
        CancellationToken cancellationToken = default);

    Task<(bool Success, int? SessionId, string? Error)> CreateSessionReplacingIfNeededAsync(
        string name,
        string phoneNumber,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string? Status, string? Error)> DisconnectSessionAsync(
        int sessionId,
        CancellationToken cancellationToken = default);
}
