using System.Globalization;
using AdminAPI.DTOs.TeachersAttendance;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;

namespace AdminAPI.Services;

public class TeachersAttendanceService(
    ITeachersAttendanceRepository repository,
    ICurrentUserContext currentUser,
    IWorkDayService workDayService) : ITeachersAttendanceService
{
    private static readonly CultureInfo DisplayCulture = CultureInfo.InvariantCulture;

    public async Task<TeachersAttendanceFilterOptionsDto> GetFilterOptionsAsync(
        CancellationToken cancellationToken = default)
    {
        var teachers = await repository.GetTeachersAsync(currentUser.IsGirlTeacher, cancellationToken);
        return new TeachersAttendanceFilterOptionsDto { Teachers = teachers };
    }

    public async Task<TeachersAttendanceListResponseDto> GetListAsync(
        string? fromDate,
        string? toDate,
        int? teacherId,
        CancellationToken cancellationToken = default)
    {
        var from = ResolveFromDate(fromDate);
        var toInclusive = ResolveToDateInclusive(toDate);
        var normalizedTeacherId = teacherId is > 0 ? teacherId : null;

        var rows = await repository.GetAttendanceRowsAsync(
            currentUser.IsGirlTeacher,
            from,
            toInclusive,
            normalizedTeacherId,
            cancellationToken);

        var workDayNumbers = await workDayService.GetWorkDayNumbersAsync(cancellationToken);
        var workDaySet = workDayNumbers.ToHashSet();
        var items = rows.Select(row => MapRow(row, workDaySet)).ToList();

        return new TeachersAttendanceListResponseDto
        {
            FromDate = from.ToString("yyyy-MM-dd", DisplayCulture),
            ToDate = toInclusive.Date.ToString("yyyy-MM-dd", DisplayCulture),
            Items = items,
        };
    }

    public async Task<byte[]> ExportExcelAsync(
        string? fromDate,
        string? toDate,
        int? teacherId,
        CancellationToken cancellationToken = default)
    {
        var report = await GetListAsync(fromDate, toDate, teacherId, cancellationToken);
        return TeachersAttendanceExcelExporter.Build(report.Items, report.FromDate, report.ToDate);
    }

    private static TeachersAttendanceRowDto MapRow(
        TeachersAttendanceSourceRow row,
        HashSet<int> workDaySet)
    {
        if (!workDaySet.Contains((int)row.AttendanceDateTime.DayOfWeek))
        {
            return new TeachersAttendanceRowDto
            {
                TeacherName = row.TeacherName,
                AttendanceDateTime = FormatAttendanceDateTime(row.AttendanceDateTime),
                DepartureDateTime = null,
                HoursWorked = 0m,
                Status = "اجازة",
                StatusClass = "status-vacation",
            };
        }

        var hasDeparture = row.DepartureDateTime.HasValue;
        var hours = hasDeparture
            ? Math.Round((decimal)(row.DepartureDateTime!.Value - row.AttendanceDateTime).TotalHours, 2)
            : 0m;

        return new TeachersAttendanceRowDto
        {
            TeacherName = row.TeacherName,
            AttendanceDateTime = FormatAttendanceDateTime(row.AttendanceDateTime),
            DepartureDateTime = hasDeparture
                ? row.DepartureDateTime!.Value.ToString("dd/MM/yyyy HH:mm", DisplayCulture)
                : null,
            HoursWorked = hours,
            Status = hasDeparture ? "حاضر" : "لم يغادر",
            StatusClass = hasDeparture ? "status-present" : "status-absent",
        };
    }

    private static string FormatAttendanceDateTime(DateTime value) =>
        value.ToString("dd/MM/yyyy hh:mm tt", DisplayCulture);

    private static DateTime ResolveFromDate(string? fromDate)
    {
        if (DateTime.TryParse(fromDate, out var parsed))
            return parsed;

        return KuwaitTime.Now.AddDays(-30);
    }

    private static DateTime ResolveToDateInclusive(string? toDate)
    {
        var parsed = DateTime.TryParse(toDate, out var value) ? value : KuwaitTime.Now;
        return parsed.Date.AddDays(1).AddSeconds(-1);
    }
}
