using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class AboutRepository(AdminDbContext db) : IAboutRepository
{
    public Task<AboutAssociation?> GetFirstAsync(CancellationToken cancellationToken = default) =>
        db.AboutAssociations.AsNoTracking().FirstOrDefaultAsync(cancellationToken);

    public async Task<AboutAssociation> AddAsync(AboutAssociation entity, CancellationToken cancellationToken = default)
    {
        await db.AboutAssociations.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
