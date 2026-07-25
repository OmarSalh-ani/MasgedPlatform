using System.Net.Http.Json;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Options;
using Microsoft.Extensions.Options;

namespace MasgedTeacherMobileAPI.Services;

public sealed class ChatBroadcastClient(
    HttpClient http,
    IOptions<ChatSettings> options,
    ILogger<ChatBroadcastClient> logger)
{
    public async Task TryBroadcastReceivedMessage(ChatMessageDto message, CancellationToken cancellationToken)
    {
        var key = options.Value.InternalBroadcastKey;
        var baseUrl = options.Value.ParentApiBaseUrl?.TrimEnd('/');
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(baseUrl))
        {
            logger.LogWarning("Chat broadcast skipped: ParentApiBaseUrl or InternalBroadcastKey not configured.");
            return;
        }

        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/api/chat/internal/broadcast-message");
            req.Headers.TryAddWithoutValidation(ChatInternal.HeaderName, key);
            req.Content = JsonContent.Create(message);

            using var resp = await http.SendAsync(req, cancellationToken).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to relay chat broadcast to Parent API.");
        }
    }
}
