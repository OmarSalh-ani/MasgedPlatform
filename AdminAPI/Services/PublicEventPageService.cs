using AdminAPI.DTOs.PublicEventPages;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;

namespace AdminAPI.Services;

public class PublicEventPageService(
    IEventPageRepository pageRepository,
    IEventPageResponseRepository responseRepository) : IPublicEventPageService
{
    public async Task<PublicEventPageDto> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        var page = await pageRepository.GetBySlugAsync(EventPageText.NormalizeSlug(slug), cancellationToken);
        if (page is null || !page.IsPublished)
            throw new KeyNotFoundException("الصفحة غير موجودة");

        return EventPageMapper.ToPublicDto(page);
    }

    public async Task SubmitRegistrationAsync(
        string slug,
        SubmitEventPageRegistrationRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var page = await pageRepository.GetBySlugAsync(EventPageText.NormalizeSlug(slug), cancellationToken);
        if (page is null || !page.IsPublished)
            throw new KeyNotFoundException("الصفحة غير موجودة");

        if (!page.IsRegistrationOpen)
            throw new ArgumentException("التسجيل مغلق لهذه الدورة");

        var values = EventPageRegistrationValidator.BuildValues(page, request.Answers);
        var response = new EventPageResponse
        {
            EventPageId = page.Id,
            ActivityName = page.ActivityName,
            SubmittedAt = DateTime.Now,
            Values = values,
        };

        await responseRepository.AddAsync(response, cancellationToken);
        await responseRepository.SaveChangesAsync(cancellationToken);
    }
}
