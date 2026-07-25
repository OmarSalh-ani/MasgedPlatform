using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Masged.WhatsApp.Interfaces;
using Masged.WhatsApp.Options;
using Microsoft.Extensions.Options;

namespace Masged.WhatsApp.Services;

public class WasenderSessionClient(
    HttpClient httpClient,
    IOptions<WasenderApiOptions> options,
    IWasenderRuntimeOverride runtimeOverride) : IWasenderSessionClient
{
    private readonly WasenderApiOptions _options = options.Value;
    private readonly IWasenderRuntimeOverride _runtimeOverride = runtimeOverride;

    public Task<(bool Success, string? Status, string? ApiKey, string? Error)> GetSessionDetailsAsync(
        int sessionId,
        CancellationToken cancellationToken = default) =>
        GetSessionDataAsync($"whatsapp-sessions/{sessionId}", cancellationToken);

    public async Task<(bool Success, string? QrCode, string? Status, string? Error)> ConnectSessionAsync(
        int sessionId,
        CancellationToken cancellationToken = default)
    {
        var response = await SendAsync(
            HttpMethod.Post,
            $"whatsapp-sessions/{sessionId}/connect",
            null,
            cancellationToken).ConfigureAwait(false);

        if (!response.Success || response.Json is null)
            return (false, null, null, response.Error);

        var data = GetData(response.Json.Value);
        return (
            GetBool(response.Json.Value, "success") ?? false,
            GetString(data, "qrCode"),
            GetString(data, "status"),
            null);
    }

    public async Task<(bool Success, string? QrCode, string? Error)> GetQrCodeAsync(
        int sessionId,
        CancellationToken cancellationToken = default)
    {
        var response = await SendAsync(
            HttpMethod.Get,
            $"whatsapp-sessions/{sessionId}/qrcode",
            null,
            cancellationToken).ConfigureAwait(false);

        if (!response.Success || response.Json is null)
            return (false, null, response.Error);

        var data = GetData(response.Json.Value);
        return (GetBool(response.Json.Value, "success") ?? false, GetString(data, "qrCode"), null);
    }

    public async Task<(bool Success, int? SessionId, string? Error)> CreateSessionReplacingIfNeededAsync(
        string name,
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var formattedPhone = WasenderPhoneFormatter.FormatForWasender(phoneNumber);
        if (string.IsNullOrEmpty(formattedPhone))
            return (false, null, "Recipient phone is required");

        var result = await CreateSessionAsync(name, formattedPhone, cancellationToken).ConfigureAwait(false);
        if (result.Success)
            return result;

        var err = result.Error ?? string.Empty;
        if (!IsPhoneTakenError(err))
            return result;

        var sessions = await ListSessionsAsync(cancellationToken).ConfigureAwait(false);
        if (!sessions.Success || sessions.Sessions is null)
            return result;

        foreach (var session in sessions.Sessions)
        {
            if (NormalizePhone(session.PhoneNumber) != NormalizePhone(formattedPhone))
                continue;

            var deleted = await DeleteSessionAsync(session.Id, cancellationToken).ConfigureAwait(false);
            if (!deleted.Success)
                return (false, null, "Deleted old session failed: " + (deleted.Error ?? string.Empty));

            return await CreateSessionAsync(name, formattedPhone, cancellationToken).ConfigureAwait(false);
        }

        return result;
    }

    public Task<(bool Success, string? Status, string? Error)> DisconnectSessionAsync(
        int sessionId,
        CancellationToken cancellationToken = default) =>
        GetStatusAsync($"whatsapp-sessions/{sessionId}/disconnect", cancellationToken, HttpMethod.Post);

    private async Task<(bool Success, int? SessionId, string? Error)> CreateSessionAsync(
        string name,
        string phoneNumber,
        CancellationToken cancellationToken)
    {
        var body = JsonSerializer.Serialize(new
        {
            name = name ?? "session",
            phone_number = phoneNumber,
            account_protection = true,
            log_messages = true,
        });

        var response = await SendAsync(
            HttpMethod.Post,
            "whatsapp-sessions",
            new StringContent(body, Encoding.UTF8, "application/json"),
            cancellationToken).ConfigureAwait(false);

        if (!response.Success || response.Json is null)
            return (false, null, response.Error);

        var data = GetData(response.Json.Value);
        var id = data.TryGetProperty("id", out var idProp) && idProp.TryGetInt32(out var parsed) ? parsed : (int?)null;
        return (GetBool(response.Json.Value, "success") ?? false, id, null);
    }

    private async Task<(bool Success, List<SessionRow>? Sessions, string? Error)> ListSessionsAsync(
        CancellationToken cancellationToken)
    {
        var response = await SendAsync(HttpMethod.Get, "whatsapp-sessions", null, cancellationToken)
            .ConfigureAwait(false);

        if (!response.Success || response.Json is null)
            return (false, null, response.Error);

        var list = new List<SessionRow>();
        if (response.Json.Value.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in data.EnumerateArray())
            {
                if (!item.TryGetProperty("id", out var idProp) || !idProp.TryGetInt32(out var id) || id <= 0)
                    continue;

                list.Add(new SessionRow(
                    id,
                    GetString(item, "phone_number") ?? string.Empty));
            }
        }

        return (GetBool(response.Json.Value, "success") ?? false, list, null);
    }

    private async Task<(bool Success, string? Error)> DeleteSessionAsync(
        int sessionId,
        CancellationToken cancellationToken)
    {
        using var request = BuildRequest(HttpMethod.Delete, $"whatsapp-sessions/{sessionId}", null);
        using var httpResponse = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (httpResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
            return (true, null);

        var content = await httpResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (!httpResponse.IsSuccessStatusCode)
            return (false, content);

        return (true, null);
    }

    private async Task<(bool Success, string? Status, string? ApiKey, string? Error)> GetSessionDataAsync(
        string path,
        CancellationToken cancellationToken,
        HttpMethod? method = null)
    {
        var response = await SendAsync(method ?? HttpMethod.Get, path, null, cancellationToken)
            .ConfigureAwait(false);

        if (!response.Success || response.Json is null)
            return (false, null, null, response.Error);

        var data = GetData(response.Json.Value);
        return (
            GetBool(response.Json.Value, "success") ?? false,
            GetString(data, "status"),
            GetString(data, "api_key"),
            null);
    }

    private async Task<(bool Success, string? Status, string? Error)> GetStatusAsync(
        string path,
        CancellationToken cancellationToken,
        HttpMethod? method = null)
    {
        var response = await SendAsync(method ?? HttpMethod.Get, path, null, cancellationToken)
            .ConfigureAwait(false);

        if (!response.Success || response.Json is null)
            return (false, null, response.Error);

        var data = GetData(response.Json.Value);
        return (GetBool(response.Json.Value, "success") ?? false, GetString(data, "status"), null);
    }

    private async Task<(bool Success, JsonElement? Json, string? Error)> SendAsync(
        HttpMethod method,
        string path,
        HttpContent? content,
        CancellationToken cancellationToken)
    {
        using var request = BuildRequest(method, path, content);
        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(body))
            return response.IsSuccessStatusCode
                ? (true, null, null)
                : (false, null, $"Empty response (HTTP {(int)response.StatusCode}).");

        try
        {
            using var doc = JsonDocument.Parse(body);
            var json = doc.RootElement.Clone();
            if (!response.IsSuccessStatusCode)
            {
                var message = GetString(json, "message") ?? GetString(json, "error") ?? body;
                return (false, json, message);
            }

            return (true, json, null);
        }
        catch (JsonException ex)
        {
            return (false, null, ex.Message);
        }
    }

    private HttpRequestMessage BuildRequest(HttpMethod method, string path, HttpContent? content)
    {
        var request = new HttpRequestMessage(method, path);
        var token = !string.IsNullOrWhiteSpace(_runtimeOverride.ApiToken)
            ? _runtimeOverride.ApiToken
            : _options.ApiToken;
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (content != null)
            request.Content = content;
        return request;
    }

    private static bool IsPhoneTakenError(string error)
    {
        if (string.IsNullOrEmpty(error))
            return false;

        return error.Contains("already been taken", StringComparison.OrdinalIgnoreCase)
            || (error.Contains("phone number", StringComparison.OrdinalIgnoreCase)
                && error.Contains("taken", StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizePhone(string phone) =>
        WasenderPhoneFormatter.FormatForWasender(phone) ?? string.Empty;

    private static JsonElement GetData(JsonElement json) =>
        json.TryGetProperty("data", out var data) ? data : default;

    private static string? GetString(JsonElement json, string name) =>
        json.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    private static bool? GetBool(JsonElement json, string name) =>
        json.TryGetProperty(name, out var value) && value.ValueKind is JsonValueKind.True or JsonValueKind.False
            ? value.GetBoolean()
            : null;

    private sealed record SessionRow(int Id, string PhoneNumber);
}
