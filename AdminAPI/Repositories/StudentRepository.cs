using AdminAPI.Data;

using AdminAPI.Models;

using AdminAPI.Repositories.Interfaces;

using Microsoft.EntityFrameworkCore;



namespace AdminAPI.Repositories;



public class StudentRepository(AdminDbContext db) : IStudentRepository

{

    public Task<RegisterForm?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>

        db.RegisterForms.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);



    public Task<List<(int Id, string Name)>> GetCirclesAsync(

        bool forGirls,

        CancellationToken cancellationToken = default) =>

        db.QuranCircles

            .AsNoTracking()

            .Where(x => x.ForGirls == forGirls)

            .OrderBy(x => x.Name)

            .Select(x => new ValueTuple<int, string>(x.Id, x.Name))

            .ToListAsync(cancellationToken);



    public Task<List<(int Id, string Name)>> GetPlanLevelsAsync(

        CancellationToken cancellationToken = default) =>

        db.PlanLevels

            .AsNoTracking()

            .OrderBy(x => x.LevelName)

            .Select(x => new ValueTuple<int, string>(x.Id, x.LevelName))

            .ToListAsync(cancellationToken);



    public async Task<RegisterForm> AddAsync(RegisterForm entity, CancellationToken cancellationToken = default)

    {

        await db.RegisterForms.AddAsync(entity, cancellationToken);

        return entity;

    }



    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>

        db.SaveChangesAsync(cancellationToken);

}

