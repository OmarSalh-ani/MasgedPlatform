using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.ContactInfo;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using AutoMapper;

namespace AdminAPI.Services;

public class ContactInfoService(
    IContactInfoRepository repository,
    IMapper mapper) : IContactInfoService
{
    public async Task<PagedResultDto<ContactInfoListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var (items, totalCount) = await repository.GetPagedAsync(page, pageSize, cancellationToken);
        var effectivePageSize = pageSize <= 0 ? (totalCount == 0 ? 1 : totalCount) : pageSize;

        return new PagedResultDto<ContactInfoListItemDto>
        {
            Items = mapper.Map<List<ContactInfoListItemDto>>(items),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = effectivePageSize,
            TotalPages = pageSize <= 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<ContactInfoDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var contact = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("بيانات التواصل غير موجودة");

        return mapper.Map<ContactInfoDto>(contact);
    }

    public Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default) =>
        repository.GetNextSortOrderAsync(cancellationToken);

    public async Task<ContactInfoDto> CreateAsync(
        SaveContactInfoRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var contact = new ContactInfo
        {
            ContactType = request.ContactType.Trim(),
            Label = NormalizeOptional(request.Label),
            Value = request.Value.Trim(),
            SortOrder = request.SortOrder
        };

        await repository.AddAsync(contact, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return mapper.Map<ContactInfoDto>(contact);
    }

    public async Task<ContactInfoDto> UpdateAsync(
        int id,
        SaveContactInfoRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var contact = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("بيانات التواصل غير موجودة");

        contact.ContactType = request.ContactType.Trim();
        contact.Label = NormalizeOptional(request.Label);
        contact.Value = request.Value.Trim();
        contact.SortOrder = request.SortOrder;

        await repository.SaveChangesAsync(cancellationToken);
        return mapper.Map<ContactInfoDto>(contact);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return false;

        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
