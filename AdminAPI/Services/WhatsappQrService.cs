using AdminAPI.DTOs.WhatsappQr;
using AdminAPI.Services.Interfaces;
using FluentValidation;
using Masged.WhatsApp;
using Masged.WhatsApp.Interfaces;
using Masged.WhatsApp.Services;

namespace AdminAPI.Services;

public class WhatsappQrService(
    IWasenderSessionClient wasenderSessionClient,
    IWhatsappSessionStore sessionStore,
    WasenderSessionKeySyncService sessionKeySync,
    ICurrentUserContext currentUser) : IWhatsappQrService
{
    public async Task<WhatsappQrStatusDto> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        var sessionId = await sessionStore.GetSessionIdAsync(cancellationToken);
        return await LoadQrAsync(sessionId, cancellationToken);
    }

    public Task<WhatsappQrStatusDto> RefreshAsync(CancellationToken cancellationToken = default) =>
        GetStatusAsync(cancellationToken);

    public async Task<WhatsappQrStatusDto> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        var sessionId = await sessionStore.GetSessionIdAsync(cancellationToken);
        var result = await wasenderSessionClient.GetSessionDetailsAsync(sessionId, cancellationToken);

        if (!result.Success)
        {
            if (IsSessionNotFoundError(result.Error))
                return SessionNotFoundStatus();

            return ErrorStatus(result.Error ?? "Could not connect to server");
        }

        if (string.Equals(result.Status, "connected", StringComparison.OrdinalIgnoreCase))
        {
            await SyncSessionApiKeyAsync(result.ApiKey, cancellationToken);
            return new WhatsappQrStatusDto
            {
                StatusText = "✅ WhatsApp Connected and Ready!",
                BodyHtml = "<p>WhatsApp is authenticated and ready to send messages.</p>",
                IsConnected = true,
                ShowDisconnect = true,
            };
        }

        return await LoadQrAsync(sessionId, cancellationToken);
    }

    public async Task<WhatsappQrStatusDto> CreateSessionAsync(
        CreateWhatsappSessionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        var phone = WasenderPhoneFormatter.FormatForWasender(request.PhoneNumber);
        if (string.IsNullOrEmpty(phone))
        {
            return new WhatsappQrStatusDto
            {
                StatusText = "❌ أدخل رقماً صالحاً (مثال كويت: 51234567 أو +96551234567، أو رقم دولي بصيغة +رمز الدولة)",
                BodyHtml = "<p>أدخل رقم الواتساب بصيغة دولية مع + أو رقم كويتي محلي (8 أرقام).</p>",
                ShowCreateSession = true,
            };
        }

        var created = await wasenderSessionClient.CreateSessionReplacingIfNeededAsync(
            sessionStore.SessionName,
            phone,
            cancellationToken);

        if (!created.Success)
        {
            return new WhatsappQrStatusDto
            {
                StatusText = "❌ Failed to create session: " + (created.Error ?? string.Empty),
                BodyHtml = "<p>Error: " + (created.Error ?? string.Empty) + "</p>",
                ShowCreateSession = true,
                ShowReconnect = true,
            };
        }

        if (created.SessionId.HasValue)
            await sessionStore.SetSessionIdAsync(created.SessionId.Value, cancellationToken);

        await sessionKeySync.SyncFromSessionDetailsAsync(cancellationToken);

        return await LoadQrAsync(created.SessionId ?? await sessionStore.GetSessionIdAsync(cancellationToken), cancellationToken);
    }

    public async Task<WhatsappQrStatusDto> DisconnectAsync(CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        var sessionId = await sessionStore.GetSessionIdAsync(cancellationToken);
        var result = await wasenderSessionClient.DisconnectSessionAsync(sessionId, cancellationToken);

        if (!result.Success)
        {
            return new WhatsappQrStatusDto
            {
                StatusText = "❌ Failed to disconnect WhatsApp: " + (result.Error ?? string.Empty),
                ShowReconnect = true,
            };
        }

        return new WhatsappQrStatusDto
        {
            StatusText = "✅ WhatsApp Disconnected Successfully!",
            BodyHtml = "<p>WhatsApp has been disconnected. Click \"Reconnect\" to link a new number.</p>",
            ShowReconnect = true,
        };
    }

    public async Task<WhatsappQrStatusDto> ReconnectAsync(CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        var sessionId = await sessionStore.GetSessionIdAsync(cancellationToken);
        await wasenderSessionClient.DisconnectSessionAsync(sessionId, cancellationToken);
        await Task.Delay(1000, cancellationToken);
        await wasenderSessionClient.ConnectSessionAsync(sessionId, cancellationToken);
        await Task.Delay(2000, cancellationToken);
        return await LoadQrAsync(sessionId, cancellationToken);
    }

    private async Task<WhatsappQrStatusDto> LoadQrAsync(int sessionId, CancellationToken cancellationToken)
    {
        var connect = await wasenderSessionClient.ConnectSessionAsync(sessionId, cancellationToken);
        if (!connect.Success && !string.IsNullOrEmpty(connect.Error))
        {
            if (IsSessionNotFoundError(connect.Error))
                return SessionNotFoundStatus();

            return ErrorStatus(connect.Error);
        }

        if (string.Equals(connect.Status, "connected", StringComparison.OrdinalIgnoreCase))
        {
            await sessionKeySync.SyncFromSessionDetailsAsync(cancellationToken);
            return new WhatsappQrStatusDto
            {
                StatusText = "✅ WhatsApp Already Connected!",
                BodyHtml = "<p>WhatsApp is already authenticated and ready to use.</p>",
                IsConnected = true,
                ShowDisconnect = true,
            };
        }

        var qrCode = connect.QrCode;
        if (string.IsNullOrEmpty(qrCode))
        {
            var qrResult = await wasenderSessionClient.GetQrCodeAsync(sessionId, cancellationToken);
            qrCode = qrResult.QrCode;
        }

        var dataUrl = WhatsappQrImageHelper.ToDataUrl(qrCode);
        if (!string.IsNullOrEmpty(dataUrl))
        {
            return new WhatsappQrStatusDto
            {
                StatusText = "✅ QR Code Ready - Scan with WhatsApp!" + KuwaitTime.Now.ToString("hh:mm:ss tt"),
                QrImageDataUrl = dataUrl,
                ShowDisconnect = true,
            };
        }

        return new WhatsappQrStatusDto
        {
            StatusText = "⏳ Waiting for QR code to be generated...",
            BodyHtml = "<p>Please wait while the QR code is being generated...</p>",
            ShowReconnect = true,
        };
    }

    private async Task SyncSessionApiKeyAsync(string? apiKey, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(apiKey))
            await sessionStore.SetSessionApiKeyAsync(apiKey, cancellationToken);
    }

    private static WhatsappQrStatusDto SessionNotFoundStatus() =>
        new()
        {
            StatusText = "⚠️ Session not found. Click \"Create Session\" to create a new session.",
            BodyHtml = "<p>No session exists yet. Click \"Create Session\" below to create one, then scan the QR code.</p>",
            ShowCreateSession = true,
            ShowReconnect = true,
        };

    private static WhatsappQrStatusDto ErrorStatus(string? error) =>
        new()
        {
            StatusText = "❌ Error: " + (error ?? "Could not connect to server"),
            BodyHtml = "<p>Error: " + (error ?? string.Empty) + "</p>",
            ShowReconnect = true,
            ShowCreateSession = true,
        };

    private static bool IsSessionNotFoundError(string? error)
    {
        if (string.IsNullOrEmpty(error))
            return false;

        var lower = error.ToLowerInvariant();
        return lower.Contains("not found") || lower.Contains("404") || lower.Contains("session does not exist");
    }

    private void EnsureCanModify()
    {
        if (!currentUser.CanModify)
            throw new UnauthorizedAccessException("ليس لديك صلاحية للوصول إلى هذه الصفحة");
    }
}
