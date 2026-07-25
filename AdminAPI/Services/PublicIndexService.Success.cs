using AdminAPI.DTOs.PublicIndex;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public partial class PublicIndexService
{
    private async Task<PublicRegisterSuccessDto> BuildRegisterSuccessAsync(CancellationToken cancellationToken)
    {
        var socialLinks = await GetSocialLinksAsync(cancellationToken);
        var masgedName = await GetMasgedNameAsync(cancellationToken);
        var whatsappUrl = socialLinks
            .FirstOrDefault(x =>
                x.PlatformName.Contains("whatsapp", StringComparison.OrdinalIgnoreCase)
                || x.PlatformName.Contains("واتساب", StringComparison.Ordinal))
            ?.Url;

        return new PublicRegisterSuccessDto
        {
            TitleText = BuildRegisterSuccessTitleText(masgedName),
            SubscribeText = BuildRegisterSuccessSubscribeText(masgedName),
            WhatsappUrl = string.IsNullOrEmpty(whatsappUrl)
                ? registrationOptions.Value.FallbackWhatsappUrl
                : whatsappUrl,
            SocialLinks = socialLinks,
        };
    }
}
