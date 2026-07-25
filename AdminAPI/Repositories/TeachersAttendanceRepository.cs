using AdminAPI.Data;
using AdminAPI.DTOs.TeachersAttendance;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class TeachersAttendanceRepository(AdminDbContext db) : ITeachersAttendanceRepository
{
    public Task<List<TeachersAttendanceTeacherOptionDto>> GetTeachersAsync(
        bool isGirlTeacher,
        CancellationToken cancellationToken = default) =>
        db.Teachers
            .AsNoTracking()
            .Where(x => x.IsGirlTeacher == isGirlTeacher)
            .OrderBy(x => x.Name)
            .Select(x => new TeachersAttendanceTeacherOptionDto
            {
                Id = x.Id,
                Name = x.Name,
            })
            .ToListAsync(cancellationToken);

    public Task<List<TeachersAttendanceSourceRow>> GetAttendanceRowsAsync(
        bool isGirlTeacher,
        DateTime fromDate,
        DateTime toDateInclusive,
        int? teacherId,
        CancellationToken cancellationToken = default)
    {
        var query = db.TeacherAttendances
            .AsNoTracking()
            .Include(a => a.Teacher)
            .Where(a => a.Teacher != null && a.Teacher.IsGirlTeacher == isGirlTeacher)
            .Where(a => a.AttendanceDateTime >= fromDate && a.AttendanceDateTime <= toDateInclusive);

        if (teacherId is > 0)
            query = query.Where(a => a.TeacherId == teacherId.Value);

        return query
            .OrderByDescending(a => a.AttendanceDateTime)
            .ThenBy(a => a.Teacher!.Name)
            .Select(a => new TeachersAttendanceSourceRow
            {
                TeacherName = a.Teacher!.Name,
                AttendanceDateTime = a.AttendanceDateTime,
                DepartureDateTime = a.DepartureDateTime,
            })
            .ToListAsync(cancellationToken);
    }
}
