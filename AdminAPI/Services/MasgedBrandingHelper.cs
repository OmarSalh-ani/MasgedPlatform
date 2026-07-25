using AdminAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public static class MasgedBrandingHelper
{
    public const string FallbackMasgedName = "مسجد الشيخ مبارك عبدالله المبارك الصباح";

    public static async Task<string> GetMasgedNameAsync(
        AdminDbContext db,
        CancellationToken cancellationToken = default)
    {
        var masgedName = await db.MasgedSettings
            .AsNoTracking()
            .Select(x => x.MasgedName)
            .FirstOrDefaultAsync(cancellationToken);

        return string.IsNullOrWhiteSpace(masgedName) ? FallbackMasgedName : masgedName.Trim();
    }
}
