using System.Drawing;
using System.Globalization;
using System.Security.Claims;
using MasgedParentMobileAPI.Services;
using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Entities;
using MasgedTeacherMobileAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MasgedTeacherMobileAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AttendanceReportController(AppDbContext db, IWorkDayService workDayService) : ControllerBase
{
    private static readonly string[] DateFormats = { "yyyy-MM-dd", "MM/dd/yyyy", "dd/MM/yyyy", "yyyy/MM/dd" };

    [HttpGet]
    public async Task<IActionResult> GetAttendanceDepartureReport(
        [FromQuery] string fromDate,
        [FromQuery] string toDate,
        CancellationToken cancellationToken)
    {
        if (!TryGetCircleId(out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("لم يتم العثور على معرف الحلقة. يرجى تسجيل الدخول مرة أخرى."));

        if (!TryParseDateRange(fromDate, toDate, out var fromDateTime, out var toDateTime, out var dateError))
            return this.ToActionResult(GlobalResponse.BadRequest(dateError));

        var (reportData, summary) = await BuildReportAsync(circleId, fromDateTime, toDateTime, cancellationToken);

        return this.ToActionResult(GlobalResponse.Ok(new AttendanceReportResponseDto
        {
            Data = reportData,
            Summary = summary
        }));
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportToExcel(
        [FromQuery] string fromDate,
        [FromQuery] string toDate,
        CancellationToken cancellationToken)
    {
        if (!TryGetCircleId(out var circleId))
            return this.ToActionResult(GlobalResponse.BadRequest("لم يتم العثور على معرف الحلقة. يرجى تسجيل الدخول مرة أخرى."));

        if (!TryParseDateRange(fromDate, toDate, out var fromDateTime, out var toDateTime, out var dateError))
            return this.ToActionResult(GlobalResponse.BadRequest(dateError));

        var (reportData, _) = await BuildReportAsync(circleId, fromDateTime, toDateTime, cancellationToken);

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("تقرير الحضور والانصراف");

        worksheet.View.RightToLeft = true;

        worksheet.Cells[1, 1, 1, 4].Merge = true;
        worksheet.Cells[1, 1].Value = "تقرير الحضور والانصراف";
        worksheet.Cells[1, 1].Style.Font.Size = 16;
        worksheet.Cells[1, 1].Style.Font.Bold = true;
        worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.Black);

        worksheet.Cells[2, 1, 2, 4].Merge = true;
        worksheet.Cells[2, 1].Value =
            $"من تاريخ: {fromDateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)} إلى تاريخ: {toDateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
        worksheet.Cells[2, 1].Style.Font.Size = 12;
        worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        const int headerRow = 4;
        worksheet.Cells[headerRow, 1].Value = "اسم الطالب";
        worksheet.Cells[headerRow, 2].Value = "التاريخ";
        worksheet.Cells[headerRow, 3].Value = "الحضور";
        worksheet.Cells[headerRow, 4].Value = "الانصراف";

        var headerRange = worksheet.Cells[headerRow, 1, headerRow, 4];
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.Size = 12;
        headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        headerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        headerRange.Style.Font.Color.SetColor(Color.Black);
        headerRange.Style.Border.Top.Style = ExcelBorderStyle.Thick;
        headerRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
        headerRange.Style.Border.Left.Style = ExcelBorderStyle.Thick;
        headerRange.Style.Border.Right.Style = ExcelBorderStyle.Thick;

        var dataRow = headerRow + 1;
        foreach (var row in reportData)
        {
            worksheet.Cells[dataRow, 1].Value = row.StudentName;
            worksheet.Cells[dataRow, 2].Value = row.DateFormatted;
            worksheet.Cells[dataRow, 3].Value = row.AttendanceText;
            worksheet.Cells[dataRow, 4].Value = row.DepartureText;

            if (row.IsPresent)
            {
                worksheet.Cells[dataRow, 3].Style.Font.Color.SetColor(Color.FromArgb(40, 167, 69));
                worksheet.Cells[dataRow, 3].Style.Font.Bold = true;
            }
            else
            {
                worksheet.Cells[dataRow, 3].Style.Font.Color.SetColor(Color.FromArgb(220, 53, 69));
                worksheet.Cells[dataRow, 3].Style.Font.Bold = true;
            }

            var hasDeparture = row.DepartureText != "لم ينصرف";
            if (hasDeparture)
            {
                worksheet.Cells[dataRow, 4].Style.Font.Color.SetColor(Color.FromArgb(253, 126, 20));
                worksheet.Cells[dataRow, 4].Style.Font.Bold = true;
            }
            else
            {
                worksheet.Cells[dataRow, 4].Style.Font.Color.SetColor(Color.FromArgb(108, 117, 125));
                worksheet.Cells[dataRow, 4].Style.Font.Italic = true;
            }

            dataRow++;
        }

        var dataRange = worksheet.Cells[headerRow, 1, Math.Max(headerRow, dataRow - 1), 4];
        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Top.Color.SetColor(Color.Black);
        dataRange.Style.Border.Bottom.Color.SetColor(Color.Black);
        dataRange.Style.Border.Left.Color.SetColor(Color.Black);
        dataRange.Style.Border.Right.Color.SetColor(Color.Black);
        dataRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        dataRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

        worksheet.Cells.AutoFitColumns();
        worksheet.Column(1).Width = Math.Max(worksheet.Column(1).Width, 20);
        worksheet.Column(2).Width = Math.Max(worksheet.Column(2).Width, 15);
        worksheet.Column(3).Width = Math.Max(worksheet.Column(3).Width, 10);
        worksheet.Column(4).Width = Math.Max(worksheet.Column(4).Width, 25);

        var fileBytes = package.GetAsByteArray();
        var fileName =
            $"تقرير_الحضور_والانصراف_{fromDateTime:yyyy-MM-dd}_{toDateTime:yyyy-MM-dd}.xlsx";

        return this.ToActionResult(GlobalResponse.Ok(new AttendanceReportExcelResponseDto
        {
            FileData = Convert.ToBase64String(fileBytes),
            FileName = fileName,
            Message = "تم إنشاء ملف Excel بنجاح"
        }));
    }

    private async Task<(List<AttendanceReportRowDto> Rows, AttendanceReportSummaryDto Summary)> BuildReportAsync(
        int circleId,
        DateTime fromDateTime,
        DateTime toDateTime,
        CancellationToken cancellationToken)
    {
        var from = fromDateTime.Date;
        var to = toDateTime.Date;
        var toExclusive = to.AddDays(1);

        var studentIdsFromAttendance = await db.CircleAttendances
            .AsNoTracking()
            .Where(a => a.CircleId == circleId
                        && a.AttendanceDateTime >= from
                        && a.AttendanceDateTime < toExclusive)
            .Select(a => a.StudentId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var currentStudentIds = await db.RegisterForms
            .AsNoTracking()
            .Where(s => s.QuranCircleId == circleId)
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        var allStudentIds = studentIdsFromAttendance
            .Union(currentStudentIds)
            .Distinct()
            .ToList();

        var students = await db.RegisterForms
            .AsNoTracking()
            .Where(s => allStudentIds.Contains(s.Id))
            .Select(s => new { s.Id, s.StudentName })
            .ToListAsync(cancellationToken);

        var attendances = await db.CircleAttendances
            .AsNoTracking()
            .Where(a => allStudentIds.Contains(a.StudentId)
                        && a.CircleId == circleId
                        && ((a.AttendanceDateTime >= from && a.AttendanceDateTime < toExclusive)
                            || (a.DepartureDate.HasValue && a.DepartureDate >= from && a.DepartureDate < toExclusive)))
            .ToListAsync(cancellationToken);

        var workDayNumbers = await workDayService.GetWorkDayNumbersAsync(cancellationToken);
        var workDaySet = workDayNumbers.ToHashSet();

        var reportData = new List<AttendanceReportRowDto>();
        var totalAttendance = 0;
        var totalDeparture = 0;

        for (var date = from; date <= to; date = date.AddDays(1))
        {
            foreach (var student in students)
            {
                if (!workDaySet.Contains((int)date.DayOfWeek))
                {
                    reportData.Add(new AttendanceReportRowDto
                    {
                        StudentName = student.StudentName,
                        Date = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        DateFormatted = date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                        IsPresent = false,
                        AttendanceText = AttendanceHelper.VacationStatusAr,
                        DepartureTime = "",
                        DepartureText = AttendanceHelper.VacationStatusAr
                    });
                    continue;
                }

                var attendance = attendances
                    .FirstOrDefault(a => a.StudentId == student.Id && a.AttendanceDateTime == date);

                var departure = attendances
                    .FirstOrDefault(d => d.StudentId == student.Id
                                         && d.DepartureDate.HasValue
                                         && d.DepartureDate.Value.Date == date);

                var isPresent = attendance?.IsHere ?? false;

                reportData.Add(new AttendanceReportRowDto
                {
                    StudentName = student.StudentName,
                    Date = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    DateFormatted = date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    IsPresent = isPresent,
                    AttendanceText = attendance?.IsHere == true ? "نعم" : "لا",
                    DepartureTime = departure?.DepartureDate?.ToString(@"hh\:mm") ?? "",
                    DepartureText = departure?.DepartureDate is not null
                        ? $"{departure.DepartureDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)} - {departure.DepartureDate.Value:hh:mm tt}"
                        : "لم ينصرف"
                });

                var hasAttendance = attendances
                    .FirstOrDefault(a => a.StudentId == student.Id
                                         && a.AttendanceDateTime.Date == date
                                         && a.IsHere);

                if (hasAttendance is not null)
                {
                    totalAttendance++;
                    if (hasAttendance.DepartureDate.HasValue)
                        totalDeparture++;
                }
            }
        }

        var summary = new AttendanceReportSummaryDto
        {
            TotalDays = (int)(to - from).TotalDays + 1,
            TotalStudents = students.Count,
            TotalAttendance = totalAttendance,
            TotalDeparture = totalDeparture
        };

        return (reportData, summary);
    }

    private bool TryGetCircleId(out int circleId)
    {
        circleId = 0;
        var circleIdClaim = User.FindFirstValue("circleId");
        return int.TryParse(circleIdClaim, out circleId) && circleId > 0;
    }

    private static bool TryParseDateRange(
        string fromDate,
        string toDate,
        out DateTime fromDateTime,
        out DateTime toDateTime,
        out string error)
    {
        fromDateTime = default;
        toDateTime = default;
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(fromDate) || string.IsNullOrWhiteSpace(toDate))
        {
            error = "تواريخ غير صالحة";
            return false;
        }

        if (!DateTime.TryParseExact(fromDate, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateTime) ||
            !DateTime.TryParseExact(toDate, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out toDateTime))
        {
            if (!DateTime.TryParse(fromDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateTime) ||
                !DateTime.TryParse(toDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out toDateTime))
            {
                error = $"تنسيق التاريخ غير صالح. التاريخ المستلم: من '{fromDate}' إلى '{toDate}'. يرجى استخدام تنسيق صالح.";
                return false;
            }
        }

        fromDateTime = fromDateTime.Date;
        toDateTime = toDateTime.Date;
        return true;
    }
}
