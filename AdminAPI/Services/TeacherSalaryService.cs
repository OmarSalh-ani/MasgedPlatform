using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.TeacherSalaries;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using AutoMapper;

namespace AdminAPI.Services;

public class TeacherSalaryService(
    ITeacherSalaryRepository repository,
    ICurrentUserContext currentUser,
    IMapper mapper) : ITeacherSalaryService
{
    public async Task<PagedResultDto<TeacherSalaryListItemDto>> GetListAsync(
        int? month,
        int? year,
        int? teacherId,
        CancellationToken cancellationToken = default)
    {
        EnsureAdmin();
        var items = await repository.GetFilteredListAsync(
            currentUser.IsGirlTeacher,
            month is 0 ? null : month,
            year is 0 ? null : year,
            teacherId is 0 ? null : teacherId,
            cancellationToken);

        var mapped = mapper.Map<List<TeacherSalaryListItemDto>>(items);
        return new PagedResultDto<TeacherSalaryListItemDto>
        {
            Items = mapped,
            TotalCount = mapped.Count,
            PageNumber = 1,
            PageSize = mapped.Count == 0 ? 1 : mapped.Count,
            TotalPages = 1,
        };
    }

    public async Task<TeacherSalaryFilterOptionsDto> GetFilterOptionsAsync(
        CancellationToken cancellationToken = default)
    {
        EnsureAdmin();
        var now = KuwaitTime.Now;
        var currentYear = now.Year;
        var teachers = await repository.GetFilterTeachersAsync(cancellationToken);

        return new TeacherSalaryFilterOptionsDto
        {
            Months =
            [
                new TeacherSalaryOptionDto { Label = "جميع الأشهر", Value = 0 },
                ..Enumerable.Range(1, 12).Select(i => new TeacherSalaryOptionDto
                {
                    Label = i.ToString(),
                    Value = i,
                }),
            ],
            Years =
            [
                new TeacherSalaryOptionDto { Label = "جميع السنوات", Value = 0 },
                ..Enumerable.Range(currentYear - 1, 2).Select(y => new TeacherSalaryOptionDto
                {
                    Label = y.ToString(),
                    Value = y,
                }),
            ],
            Teachers =
            [
                new TeacherSalaryOptionDto { Label = "جميع المعلمين", Value = 0 },
                ..teachers.Select(t => new TeacherSalaryOptionDto { Label = t.Name, Value = t.Id }),
            ],
            DefaultMonth = now.Month,
            DefaultYear = currentYear,
        };
    }

    public async Task<List<TeacherSalaryFormTeacherDto>> GetFormTeachersAsync(
        CancellationToken cancellationToken = default)
    {
        EnsureAdmin();
        var teachers = await repository.GetFormTeachersAsync(cancellationToken);
        return teachers.Select(t => new TeacherSalaryFormTeacherDto
        {
            Id = t.Id,
            Name = t.Name,
            BaseSalary = t.BaseSalary,
        }).ToList();
    }

    public async Task<TeacherSalaryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        EnsureAdmin();
        var entity = await GetScopedSalaryAsync(id, cancellationToken);
        return mapper.Map<TeacherSalaryDto>(entity);
    }

    public async Task<TeacherSalaryDto> CreateAsync(
        SaveTeacherSalaryRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureAdmin();
        EnsureCanModify();

        var existing = await repository.GetByTeacherMonthYearAsync(
            request.TeacherId, request.Month, request.Year, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException("يوجد راتب مسجل بالفعل لهذا المعلم في هذا الشهر");

        var entity = BuildEntity(request);
        entity.CreatedAt = KuwaitTime.Now;
        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return mapper.Map<TeacherSalaryDto>(await repository.GetByIdAsync(entity.Id, cancellationToken)
            ?? entity);
    }

    public async Task<TeacherSalaryDto> UpdateAsync(
        int id,
        SaveTeacherSalaryRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureAdmin();
        EnsureCanModify();
        var entity = await GetScopedSalaryAsync(id, cancellationToken);
        ApplySave(entity, request);
        await repository.SaveChangesAsync(cancellationToken);
        return mapper.Map<TeacherSalaryDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        EnsureAdmin();
        EnsureCanModify();
        var deleted = await repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return false;

        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<AttendanceCalculationResultDto> CalculateAttendanceAsync(
        CalculateTeacherAttendanceRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureAdmin();
        var attendances = await GetMonthAttendancesAsync(
            request.TeacherId, request.Month, request.Year, cancellationToken);
        var result = TeacherSalaryCalculationHelper.CalculateMonthlyAttendance(attendances);
        return TeacherSalaryCalculationHelper.ToDto(result);
    }

    public async Task<SalaryCalculationResultDto> CalculateSalaryAsync(
        CalculateTeacherSalaryRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureAdmin();
        var attendances = await GetMonthAttendancesAsync(
            request.TeacherId, request.Month, request.Year, cancellationToken);
        var attendance = TeacherSalaryCalculationHelper.CalculateMonthlyAttendance(attendances);
        var result = TeacherSalaryCalculationHelper.CalculateSalaryAmount(
            attendance, request.DayOffDate, request.BaseSalary);
        return TeacherSalaryCalculationHelper.ToDto(result);
    }

    public async Task<AutoCalculateMonthResultDto> AutoCalculateAllForMonthAsync(
        AutoCalculateMonthRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureAdmin();
        EnsureCanModify();

        var teachers = await repository.GetAutoCalculateTeachersAsync(cancellationToken);
        var successCount = 0;
        var errors = new List<string>();

        foreach (var teacher in teachers)
        {
            try
            {
                var baseSalary = teacher.BaseSalary ?? 120m;
                var attendances = await GetMonthAttendancesAsync(
                    teacher.Id, request.Month, request.Year, cancellationToken);
                var attendance = TeacherSalaryCalculationHelper.CalculateMonthlyAttendance(attendances);
                var calculated = TeacherSalaryCalculationHelper.CalculateSalaryAmount(
                    attendance, null, baseSalary);

                var existing = await repository.GetByTeacherMonthYearAsync(
                    teacher.Id, request.Month, request.Year, cancellationToken);

                if (existing is not null)
                {
                    existing.DaysAttended = calculated.DaysAttended;
                    existing.TotalHours = calculated.TotalHours;
                    existing.BaseSalary = baseSalary;
                    existing.CalculatedSalary = calculated.CalculatedSalary;
                }
                else
                {
                    await repository.AddAsync(new TeacherSalary
                    {
                        TeacherId = teacher.Id,
                        Month = request.Month,
                        Year = request.Year,
                        BaseSalary = baseSalary,
                        DaysAttended = calculated.DaysAttended,
                        TotalHours = calculated.TotalHours,
                        CalculatedSalary = calculated.CalculatedSalary,
                        Status = "paid",
                        CreatedAt = KuwaitTime.Now,
                    }, cancellationToken);
                }

                successCount++;
            }
            catch (Exception ex)
            {
                errors.Add($"{teacher.Name}: {ex.Message}");
            }
        }

        await repository.SaveChangesAsync(cancellationToken);
        return new AutoCalculateMonthResultDto
        {
            SuccessCount = successCount,
            ErrorCount = errors.Count,
            Errors = errors,
        };
    }

    public async Task<PaySelectedSalariesResultDto> PaySelectedSalariesAsync(
        PaySelectedSalariesRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureAdmin();
        EnsureCanModify();

        if (request.SalaryIds.Count == 0)
            throw new InvalidOperationException("لم يتم تحديد أي رواتب");

        var salaries = await repository.GetByIdsForGirlsAsync(
            request.SalaryIds, currentUser.IsGirlTeacher, cancellationToken);
        if (salaries.Count == 0)
            throw new InvalidOperationException("لم يتم العثور على الرواتب المحددة");

        var expensesCreated = 0;
        var errors = new List<string>();

        foreach (var group in salaries.GroupBy(s => new { s.Month, s.Year }))
        {
            try
            {
                var groupSalaries = group.ToList();
                var totalAmount = groupSalaries.Sum(s => s.CalculatedSalary);
                var teacherNames = string.Join("، ", groupSalaries
                    .Select(s => s.Teacher?.Name)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Distinct());
                var monthYear = group.Key.Month.ToString("00") + "-" + group.Key.Year;

                await repository.AddExpensiveAsync(new Expensive
                {
                    Reason = "رواتب عن شهر " + monthYear,
                    Notes = teacherNames,
                    TotalAmount = (double)totalAmount,
                    Supplier = "رواتب المعلمين",
                    TeacherId = currentUser.TeacherId,
                    CreatedAt = KuwaitTime.Now,
                    ForGirls = currentUser.IsGirlTeacher,
                    AttachmentsFolder = string.Empty,
                }, cancellationToken);
                expensesCreated++;
            }
            catch (Exception ex)
            {
                errors.Add($"خطأ في إنشاء مصروف لشهر {group.Key.Month}/{group.Key.Year}: {ex.Message}");
            }
        }

        if (expensesCreated > 0)
            await repository.SaveChangesAsync(cancellationToken);

        if (errors.Count > 0 && expensesCreated == 0)
            throw new InvalidOperationException(string.Join("\n", errors));

        var message = $"تم إنشاء {expensesCreated} مصروف بنجاح";
        if (errors.Count > 0)
            message += $"\n{errors.Count} أخطاء: " + string.Join("\n", errors);

        return new PaySelectedSalariesResultDto
        {
            ExpensesCreated = expensesCreated,
            Message = message,
            Errors = errors,
        };
    }

    public async Task<TeacherSalaryReportDto> GetReportAsync(
        int month,
        int year,
        CancellationToken cancellationToken = default)
    {
        EnsureAdmin();
        var salaries = await repository.GetReportAsync(currentUser.IsGirlTeacher, month, year, cancellationToken);
        return BuildReport(salaries);
    }

    public async Task<byte[]> ExportReportExcelAsync(
        int month,
        int year,
        CancellationToken cancellationToken = default)
    {
        var report = await GetReportAsync(month, year, cancellationToken);
        return TeacherSalaryExcelExporter.Build(report.Items);
    }

    private TeacherSalaryReportDto BuildReport(List<TeacherSalary> salaries)
    {
        var items = salaries.Select(s => new TeacherSalaryReportItemDto
        {
            Id = s.Id,
            TeacherName = s.Teacher?.Name ?? string.Empty,
            DaysAttended = s.DaysAttended,
            TotalHours = s.TotalHours,
            BaseSalary = s.BaseSalary,
            CalculatedSalary = s.CalculatedSalary,
            Deduction = (s.BaseSalary ?? 0) - s.CalculatedSalary,
        }).ToList();

        var totalTeachers = items.Count;
        var totalSalary = items.Sum(s => s.CalculatedSalary);

        return new TeacherSalaryReportDto
        {
            Summary = new TeacherSalaryReportSummaryDto
            {
                TotalTeachers = totalTeachers,
                TotalSalary = totalSalary,
                AverageSalary = totalTeachers > 0 ? totalSalary / totalTeachers : 0,
                FullAttendance = items.Count(s => s.DaysAttended >= TeacherSalaryCalculationHelper.RequiredDays),
                WithDeductions = items.Count(s => s.Deduction > 0),
            },
            Items = items,
        };
    }

    private static TeacherSalary BuildEntity(SaveTeacherSalaryRequestDto request)
    {
        var entity = new TeacherSalary();
        ApplySave(entity, request);
        return entity;
    }

    private static void ApplySave(TeacherSalary entity, SaveTeacherSalaryRequestDto request)
    {
        entity.TeacherId = request.TeacherId;
        entity.Month = request.Month;
        entity.Year = request.Year;
        entity.BaseSalary = request.BaseSalary;
        entity.DaysAttended = request.DaysAttended;
        entity.TotalHours = request.TotalHours;
        entity.CalculatedSalary = request.CalculatedSalary;
        entity.Status = "paid";
        entity.Notes = request.Notes;
        entity.DayOffDate = request.DayOffDate;
    }

    private async Task<List<TeacherAttendance>> GetMonthAttendancesAsync(
        int teacherId,
        int month,
        int year,
        CancellationToken cancellationToken)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        return await repository.GetMonthAttendancesAsync(teacherId, startDate, endDate, cancellationToken);
    }

    private async Task<TeacherSalary> GetScopedSalaryAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("الراتب غير موجود");

        if (entity.Teacher?.IsGirlTeacher != currentUser.IsGirlTeacher)
            throw new KeyNotFoundException("الراتب غير موجود");

        return entity;
    }

    private void EnsureAdmin()
    {
        if (!currentUser.IsAdmin)
            throw new UnauthorizedAccessException("غير مصرح");
    }

    private void EnsureCanModify()
    {
        if (!currentUser.CanModify)
            throw new UnauthorizedAccessException("ليس لديك صلاحية التعديل");
    }
}
