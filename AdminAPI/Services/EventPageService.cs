using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.EventPages;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class EventPageService(
    IEventPageRepository repository,
    IOptions<EventPageUploadOptions> uploadOptions) : IEventPageService
{
    public async Task<PagedResultDto<EventPageListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var (items, totalCount) = await repository.GetPagedAsync(page, pageSize, cancellationToken);
        var effectivePageSize = pageSize <= 0 ? (totalCount == 0 ? 1 : totalCount) : pageSize;

        return new PagedResultDto<EventPageListItemDto>
        {
            Items = items.Select(EventPageMapper.ToListItem).ToList(),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = effectivePageSize,
            TotalPages = pageSize <= 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize),
        };
    }

    public async Task<EventPageDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var page = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("الصفحة غير موجودة");

        return EventPageMapper.ToDto(page);
    }

    public async Task<List<EventPageLookupDto>> GetLookupsAsync(CancellationToken cancellationToken = default)
    {
        var pages = await repository.GetLookupsAsync(cancellationToken);
        return pages.Select(p => new EventPageLookupDto
        {
            Id = p.Id,
            ActivityName = p.ActivityName,
        }).ToList();
    }

    public async Task<EventPageDto> CreateAsync(
        SaveEventPageRequestDto request,
        CancellationToken cancellationToken = default)
    {
        await EnsureUniqueAsync(request, null, cancellationToken);

        var page = new EventPage { CreatedAt = DateTime.Now };
        await ApplySaveAsync(page, request, cancellationToken);
        await repository.AddAsync(page, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return EventPageMapper.ToDto(page);
    }

    public async Task<EventPageDto> UpdateAsync(
        int id,
        SaveEventPageRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var page = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("الصفحة غير موجودة");

        await EnsureUniqueAsync(request, id, cancellationToken);
        await ApplySaveAsync(page, request, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return EventPageMapper.ToDto(page);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return false;

        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task EnsureUniqueAsync(
        SaveEventPageRequestDto request,
        int? excludeId,
        CancellationToken cancellationToken)
    {
        var activityName = EventPageText.Required(request.ActivityName);
        var slug = EventPageText.NormalizeSlug(request.Slug);

        if (await repository.ActivityNameExistsAsync(activityName, excludeId, cancellationToken))
            throw new ArgumentException("اسم النشاط مستخدم مسبقاً");

        if (await repository.SlugExistsAsync(slug, excludeId, cancellationToken))
            throw new ArgumentException("رابط الصفحة مستخدم مسبقاً");
    }

    private async Task ApplySaveAsync(
        EventPage page,
        SaveEventPageRequestDto request,
        CancellationToken cancellationToken)
    {
        EventPageLandingBinder.Apply(page, request);
        EventPageLandingBinder.ReplaceTracks(page, EventPageJsonParser.ParseTracks(request.TracksJson));
        EventPageLandingBinder.ReplaceFields(page, EventPageJsonParser.ParseFields(request.FieldsJson));

        var imageUrl = await EventPageImageStorage.SaveAsync(
            request.Image,
            uploadOptions.Value.Directory,
            cancellationToken);

        if (imageUrl is not null)
            page.ImageUrl = imageUrl;
    }
}
