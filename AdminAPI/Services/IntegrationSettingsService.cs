using AdminAPI.Data;
using AdminAPI.DTOs.Integrations;
using AdminAPI.Models;
using AdminAPI.Services.Interfaces;
using Masged.WhatsApp.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class IntegrationSettingsService(
    AdminDbContext db,
    IntegrationSecretsCache cache,
    IOptions<WasenderApiOptions> wasenderOptions,
    IConfiguration configuration) : IIntegrationSettingsService
{
    public async Task<IntegrationSettingsDto> GetAsync(CancellationToken cancellationToken = default)
    {
        var row = await db.IntegrationSettings.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        var envWasender = wasenderOptions.Value;
        var envAgoraId = configuration["Agora:AppId"];
        var envAgoraCert = configuration["Agora:AppCertificate"];

        var apiToken = FirstNonEmpty(row?.WasenderApiToken, envWasender.ApiToken);
        var sessionKey = FirstNonEmpty(row?.WasenderSessionApiKey, envWasender.SessionApiKey);
        var agoraId = FirstNonEmpty(row?.AgoraAppId, envAgoraId);
        var agoraCert = FirstNonEmpty(row?.AgoraAppCertificate, envAgoraCert);

        return new IntegrationSettingsDto
        {
            WasenderApiTokenConfigured = !string.IsNullOrWhiteSpace(apiToken),
            WasenderApiTokenHint = Mask(apiToken),
            WasenderSessionApiKeyConfigured = !string.IsNullOrWhiteSpace(sessionKey),
            WasenderSessionApiKeyHint = Mask(sessionKey),
            AgoraAppIdConfigured = !string.IsNullOrWhiteSpace(agoraId),
            AgoraAppIdHint = Mask(agoraId),
            AgoraAppCertificateConfigured = !string.IsNullOrWhiteSpace(agoraCert),
            AgoraAppCertificateHint = Mask(agoraCert),
        };
    }

    public async Task<IntegrationSettingsDto> SaveAsync(
        UpdateIntegrationSettingsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var row = await db.IntegrationSettings.FirstOrDefaultAsync(cancellationToken);
        if (row is null)
        {
            row = new IntegrationSetting();
            db.IntegrationSettings.Add(row);
        }

        ApplyField(request.WasenderApiToken, v => row.WasenderApiToken = v);
        ApplyField(request.WasenderSessionApiKey, v => row.WasenderSessionApiKey = v);
        ApplyField(request.AgoraAppId, v => row.AgoraAppId = v);
        ApplyField(request.AgoraAppCertificate, v => row.AgoraAppCertificate = v);
        row.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        cache.Replace(
            row.WasenderApiToken,
            row.WasenderSessionApiKey,
            row.AgoraAppId,
            row.AgoraAppCertificate);

        return await GetAsync(cancellationToken);
    }

    private static void ApplyField(string? incoming, Action<string?> assign)
    {
        if (incoming is null)
            return;
        assign(string.IsNullOrWhiteSpace(incoming) ? null : incoming.Trim());
    }

    private static string? FirstNonEmpty(string? preferred, string? fallback) =>
        !string.IsNullOrWhiteSpace(preferred) ? preferred : fallback;

    private static string? Mask(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        var trimmed = value.Trim();
        if (trimmed.Length <= 4)
            return "****";
        return $"••••{trimmed[^4..]}";
    }
}
