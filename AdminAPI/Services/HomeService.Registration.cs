using AdminAPI.DTOs.Home;
using AdminAPI.Models;
using AdminAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public partial class HomeService
{
    public async Task<HomeRegistrationSettingsDto> GetRegistrationSettingsAsync(
        CancellationToken cancellationToken = default)
    {
        var menEnabled = await GetRegistrationEnabledAsync(false, cancellationToken);
        var womenEnabled = await GetRegistrationEnabledAsync(true, cancellationToken);

        return new HomeRegistrationSettingsDto
        {
            MenEnabled = menEnabled,
            WomenEnabled = womenEnabled,
            ShowControls = currentUser.IsAdmin && !currentUser.IsGirlTeacher,
        };
    }

    public async Task UpdateRegistrationSettingsAsync(
        UpdateHomeRegistrationRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAdmin || currentUser.IsGirlTeacher)
            throw new UnauthorizedAccessException("ليس لديك صلاحية لتعديل إعدادات التسجيل");

        EnsureCanModify();
        await SetRegistrationEnabledAsync(request.Enabled, request.ForGirl, cancellationToken);
    }

    private async Task<bool> GetRegistrationEnabledAsync(bool forGirl, CancellationToken cancellationToken)
    {
        var setting = await db.AppSettings
            .FirstOrDefaultAsync(x => x.Key == "RegistrationEnabled" && x.ForGirl == forGirl, cancellationToken);

        return setting != null && bool.TryParse(setting.Value, out var enabled) && enabled;
    }

    private async Task SetRegistrationEnabledAsync(
        bool enabled,
        bool forGirl,
        CancellationToken cancellationToken)
    {
        var setting = await db.AppSettings
            .FirstOrDefaultAsync(x => x.Key == "RegistrationEnabled" && x.ForGirl == forGirl, cancellationToken);

        if (setting == null)
        {
            setting = new AppSetting
            {
                Key = "RegistrationEnabled",
                Value = enabled.ToString().ToLowerInvariant(),
                Description = forGirl
                    ? "Controls whether women registration is enabled or disabled"
                    : "Controls whether men registration is enabled or disabled",
                CreatedAt = KuwaitTime.Now,
                UpdatedAt = KuwaitTime.Now,
                ForGirl = forGirl,
            };
            db.AppSettings.Add(setting);
        }
        else
        {
            setting.Value = enabled.ToString().ToLowerInvariant();
            setting.UpdatedAt = KuwaitTime.Now;
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
