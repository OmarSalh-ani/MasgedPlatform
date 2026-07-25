using AdminAPI.Data;
using AdminAPI.DTOs.PublicIndex;
using AdminAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public partial class PublicIndexService(
    AdminDbContext db,
    RegistrationSettingsService registrationSettings,
    IOptions<PublicRegistrationOptions> registrationOptions,
    IOptions<PublicSiteOptions> publicSiteOptions) : IPublicIndexService
{
    private const string DefaultAboutContent =
        "مسجد الشيخ مبارك عبدالله المبارك الصباح تسعى لنشر العلم الشرعي والقيم الإسلامية في المجتمع من خلال برامج تعليمية متميزة وأنشطة اجتماعية مؤثرة.";

    public Task<PublicWebsiteContentDto> GetWebsiteContentAsync(CancellationToken cancellationToken = default) =>
        BuildWebsiteContentAsync(cancellationToken);

    public Task<PublicRegistrationConfigDto> GetRegistrationConfigAsync(
        string? mode,
        CancellationToken cancellationToken = default) =>
        BuildRegistrationConfigAsync(NormalizeMode(mode), cancellationToken);

    public Task<SubmitPublicRegistrationResponseDto> SubmitRegistrationAsync(
        SubmitPublicRegistrationRequestDto request,
        CancellationToken cancellationToken = default) =>
        SaveRegistrationAsync(NormalizeMode(request.Mode), request, cancellationToken);

    public Task<PublicRegisterSuccessDto> GetRegisterSuccessAsync(CancellationToken cancellationToken = default) =>
        BuildRegisterSuccessAsync(cancellationToken);

    private static string NormalizeMode(string? mode)
    {
        var normalized = mode?.Trim().ToLowerInvariant();
        return normalized switch
        {
            "mregister" => "mregister",
            "wregister" => "wregister",
            _ => "default",
        };
    }

    private async Task<List<PublicSocialLinkItemDto>> GetSocialLinksAsync(CancellationToken cancellationToken)
    {
        var items = await db.SocialLinks
            .AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return items.Select(x => new PublicSocialLinkItemDto
        {
            Id = x.Id,
            PlatformName = x.PlatformName,
            Url = x.Url,
            IconClass = x.IconClass,
            ResolvedIconClass = SocialLinkIconResolver.Resolve(x.IconClass, x.PlatformName),
        }).ToList();
    }

    private async Task<List<PublicWomanActivityOptionDto>> GetWomanActivitiesAsync(
        bool forGirl,
        CancellationToken cancellationToken)
    {
        return await db.WomanActivities
            .AsNoTracking()
            .Where(x => x.IsVisible && x.ForGirl == forGirl)
            .OrderBy(x => x.Name)
            .Select(x => new PublicWomanActivityOptionDto { Id = x.Id, Name = x.Name })
            .ToListAsync(cancellationToken);
    }

    private Task<string> GetMasgedNameAsync(CancellationToken cancellationToken) =>
        MasgedBrandingHelper.GetMasgedNameAsync(db, cancellationToken);

    private static string BuildRegisterSuccessTitleText(string masgedName) =>
        $"للاشتراك في قنوات التواصل الاجتماعي ل{masgedName}";

    private static string BuildRegisterSuccessSubscribeText(string masgedName) =>
        $"للاشتراك في خدمة رسائل {masgedName}";
}
