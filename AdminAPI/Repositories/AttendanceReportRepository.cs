using AdminAPI.DTOs.AttendanceReport;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class AttendanceReportRepository(Data.AdminDbContext db) : IAttendanceReportRepository
{
    public async Task<AttendanceReportFilterOptionsDto> GetFilterOptionsAsync(
        bool isGirlTeacher,
        CancellationToken cancellationToken = default)
    {
        var circles = await db.QuranCircles
            .AsNoTracking()
            .Where(x => x.Teacher != null && x.Teacher.IsGirlTeacher == isGirlTeacher)
            .OrderBy(c => c.Name)
            .Select(c => new AttendanceReportLookupDto
            {
                Id = c.Id,
                Name = c.Name,
            })
            .ToListAsync(cancellationToken);

        var teachers = await db.Teachers
            .AsNoTracking()
            .Where(x => x.IsGirlTeacher == isGirlTeacher)
            .OrderBy(t => t.Name)
            .Select(t => new AttendanceReportLookupDto
            {
                Id = t.Id,
                Name = t.Name,
            })
            .ToListAsync(cancellationToken);

        return new AttendanceReportFilterOptionsDto
        {
            Circles = circles,
            Teachers = teachers,
        };
    }

    public IQueryable<RegisterForm> BuildStudentQuery(
        bool isGirlTeacher,
        int? circleId,
        int? teacherId)
    {
        var gender = isGirlTeacher ? "أنثى" : "ذكر";
        var query = db.RegisterForms
            .AsNoTracking()
            .Where(s => s.QuranCircleId != null && s.StudentGender == gender);

        if (circleId.HasValue)
            query = query.Where(s => s.QuranCircleId == circleId.Value);

        if (teacherId.HasValue)
            query = query.Where(s => s.QuranCircle != null && s.QuranCircle.TeacherId == teacherId.Value);

        return query;
    }
}
