using System.Globalization;
using System.Security.Claims;
using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasgedParentMobileAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly StudentService _studentService;
    private readonly IWorkDayService _workDayService;

    public AttendanceController(StudentService studentService, IWorkDayService workDayService)
    {
        _studentService = studentService;
        _workDayService = workDayService;
    }

    /// <summary>
    /// Returns daily attendance for a student in the given month (defaults to current month).
    /// </summary>
    [HttpGet("{studentId:int}")]
    public async Task<ActionResult<AttendanceMonthResponseDto>> GetStudentAttendance(
        int studentId,
        [FromQuery] int? year = null,
        [FromQuery] int? month = null)
    {
        var fatherPhone = User.FindFirstValue("fatherPhone");
        if (fatherPhone == null) return Unauthorized();

        var today = DateTime.Today;
        var selectedYear = year ?? today.Year;
        var selectedMonth = month ?? today.Month;

        if (selectedMonth < 1 || selectedMonth > 12)
            return BadRequest(new { message = "الشهر غير صالح" });

        if (selectedYear < 2000 || selectedYear > today.Year + 1)
            return BadRequest(new { message = "السنة غير صالحة" });

        var student = await _studentService.GetParentStudentByIdAsync(fatherPhone, studentId);
        if (student == null) return NotFound();

        var monthStart = new DateTime(selectedYear, selectedMonth, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        // Do not include future days when viewing the current month
        var rangeEnd = monthEnd > today ? today : monthEnd;

        if (monthStart > today)
        {
            return Ok(new AttendanceMonthResponseDto
            {
                Year = selectedYear,
                Month = selectedMonth,
                Records = new List<AttendanceRecordDto>(),
            });
        }

        var records = await _studentService.GetAttendanceBetweenAsync(studentId, monthStart, rangeEnd);
        var departures = await _studentService.GetDeparturesBetweenAsync(studentId, monthStart, rangeEnd);

        var result = new List<AttendanceRecordDto>();
        for (var day = rangeEnd; day >= monthStart; day = day.AddDays(-1))
        {
            var isWorkDay = await _workDayService.IsWorkDayAsync(day);
            var dayRecords = records.Where(a => a.AttendanceDateTime.Date == day).ToList();
            var dayDeparture = departures.FirstOrDefault(d => d.DepartureDate == DateOnly.FromDateTime(day));
            var dayRecord = dayRecords.OrderByDescending(a => a.AttendanceDateTime).FirstOrDefault();

            var (status, statusKey) = AttendanceHelper.MapDayStatus(dayRecord, dayDeparture, isWorkDay);
            result.Add(new AttendanceRecordDto
            {
                Day = AttendanceHelper.GetArabicDayName(day.DayOfWeek),
                Date = day.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture),
                Status = status,
                StatusKey = statusKey,
            });
        }

        return Ok(new AttendanceMonthResponseDto
        {
            Year = selectedYear,
            Month = selectedMonth,
            Records = result,
        });
    }
}
