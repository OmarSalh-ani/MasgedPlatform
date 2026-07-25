using AdminAPI.Data;
using AdminAPI.Models;
using Masged.WhatsApp.Interfaces;
using Masged.WhatsApp.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AdminAPI.Services;

public class WhatsappQrSessionStore(AdminDbContext db, IConfiguration configuration) : IWhatsappSessionStore
{
    public const string SessionKey = "WhatsappQR_SessionId";
    public const string SessionApiKeySettingKey = "WhatsappQR_SessionApiKey";
    private const string DefaultSessionName = "Masged";

    public string SessionName => DefaultSessionName;

    public async Task<int> GetSessionIdAsync(CancellationToken cancellationToken = default)
    {
        var setting = await db.AppSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Key == SessionKey && !x.ForGirl, cancellationToken);

        if (setting != null && int.TryParse(setting.Value, out var dbId))
            return dbId;

        return int.TryParse(configuration[SessionKey], out var configId) ? configId : 1;
    }

    public async Task SetSessionIdAsync(int sessionId, CancellationToken cancellationToken = default)
    {
        var setting = await db.AppSettings
            .FirstOrDefaultAsync(x => x.Key == SessionKey && !x.ForGirl, cancellationToken);

        if (setting == null)
        {
            setting = new AppSetting
            {
                Key = SessionKey,
                Value = sessionId.ToString(),
                Description = "WasenderAPI session ID for masged",
                CreatedAt = KuwaitTime.Now,
                UpdatedAt = KuwaitTime.Now,
                ForGirl = false,
            };
            db.AppSettings.Add(setting);
        }
        else
        {
            setting.Value = sessionId.ToString();
            setting.UpdatedAt = KuwaitTime.Now;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<string?> GetSessionApiKeyAsync(CancellationToken cancellationToken = default)
    {
        var setting = await db.AppSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Key == SessionApiKeySettingKey && !x.ForGirl, cancellationToken);

        if (!string.IsNullOrWhiteSpace(setting?.Value))
            return setting!.Value.Trim();

        var configKey = configuration[$"{WasenderApiOptions.SectionName}:SessionApiKey"];
        return string.IsNullOrWhiteSpace(configKey) ? null : configKey.Trim();
    }

    public async Task SetSessionApiKeyAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            return;

        var setting = await db.AppSettings
            .FirstOrDefaultAsync(x => x.Key == SessionApiKeySettingKey && !x.ForGirl, cancellationToken);

        if (setting == null)
        {
            setting = new AppSetting
            {
                Key = SessionApiKeySettingKey,
                Value = apiKey.Trim(),
                Description = "WasenderAPI session API key for masged",
                CreatedAt = KuwaitTime.Now,
                UpdatedAt = KuwaitTime.Now,
                ForGirl = false,
            };
            db.AppSettings.Add(setting);
        }
        else
        {
            setting.Value = apiKey.Trim();
            setting.UpdatedAt = KuwaitTime.Now;
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
