using AdminAPI.Models;



namespace AdminAPI.Repositories.Interfaces;



public interface IStudentRepository

{

    Task<RegisterForm?> GetByIdAsync(int id, CancellationToken cancellationToken = default);



    Task<List<(int Id, string Name)>> GetCirclesAsync(

        bool forGirls,

        CancellationToken cancellationToken = default);



    Task<List<(int Id, string Name)>> GetPlanLevelsAsync(

        CancellationToken cancellationToken = default);



    Task<RegisterForm> AddAsync(RegisterForm entity, CancellationToken cancellationToken = default);



    Task SaveChangesAsync(CancellationToken cancellationToken = default);

}

