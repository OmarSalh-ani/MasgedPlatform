using AdminAPI.Data;
using AdminAPI.DTOs.MasgedSettings;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class MasgedSettingsService(
    IMasgedSettingsRepository repository,
    AdminDbContext db,
    IHttpContextAccessor httpContextAccessor,
    IOptions<TeacherUploadOptions> uploadOptions) : IMasgedSettingsService
{
    public async Task<MasgedSettingsDto?> GetAsync(CancellationToken cancellationToken = default)
    {
        var setting = await repository.GetFirstAsync(cancellationToken);
        return setting is null ? null : MapToDto(setting);
    }

    public async Task<MasgedSettingsDto> SaveAsync(
        UpdateMasgedSettingsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var masgedName = request.MasgedName.Trim();
        var setting = await db.MasgedSettings.FirstOrDefaultAsync(cancellationToken);
        var uploadDirectory = uploadOptions.Value.Directory;

        if (setting is null)
        {
            setting = new MasgedSetting
            {
                MasgedName = masgedName,
                UpdatedAt = DateTime.Now,
            };
            ApplyOptionalBranding(setting, request);
            ApplyAppStoreUrls(setting, request);
            await ApplyLogoChangesAsync(setting, request, uploadDirectory, cancellationToken);
            await repository.AddAsync(setting, cancellationToken);
        }
        else
        {
            setting.MasgedName = masgedName;
            setting.UpdatedAt = DateTime.Now;
            ApplyOptionalBranding(setting, request);
            ApplyAppStoreUrls(setting, request);
            await ApplyLogoChangesAsync(setting, request, uploadDirectory, cancellationToken);
        }

        await repository.SaveChangesAsync(cancellationToken);
        return MapToDto(setting);
    }

    private static void ApplyOptionalBranding(MasgedSetting setting, UpdateMasgedSettingsRequestDto request)
    {
        if (!string.IsNullOrWhiteSpace(request.PrimaryColor))
            setting.PrimaryColor = request.PrimaryColor.Trim();
    }

    private async Task ApplyLogoChangesAsync(
        MasgedSetting setting,
        UpdateMasgedSettingsRequestDto request,
        string uploadDirectory,
        CancellationToken cancellationToken)
    {
        if (request.RemoveLogo)
        {
            TeacherImageStorage.DeleteIfExists(setting.LogoFileName, uploadDirectory);
            setting.LogoFileName = null;
            return;
        }

        if (request.LogoFile is null || request.LogoFile.Length == 0)
            return;

        var savedFileName = await TeacherImageStorage.SaveAsync(
            request.LogoFile,
            uploadDirectory,
            cancellationToken);

        if (string.IsNullOrWhiteSpace(savedFileName))
            return;

        TeacherImageStorage.DeleteIfExists(setting.LogoFileName, uploadDirectory);
        setting.LogoFileName = savedFileName;
    }

    private static void ApplyAppStoreUrls(MasgedSetting setting, UpdateMasgedSettingsRequestDto request)
    {
        setting.ParentAppStoreUrl = NormalizeOptionalUrl(request.ParentAppStoreUrl);
        setting.ParentGooglePlayUrl = NormalizeOptionalUrl(request.ParentGooglePlayUrl);
        setting.TeacherAppStoreUrl = NormalizeOptionalUrl(request.TeacherAppStoreUrl);
        setting.TeacherGooglePlayUrl = NormalizeOptionalUrl(request.TeacherGooglePlayUrl);
    }

    private static string? NormalizeOptionalUrl(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private MasgedSettingsDto MapToDto(MasgedSetting setting) =>
        new()
        {
            Id = setting.Id,
            MasgedName = setting.MasgedName,
            LogoUrl = BuildLogoUrl(setting.LogoFileName),
            ParentAppStoreUrl = setting.ParentAppStoreUrl,
            ParentGooglePlayUrl = setting.ParentGooglePlayUrl,
            TeacherAppStoreUrl = setting.TeacherAppStoreUrl,
            TeacherGooglePlayUrl = setting.TeacherGooglePlayUrl,
            PrimaryColor = setting.PrimaryColor,
        };

    private string? BuildLogoUrl(string? logoFileName)
    {
        if (string.IsNullOrWhiteSpace(logoFileName))
            return null;

        var baseUrl = GetRequestBaseUrl();
        return TeacherImageStorage.BuildPublicImageUrl(logoFileName, baseUrl);
    }

    private string GetRequestBaseUrl()
    {
        var request = httpContextAccessor.HttpContext?.Request;
        if (request is null)
            return "http://localhost:5287";

        return $"{request.Scheme}://{request.Host.Value}";
    }
}
