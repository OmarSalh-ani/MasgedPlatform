using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class ParentsFollowupRepository(AdminDbContext db) : IParentsFollowupRepository
{
    public Task<RegisterForm?> GetStudentWithFollowupAsync(
        int studentId,
        CancellationToken cancellationToken = default) =>
        db.RegisterForms
            .Include(x => x.ParentFollowup)
            .FirstOrDefaultAsync(x => x.Id == studentId, cancellationToken);
}
