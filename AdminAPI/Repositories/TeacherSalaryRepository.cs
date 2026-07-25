using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class TeacherSalaryRepository(AdminDbContext db) : ITeacherSalaryRepository
{
    public Task<List<TeacherSalary>> GetFilteredListAsync(
        bool forGirls,
        int? month,
        int? year,
        int? teacherId,
        CancellationToken cancellationToken = default)
    {
        var query = db.TeacherSalaries.AsNoTracking()
            .Include(s => s.Teacher)
            .Where(s => s.Teacher != null && s.Teacher.IsGirlTeacher == forGirls);

        if (month is > 0)
            query = query.Where(s => s.Month == month);
        if (year is > 0)
            query = query.Where(s => s.Year == year);
        if (teacherId is > 0)
            query = query.Where(s => s.TeacherId == teacherId);

        return query
            .OrderByDescending(s => s.Year)
            .ThenByDescending(s => s.Month)
            .ThenBy(s => s.Teacher!.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<TeacherSalary?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.TeacherSalaries
            .Include(s => s.Teacher)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public Task<TeacherSalary?> GetByTeacherMonthYearAsync(
        int teacherId,
        int month,
        int year,
        CancellationToken cancellationToken = default) =>
        db.TeacherSalaries.FirstOrDefaultAsync(
            s => s.TeacherId == teacherId && s.Month == month && s.Year == year,
            cancellationToken);

    public Task<List<TeacherSalary>> GetReportAsync(
        bool forGirls,
        int month,
        int year,
        CancellationToken cancellationToken = default) =>
        db.TeacherSalaries.AsNoTracking()
            .Include(s => s.Teacher)
            .Where(s => s.Month == month && s.Year == year && s.Teacher != null && s.Teacher.IsGirlTeacher == forGirls)
            .OrderBy(s => s.Teacher!.Name)
            .ToListAsync(cancellationToken);

    public Task<List<TeacherSalary>> GetByIdsForGirlsAsync(
        IEnumerable<int> ids,
        bool forGirls,
        CancellationToken cancellationToken = default) =>
        db.TeacherSalaries
            .Include(s => s.Teacher)
            .Where(s => ids.Contains(s.Id) && s.Teacher != null && s.Teacher.IsGirlTeacher == forGirls)
            .ToListAsync(cancellationToken);

    public Task<List<TeacherAttendance>> GetMonthAttendancesAsync(
        int teacherId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default) =>
        db.TeacherAttendances.AsNoTracking()
            .Where(a => a.TeacherId == teacherId
                && a.AttendanceDateTime >= startDate
                && a.AttendanceDateTime <= endDate
                && a.DepartureDateTime.HasValue)
            .ToListAsync(cancellationToken);

    public Task<List<Teacher>> GetFilterTeachersAsync(CancellationToken cancellationToken = default) =>
        db.Teachers.AsNoTracking()
            .Where(t => t.IsGirlTeacher == false && t.UsersManage == false)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

    public Task<List<Teacher>> GetFormTeachersAsync(CancellationToken cancellationToken = default) =>
        db.Teachers.AsNoTracking()
            .Where(t => t.IsGirlTeacher == false
                && t.UsersManage == false
                && db.QuranCircles.Any(c => c.TeacherId == t.Id))
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

    public Task<List<Teacher>> GetAutoCalculateTeachersAsync(CancellationToken cancellationToken = default) =>
        db.Teachers.AsNoTracking()
            .Where(t => t.UsersManage == false)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(TeacherSalary entity, CancellationToken cancellationToken = default) =>
        await db.TeacherSalaries.AddAsync(entity, cancellationToken);

    public async Task AddExpensiveAsync(Expensive entity, CancellationToken cancellationToken = default) =>
        await db.Expensives.AddAsync(entity, cancellationToken);

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.TeacherSalaries.FindAsync([id], cancellationToken);
        if (entity is null)
            return false;

        db.TeacherSalaries.Remove(entity);
        return true;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
