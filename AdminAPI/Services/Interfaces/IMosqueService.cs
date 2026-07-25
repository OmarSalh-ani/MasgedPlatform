using AdminAPI.DTOs.Common;

using AdminAPI.DTOs.Mosques;



namespace AdminAPI.Services.Interfaces;



public interface IMosqueService

{

    Task<PagedResultDto<MosqueListItemDto>> GetListAsync(

        int pageNumber,

        int pageSize,

        CancellationToken cancellationToken = default);



    Task<MosqueDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);



    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);



    Task<MosqueDto> CreateAsync(

        SaveMosqueRequestDto request,

        CancellationToken cancellationToken = default);



    Task<MosqueDto> UpdateAsync(

        int id,

        SaveMosqueRequestDto request,

        CancellationToken cancellationToken = default);



    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

}

