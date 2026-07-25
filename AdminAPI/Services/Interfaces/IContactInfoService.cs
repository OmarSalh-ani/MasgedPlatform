using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.ContactInfo;

namespace AdminAPI.Services.Interfaces;

public interface IContactInfoService
{
    Task<PagedResultDto<ContactInfoListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ContactInfoDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);

    Task<ContactInfoDto> CreateAsync(
        SaveContactInfoRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ContactInfoDto> UpdateAsync(
        int id,
        SaveContactInfoRequestDto request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
