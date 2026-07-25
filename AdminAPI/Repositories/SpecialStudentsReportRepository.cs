using AdminAPI.Data;
using AdminAPI.DTOs.SpecialStudentsReport;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class SpecialStudentsReportRepository(AdminDbContext db) : ISpecialStudentsReportRepository
{
    public async Task<List<SpecialStudentsReportRowDto>> GetSpecialStudentsAsync(
        CancellationToken cancellationToken = default)
    {
        return await db.RegisterForms
            .AsNoTracking()
            .Where(x => x.IsSpecial && x.QuranCircleId.HasValue)
            .OrderBy(x => x.QuranCircle!.Name)
            .ThenBy(x => x.StudentName)
            .Select(x => new SpecialStudentsReportRowDto
            {
                StudentName = x.StudentName,
                CircleName = x.QuranCircle != null ? x.QuranCircle.Name ?? "غير محدد" : "غير محدد",
                FatherPhone = x.FatherPhone,
                FatherPhone2 = x.FatherPhone2,
                StudentPhone = x.StudentPhone,
                StudentGender = x.StudentGender,
                Age = x.Age,
                PhotoPath = x.ParentFollowup != null ? x.ParentFollowup.PhotoPath : null,
                CircleId = x.QuranCircleId,
            })
            .ToListAsync(cancellationToken);
    }
}
