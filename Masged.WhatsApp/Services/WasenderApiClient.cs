using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Masged.WhatsApp.Interfaces;
using Masged.WhatsApp.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Masged.WhatsApp.Services;

public class WasenderApiClient : IWasenderApiClient
{
    private readonly HttpClient _httpClient;
    private readonly WasenderApiOptions _options;
    private readonly IWasenderRuntimeOverride _runtimeOverride;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WasenderApiClient> _logger;
    private readonly object _logLock = new();

    public WasenderApiClient(
        HttpClient httpClient,
        IOptions<WasenderApiOptions> options,
        IWasenderRuntimeOverride runtimeOverride,
        IServiceScopeFactory scopeFactory,
        ILogger<WasenderApiClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _runtimeOverride = runtimeOverride;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<(bool Success, string? Error)> SendMessageAsync(
        string to,
        string text,
        string? imageBase64 = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(to))
            return (false, "Recipient phone is required");

        var formattedTo = WasenderPhoneFormatter.FormatForWasender(to);
        if (string.IsNullOrEmpty(formattedTo))
            return (false, "Recipient phone is required");

        if (string.IsNullOrWhiteSpace(text) && string.IsNullOrWhiteSpace(imageBase64))
            return (false, "Message text or image is required");

        var requestSummary =
            $" to={formattedTo} text_length={(text ?? "").Length} image={(string.IsNullOrWhiteSpace(imageBase64) ? "no" : "yes")}";

        try
        {
            string? imageUrl = null;
            if (!string.IsNullOrWhiteSpace(imageBase64))
            {
                var uploadResult = await UploadMediaAsync(imageBase64, cancellationToken).ConfigureAwait(false);
                if (!uploadResult.Success)
                {
                    LogSend(requestSummary + " [upload failed]", 0, uploadResult.Error ?? "", false);
                    return (false, "Upload failed: " + uploadResult.Error);
                }

                imageUrl = uploadResult.PublicUrl;
            }

            var bodyDict = new Dictionary<string, object?>
            {
                ["to"] = formattedTo,
                ["text"] = text ?? ""
            };
            if (!string.IsNullOrEmpty(imageUrl))
                bodyDict["imageUrl"] = imageUrl;

            var content = new StringContent(
                JsonSerializer.Serialize(bodyDict),
                Encoding.UTF8,
                "application/json");

            var token = await ResolveSendTokenAsync(cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(token))
            {
                const string missingKeyError =
                    "WhatsApp session API key is not configured. Open WhatsApp QR in admin panel and click Check Health, or set Wasender:SessionApiKey.";
                LogSend(requestSummary, 0, missingKeyError, false);
                return (false, missingKeyError);
            }

            var response = await SendWithTokenAsync(
                HttpMethod.Post,
                "send-message",
                token,
                content,
                cancellationToken).ConfigureAwait(false);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var statusCode = (int)response.StatusCode;

            if (!TryParseJsonObject(responseContent, statusCode, out var json, out var parseError))
            {
                LogSend(requestSummary, statusCode, parseError ?? responseContent, false);
                return (false, parseError ?? responseContent);
            }

            if (!response.IsSuccessStatusCode)
            {
                var message = GetJsonString(json, "message") ?? GetJsonString(json, "error") ?? responseContent;
                LogSend(requestSummary, statusCode, responseContent, false);
                return (false, message);
            }

            var success = GetJsonBool(json, "success") ?? false;
            var err = success
                ? null
                : GetJsonString(json, "message") ?? GetJsonString(json, "error") ?? responseContent;
            LogSend(requestSummary, statusCode, responseContent, success);
            return (success, err);
        }
        catch (Exception ex)
        {
            LogSend(requestSummary, 0, "Exception: " + ex.Message, false);
            return (false, ex.Message);
        }
    }

    private async Task<(bool Success, string? PublicUrl, string? Error)> UploadMediaAsync(
        string base64Data,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(base64Data))
            return (false, null, "No base64 data provided");

        try
        {
            var payload = base64Data.StartsWith("data:", StringComparison.Ordinal)
                ? base64Data
                : "data:image/jpeg;base64," + base64Data;

            var content = new StringContent(
                JsonSerializer.Serialize(new { base64 = payload }),
                Encoding.UTF8,
                "application/json");

            var response = await SendWithTokenAsync(
                HttpMethod.Post,
                "upload",
                FirstNonEmpty(_runtimeOverride.ApiToken, _options.ApiToken),
                content,
                cancellationToken).ConfigureAwait(false);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (!TryParseJsonObject(responseContent, (int)response.StatusCode, out var json, out var parseError))
                return (false, null, parseError ?? responseContent);

            if (!response.IsSuccessStatusCode)
            {
                var message = GetJsonString(json, "message") ?? GetJsonString(json, "error") ?? responseContent;
                return (false, null, message);
            }

            var success = GetJsonBool(json, "success") ?? false;
            var publicUrl = GetJsonString(json, "publicUrl");
            return (success, publicUrl, null);
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message);
        }
    }

    private async Task<HttpResponseMessage> SendWithTokenAsync(
        HttpMethod method,
        string path,
        string? token,
        HttpContent? content,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, path);
        var apiToken = FirstNonEmpty(_runtimeOverride.ApiToken, _options.ApiToken);
        var authToken = !string.IsNullOrWhiteSpace(token) ? token : apiToken;
        if (!string.IsNullOrEmpty(authToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        if (content != null)
            request.Content = content;

        return await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private async Task<string?> ResolveSendTokenAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var sync = scope.ServiceProvider.GetRequiredService<WasenderSessionKeySyncService>();
        await sync.EnsureSessionApiKeyAsync(cancellationToken).ConfigureAwait(false);

        var sessionStore = scope.ServiceProvider.GetRequiredService<IWhatsappSessionStore>();
        var storedKey = await sessionStore.GetSessionApiKeyAsync(cancellationToken).ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(storedKey))
            return storedKey;

        return FirstNonEmpty(_runtimeOverride.SessionApiKey, _options.SessionApiKey);
    }

    private static string? FirstNonEmpty(string? preferred, string? fallback) =>
        !string.IsNullOrWhiteSpace(preferred) ? preferred.Trim() : (string.IsNullOrWhiteSpace(fallback) ? null : fallback.Trim());

    private void LogSend(string requestSummary, int statusCode, string responseBodyOrError, bool success)
    {
        lock (_logLock)
        {
            try
            {
                var directory = Path.Combine(AppContext.BaseDirectory, _options.ErrorLogDirectory);
                Directory.CreateDirectory(directory);
                var path = Path.Combine(directory, "WhatsAppSendLog.txt");
                var line = new StringBuilder()
                    .AppendLine("----------------------------------------")
                    .AppendLine(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " UTC")
                    .AppendLine("REQUEST: " + requestSummary)
                    .AppendLine("HTTP STATUS: " + statusCode)
                    .AppendLine("SUCCESS: " + success)
                    .AppendLine("RESPONSE: " + (responseBodyOrError ?? "").Replace("\r", " ").Replace("\n", " "))
                    .AppendLine();
                File.AppendAllText(path, line.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to write Wasender send log");
            }
        }
    }

    private static bool TryParseJsonObject(
        string content,
        int statusCode,
        out JsonElement json,
        out string? errorMessage)
    {
        json = default;
        errorMessage = null;
        if (string.IsNullOrWhiteSpace(content))
        {
            errorMessage = $"Empty response (HTTP {statusCode}).";
            return false;
        }

        var trimmed = content.TrimStart();
        if (!trimmed.StartsWith('{') && !trimmed.StartsWith('['))
        {
            errorMessage = trimmed.StartsWith('<')
                ? $"HTTP {statusCode} - Server returned HTML instead of JSON."
                : $"Invalid response format (HTTP {statusCode}).";
            return false;
        }

        try
        {
            using var doc = JsonDocument.Parse(content);
            json = doc.RootElement.Clone();
            return true;
        }
        catch (JsonException ex)
        {
            errorMessage = "Invalid JSON response: " + ex.Message;
            return false;
        }
    }

    private static string? GetJsonString(JsonElement json, string propertyName) =>
        json.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    private static bool? GetJsonBool(JsonElement json, string propertyName) =>
        json.TryGetProperty(propertyName, out var value) && value.ValueKind is JsonValueKind.True or JsonValueKind.False
            ? value.GetBoolean()
            : null;
}
