using AdminAPI.DTOs.AttendanceReport;
using AdminAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

internal static class AttendanceReportRowBuilder
{
    private static readonly Dictionary<DayOfWeek, string> ArabicDayNames = new()
    {
        [DayOfWeek.Sunday] = "الأحد",
        [DayOfWeek.Monday] = "الاثنين",
        [DayOfWeek.Tuesday] = "الثلاثاء",
        [DayOfWeek.Wednesday] = "الأربعاء",
        [DayOfWeek.Thursday] = "الخميس",
        [DayOfWeek.Friday] = "الجمعة",
        [DayOfWeek.Saturday] = "السبت",
    };

    public static async Task<List<AttendanceReportRowDto>> BuildRowsAsync(
        IQueryable<RegisterForm> query,
        DateTime fromDate,
        DateTime toDateEnd,
        string attendanceFilter,
        IReadOnlyList<int> workDayNumbers,
        CancellationToken cancellationToken)
    {
        var workDaySet = workDayNumbers.ToHashSet();
        var from = fromDate.Date;
        var toEnd = toDateEnd.Date;
        var daysDiff = (toEnd - from).Days + 1;

        if (daysDiff > 365)
            throw new InvalidOperationException("تاريخ الفترة كبير جداً. الحد الأقصى هو 365 يوم. يرجى تقليل فترة التقرير.");

        var reportData = await query
            .Select(student => new StudentReportData
            {
                StudentId = student.Id,
                StudentName = student.StudentName,
                CircleName = student.QuranCircle!.Name,
                TeacherName = student.QuranCircle!.Teacher != null
                    ? student.QuranCircle!.Teacher!.Name
                    : "غير محدد",
                FatherPhone = student.FatherPhone,
                AttendanceRecords = student.CircleAttendances
                    .Where(a => a.AttendanceDateTime.Date >= from && a.AttendanceDateTime.Date <= toEnd)
                    .Select(a => new AttendanceRecordData
                    {
                        Date = a.AttendanceDateTime,
                        IsPresent = a.IsHere,
                    })
                    .ToList(),
                DepartureRecords = student.CircleAttendances
                    .Where(d => d.DepartureDate.HasValue
                        && d.DepartureDate.Value.Date >= from
                        && d.DepartureDate.Value.Date <= toEnd)
                    .Select(d => new DepartureRecordData
                    {
                        Date = d.DepartureDate!.Value,
                    })
                    .ToList(),
            })
            .ToListAsync(cancellationToken);

        var rows = new List<AttendanceReportRowDto>();

        foreach (var student in reportData)
        {
            var attendanceByDate = student.AttendanceRecords
                .GroupBy(a => a.Date.Date)
                .ToDictionary(g => g.Key, g => g.First().IsPresent);

            var departureByDate = student.DepartureRecords
                .GroupBy(d => d.Date.Date)
                .ToDictionary(g => g.Key, g => g.First().Date);

            for (var date = from; date <= toEnd; date = date.AddDays(1))
            {
                if (!workDaySet.Contains((int)date.DayOfWeek))
                {
                    rows.Add(BuildVacationRow(student, date));
                    continue;
                }

                attendanceByDate.TryGetValue(date, out var isPresent);
                var hasAttendance = attendanceByDate.ContainsKey(date);
                departureByDate.TryGetValue(date, out var departureDateTime);
                var hasDeparture = departureByDate.ContainsKey(date);

                string status;
                string color;

                if (hasAttendance && isPresent)
                {
                    if (hasDeparture)
                    {
                        status = "حاضر وانصرف";
                        color = "green";
                    }
                    else
                    {
                        status = "حاضر ولم ينصرف";
                        color = "yellow";
                    }
                }
                else
                {
                    status = "غائب";
                    color = "red";
                }

                rows.Add(new AttendanceReportRowDto
                {
                    StudentId = student.StudentId,
                    StudentName = student.StudentName,
                    CircleName = student.CircleName,
                    TeacherName = student.TeacherName,
                    FatherPhone = student.FatherPhone,
                    Date = date.ToString("yyyy-MM-dd"),
                    DayOfWeek = ArabicDayNames[date.DayOfWeek],
                    IsPresent = hasAttendance && isPresent,
                    IsDeparted = hasDeparture,
                    DepartureTime = hasDeparture ? departureDateTime.ToString("hh:mm tt") : null,
                    Status = status,
                    Color = color,
                });
            }
        }

        return ApplyAttendanceFilter(rows, attendanceFilter);
    }

    public static AttendanceReportSummaryDto BuildSummary(IReadOnlyList<AttendanceReportRowDto> rows) =>
        new()
        {
            TotalStudents = rows.Select(x => x.StudentId).Distinct().Count(),
            TotalDays = rows.Select(x => x.Date).Distinct().Count(),
            TotalAttendance = rows.Count(x => x.IsPresent),
            TotalDeparture = rows.Count(x => x.IsDeparted),
        };

    private static AttendanceReportRowDto BuildVacationRow(StudentReportData student, DateTime date) =>
        new()
        {
            StudentId = student.StudentId,
            StudentName = student.StudentName,
            CircleName = student.CircleName,
            TeacherName = student.TeacherName,
            FatherPhone = student.FatherPhone,
            Date = date.ToString("yyyy-MM-dd"),
            DayOfWeek = ArabicDayNames[date.DayOfWeek],
            IsPresent = false,
            IsDeparted = false,
            DepartureTime = null,
            Status = "اجازة",
            Color = "gray",
        };

    private static List<AttendanceReportRowDto> ApplyAttendanceFilter(
        List<AttendanceReportRowDto> rows,
        string attendanceFilter) =>
        attendanceFilter.ToLowerInvariant() switch
        {
            "present" => rows.Where(x => x.IsPresent).ToList(),
            "departed" => rows.Where(x => x.IsDeparted).ToList(),
            "absent" => rows.Where(x => !x.IsPresent && x.Color != "gray").ToList(),
            _ => rows,
        };

    private sealed class StudentReportData
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string CircleName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public string FatherPhone { get; set; } = string.Empty;
        public List<AttendanceRecordData> AttendanceRecords { get; set; } = [];
        public List<DepartureRecordData> DepartureRecords { get; set; } = [];
    }

    private sealed class AttendanceRecordData
    {
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
    }

    private sealed class DepartureRecordData
    {
        public DateTime Date { get; set; }
    }
}
