using AdminAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public static class WhatsappPreconfiguredMessageFormatter
{
    public const string ParentPortalWelcomeEvent = "ParentPortalWelcome";

    public static async Task<string?> GetFormattedMessageAsync(
        AdminDbContext db,
        string eventName,
        IDictionary<string, string> tokens,
        CancellationToken cancellationToken = default)
    {
        var config = await db.WhatsappPreConfiguredMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Event == eventName, cancellationToken);

        if (config is null || !config.IsEnabled)
            return null;

        var formatted = config.WhatsappMessage;
        foreach (var token in tokens)
        {
            if (string.IsNullOrEmpty(token.Key))
                continue;

            formatted = formatted.Replace("{" + token.Key + "}", token.Value ?? string.Empty);
        }

        return formatted;
    }
}
