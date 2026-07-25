using AdminAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class RegistrationSettingsService(
    AdminDbContext db,
    IOptions<PublicRegistrationOptions> registrationOptions)
{
    public async Task<bool> GetRegistrationEnabledAsync(
        bool forGirl,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var setting = await db.AppSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Key == "RegistrationEnabled" && x.ForGirl == forGirl,
                    cancellationToken);

            if (setting != null && bool.TryParse(setting.Value, out var enabled))
                return enabled;

            return registrationOptions.Value.Enabled;
        }
        catch
        {
            return false;
        }
    }
}
