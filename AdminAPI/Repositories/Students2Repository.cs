using AdminAPI.Data;
using AdminAPI.DTOs.Students2;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class Students2Repository(AdminDbContext db) : IStudents2Repository
{
    public async Task<List<Students2RowDto>> GetStudentsAsync(
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        var query = db.RegisterForms.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(x =>
                x.StudentName.Contains(searchTerm) ||
                x.FatherName.Contains(searchTerm) ||
                x.FatherPhone.Contains(searchTerm));
        }

        return await query
            .OrderBy(x => x.StudentName)
            .Select(x => new Students2RowDto
            {
                Id = x.Id,
                StudentName = x.StudentName,
                FatherName = x.FatherName,
                Age = x.Age,
                StudentGender = x.StudentGender,
                FatherPhone = x.FatherPhone,
                CircleName = x.QuranCircle != null ? x.QuranCircle.Name : null,
                MrkzStudent = false,
                CreatedAt = x.CreatedAt,
                PhotoPath = x.ParentFollowup != null ? x.ParentFollowup.PhotoPath : null,
            })
            .ToListAsync(cancellationToken);
    }
}
