using AdminAPI.Data;
using AdminAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public static class WhatsappMessageFormatter
{
    public static string FormatMessage(string template, IReadOnlyDictionary<string, string> tokens)
    {
        if (string.IsNullOrEmpty(template))
            return string.Empty;

        var formatted = template;
        foreach (var token in tokens)
        {
            if (string.IsNullOrEmpty(token.Key))
                continue;
            formatted = formatted.Replace("{" + token.Key + "}", token.Value ?? string.Empty);
        }

        return formatted;
    }

    public static async Task<string?> GetFormattedMessageAsync(
        AdminDbContext db,
        WhatsappMessageEvent eventType,
        IReadOnlyDictionary<string, string> tokens,
        CancellationToken cancellationToken = default)
    {
        var config = await db.WhatsappPreConfiguredMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Event == eventType.ToString(), cancellationToken);

        if (config is null || !config.IsEnabled)
            return null;

        return FormatMessage(config.WhatsappMessage, tokens);
    }
}
