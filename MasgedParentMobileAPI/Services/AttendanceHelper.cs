using MasgedParentMobileAPI.Models;

namespace MasgedParentMobileAPI.Services;

public static class AttendanceHelper
{
    public const string VacationStatusKey = "vacation";
    public const string VacationStatusAr = "اجازة";

    private static readonly string[] WeekDays =
    {
        "السبت", "الأحد", "الاثنين", "الثلاثاء", "الأربعاء", "الخميس", "الجمعة"
    };

    public static DateTime GetWeekStartSaturday(DateTime date)
    {
        var day = date.Date;
        while (day.DayOfWeek != DayOfWeek.Saturday)
            day = day.AddDays(-1);
        return day;
    }

    public static Dictionary<string, bool?> BuildWeeklyAttendance(
        IEnumerable<CircleAttendance> weekRecords,
        DateTime weekStart,
        IReadOnlyList<int> workDayNumbers)
    {
        var workDaySet = workDayNumbers.ToHashSet();
        var result = new Dictionary<string, bool?>();
        for (var i = 0; i < 7; i++)
        {
            var day = weekStart.AddDays(i);
            var dayKey = WeekDays[i];

            if (!workDaySet.Contains((int)day.DayOfWeek))
            {
                result[dayKey] = day > DateTime.Today ? null : null;
                continue;
            }

            var records = weekRecords
                .Where(a => a.AttendanceDateTime.Date == day)
                .ToList();

            if (records.Count == 0)
                result[dayKey] = day > DateTime.Today ? null : false;
            else if (records.Any(r => r.IsHere))
                result[dayKey] = true;
            else
                result[dayKey] = false;
        }

        return result;
    }

    public static string DetermineStatus(
        CircleAttendance? todayAttendance,
        CircleDeparture? todayDeparture,
        bool isWorkDay)
    {
        if (!isWorkDay)
            return VacationStatusKey;

        if (todayAttendance == null)
            return "absent";

        if (!todayAttendance.IsHere)
            return "absent";

        var hasDeparture = todayDeparture != null ||
            todayAttendance.DepartureDate.HasValue;

        return hasDeparture ? "left" : "inMasged";
    }

    public static string? FormatTimeArabic(DateTime? dateTime)
    {
        if (!dateTime.HasValue)
            return null;

        return dateTime.Value.ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture)
            .Replace("AM", "ص")
            .Replace("PM", "م");
    }

    public static string? FormatTimeOnlyArabic(TimeOnly? time)
    {
        if (!time.HasValue)
            return null;

        var dt = DateTime.Today.Add(time.Value.ToTimeSpan());
        return FormatTimeArabic(dt);
    }

    public static string GetArabicDayName(DayOfWeek dayOfWeek) => dayOfWeek switch
    {
        DayOfWeek.Saturday => "السبت",
        DayOfWeek.Sunday => "الأحد",
        DayOfWeek.Monday => "الاثنين",
        DayOfWeek.Tuesday => "الثلاثاء",
        DayOfWeek.Wednesday => "الأربعاء",
        DayOfWeek.Thursday => "الخميس",
        DayOfWeek.Friday => "الجمعة",
        _ => dayOfWeek.ToString(),
    };

    public static (string Status, string StatusKey) MapDayStatus(
        CircleAttendance? attendance,
        CircleDeparture? departure,
        bool isWorkDay)
    {
        if (!isWorkDay)
            return (VacationStatusAr, VacationStatusKey);

        if (attendance == null || !attendance.IsHere)
            return ("غياب", "absent");

        var attendTime = attendance.AttendanceDateTime.TimeOfDay;
        if (attendTime >= new TimeSpan(16, 0, 0))
            return ("تأخير", "late");

        return ("حضور", "present");
    }
}
