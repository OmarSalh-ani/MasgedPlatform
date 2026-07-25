using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface ITeacherSalaryRepository
{
    Task<List<TeacherSalary>> GetFilteredListAsync(
        bool forGirls,
        int? month,
        int? year,
        int? teacherId,
        CancellationToken cancellationToken = default);

    Task<TeacherSalary?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<TeacherSalary?> GetByTeacherMonthYearAsync(
        int teacherId,
        int month,
        int year,
        CancellationToken cancellationToken = default);

    Task<List<TeacherSalary>> GetReportAsync(
        bool forGirls,
        int month,
        int year,
        CancellationToken cancellationToken = default);

    Task<List<TeacherSalary>> GetByIdsForGirlsAsync(
        IEnumerable<int> ids,
        bool forGirls,
        CancellationToken cancellationToken = default);

    Task<List<TeacherAttendance>> GetMonthAttendancesAsync(
        int teacherId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<List<Teacher>> GetFilterTeachersAsync(CancellationToken cancellationToken = default);

    Task<List<Teacher>> GetFormTeachersAsync(CancellationToken cancellationToken = default);

    Task<List<Teacher>> GetAutoCalculateTeachersAsync(CancellationToken cancellationToken = default);

    Task AddAsync(TeacherSalary entity, CancellationToken cancellationToken = default);

    Task AddExpensiveAsync(Expensive entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
