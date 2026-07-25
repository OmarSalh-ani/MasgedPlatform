using AdminAPI.DTOs.PublicIndex;
using Microsoft.EntityFrameworkCore;
namespace AdminAPI.Services;

public partial class PublicIndexService
{
    private async Task<PublicWebsiteContentDto> BuildWebsiteContentAsync(CancellationToken cancellationToken)
    {
        var about = await db.AboutAssociations.AsNoTracking().FirstOrDefaultAsync(cancellationToken);

        return new PublicWebsiteContentDto
        {
            HeroSlides = await db.HeroSlides
                .AsNoTracking()
                .Where(x => x.ImageUrl != null && x.ImageUrl != string.Empty)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Id)
                .Select(x => new PublicHeroSlideItemDto { Id = x.Id, ImageUrl = x.ImageUrl! })
                .ToListAsync(cancellationToken),
            Competitions = await db.Competitions
                .AsNoTracking()
                .OrderBy(x => x.SortOrder)
                .ThenByDescending(x => x.Id)
                .Select(x => new PublicCompetitionItemDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    ImageUrl = x.ImageUrl,
                    LinkUrl = x.LinkUrl,
                })
                .ToListAsync(cancellationToken),
            Mosques = await db.Mosques
                .AsNoTracking()
                .OrderBy(x => x.SortOrder)
                .ThenByDescending(x => x.Id)
                .Select(x => new PublicMosqueItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    GoogleMapsUrl = x.GoogleMapsUrl,
                    ImageUrl = x.ImageUrl,
                })
                .ToListAsync(cancellationToken),
            News = await db.NewsItems
                .AsNoTracking()
                .OrderBy(x => x.SortOrder)
                .ThenByDescending(x => x.NewsDate)
                .Select(x => new PublicNewsItemDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    NewsDate = x.NewsDate,
                    ImageUrl = x.ImageUrl,
                    LinkUrl = x.LinkUrl,
                })
                .ToListAsync(cancellationToken),
            Activities = await db.Activities
                .AsNoTracking()
                .OrderBy(x => x.SortOrder)
                .ThenByDescending(x => x.Id)
                .Select(x => new PublicActivityItemDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    ImageUrl = x.ImageUrl,
                    IconClass = x.IconClass,
                })
                .ToListAsync(cancellationToken),
            About = new PublicAboutDto
            {
                Content = string.IsNullOrEmpty(about?.Content) ? DefaultAboutContent : about.Content,
                Address = string.IsNullOrEmpty(about?.Address) ? null : about.Address,
                MapsUrl = string.IsNullOrEmpty(about?.MapsUrl) ? null : about.MapsUrl,
            },
            SocialLinks = await GetSocialLinksAsync(cancellationToken),
        };
    }
}
