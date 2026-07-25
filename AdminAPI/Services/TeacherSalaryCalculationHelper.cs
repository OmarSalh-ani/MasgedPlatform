using AdminAPI.DTOs.TeacherSalaries;
using AdminAPI.Models;

namespace AdminAPI.Services;

public static class TeacherSalaryCalculationHelper
{
    public const int RequiredDays = 16;
    private const decimal MinimumValidHours = 1.5m;

    public static AttendanceCalculationResult CalculateMonthlyAttendance(
        IReadOnlyList<TeacherAttendance> attendances)
    {
        var dailyDetails = new List<DailyAttendanceDetail>();
        var validDays = 0;
        decimal totalHours = 0;

        foreach (var attendance in attendances)
        {
            if (!attendance.DepartureDateTime.HasValue)
                continue;

            var minutes = (decimal)(attendance.DepartureDateTime.Value - attendance.AttendanceDateTime).TotalMinutes;
            var hoursDecimal = Math.Round(minutes / 60, 2);
            var isValid = hoursDecimal >= MinimumValidHours;

            if (isValid)
            {
                validDays++;
                totalHours += hoursDecimal;
            }

            dailyDetails.Add(new DailyAttendanceDetail
            {
                Date = attendance.AttendanceDateTime.Date,
                AttendanceTime = attendance.AttendanceDateTime,
                DepartureTime = attendance.DepartureDateTime.Value,
                Hours = hoursDecimal,
                IsValid = isValid,
            });
        }

        return new AttendanceCalculationResult
        {
            DaysAttended = validDays,
            TotalHours = Math.Round(totalHours, 2),
            DailyDetails = dailyDetails.OrderBy(d => d.Date).ToList(),
        };
    }

    public static SalaryCalculationResult CalculateSalaryAmount(
        AttendanceCalculationResult attendanceResult,
        DateTime? dayOffDate,
        decimal baseSalary)
    {
        var daysAttended = attendanceResult.DaysAttended;
        var totalHours = attendanceResult.TotalHours;

        if (dayOffDate.HasValue)
        {
            var dayOff = dayOffDate.Value.Date;
            var dayOffDetail = attendanceResult.DailyDetails
                .FirstOrDefault(d => d.Date.Date == dayOff);
            if (dayOffDetail is { IsValid: true })
            {
                daysAttended--;
                totalHours -= dayOffDetail.Hours;
            }
        }

        var calculatedSalary = baseSalary;
        decimal deduction = 0;

        if (daysAttended < RequiredDays)
        {
            var missingDays = RequiredDays - daysAttended;
            var perDayAmount = baseSalary / RequiredDays;
            deduction = missingDays * perDayAmount;
            calculatedSalary = baseSalary - deduction;
        }

        return new SalaryCalculationResult
        {
            DaysAttended = daysAttended,
            TotalHours = Math.Round(totalHours, 2),
            BaseSalary = baseSalary,
            CalculatedSalary = Math.Round(calculatedSalary, 2),
            Deduction = Math.Round(deduction, 2),
            RequiredDays = RequiredDays,
            DailyDetails = attendanceResult.DailyDetails,
        };
    }

    public static AttendanceCalculationResultDto ToDto(AttendanceCalculationResult result) =>
        new()
        {
            DaysAttended = result.DaysAttended,
            TotalHours = result.TotalHours,
            DailyDetails = result.DailyDetails.Select(ToDailyDto).ToList(),
        };

    public static SalaryCalculationResultDto ToDto(SalaryCalculationResult result) =>
        new()
        {
            DaysAttended = result.DaysAttended,
            TotalHours = result.TotalHours,
            BaseSalary = result.BaseSalary,
            CalculatedSalary = result.CalculatedSalary,
            Deduction = result.Deduction,
            RequiredDays = result.RequiredDays,
            DailyDetails = result.DailyDetails.Select(ToDailyDto).ToList(),
        };

    private static DailyAttendanceDetailDto ToDailyDto(DailyAttendanceDetail detail) =>
        new()
        {
            Date = detail.Date.ToString("yyyy-MM-dd"),
            DateFormatted = detail.Date.ToString("dd/MM/yyyy"),
            AttendanceTime = detail.AttendanceTime.ToString("HH:mm"),
            DepartureTime = detail.DepartureTime.ToString("HH:mm"),
            Hours = detail.Hours,
            IsValid = detail.IsValid,
        };

    public sealed class DailyAttendanceDetail
    {
        public DateTime Date { get; init; }
        public DateTime AttendanceTime { get; init; }
        public DateTime DepartureTime { get; init; }
        public decimal Hours { get; init; }
        public bool IsValid { get; init; }
    }

    public sealed class AttendanceCalculationResult
    {
        public int DaysAttended { get; init; }
        public decimal TotalHours { get; init; }
        public List<DailyAttendanceDetail> DailyDetails { get; init; } = [];
    }

    public sealed class SalaryCalculationResult
    {
        public int DaysAttended { get; init; }
        public decimal TotalHours { get; init; }
        public decimal BaseSalary { get; init; }
        public decimal CalculatedSalary { get; init; }
        public decimal Deduction { get; init; }
        public int RequiredDays { get; init; }
        public List<DailyAttendanceDetail> DailyDetails { get; init; } = [];
    }
}
