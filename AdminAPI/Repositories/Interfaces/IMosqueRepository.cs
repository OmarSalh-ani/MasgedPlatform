using AdminAPI.Models;



namespace AdminAPI.Repositories.Interfaces;



public interface IMosqueRepository

{

    Task<(List<Mosque> Items, int TotalCount)> GetPagedAsync(

        int pageNumber,

        int pageSize,

        CancellationToken cancellationToken = default);



    Task<Mosque?> GetByIdAsync(int id, CancellationToken cancellationToken = default);



    Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken = default);



    Task<Mosque> AddAsync(Mosque entity, CancellationToken cancellationToken = default);



    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);



    Task SaveChangesAsync(CancellationToken cancellationToken = default);

}

