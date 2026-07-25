namespace Masged.WhatsApp.Interfaces;

public interface IWasenderApiClient
{
    Task<(bool Success, string? Error)> SendMessageAsync(
        string to,
        string text,
        string? imageBase64 = null,
        CancellationToken cancellationToken = default);
}
