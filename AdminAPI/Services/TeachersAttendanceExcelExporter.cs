using AdminAPI.DTOs.TeachersAttendance;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace AdminAPI.Services;

public static class TeachersAttendanceExcelExporter
{
    public static byte[] Build(
        IReadOnlyList<TeachersAttendanceRowDto> items,
        string fromDate,
        string toDate)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("حضور المعلمين");

        worksheet.Cells[1, 1].Value = $"تقرير حضور المعلمين من {fromDate} إلى {toDate}";
        worksheet.Cells[1, 1, 1, 5].Merge = true;
        worksheet.Cells[1, 1].Style.Font.Bold = true;
        worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        const int headerRow = 3;
        worksheet.Cells[headerRow, 1].Value = "اسم المعلم";
        worksheet.Cells[headerRow, 2].Value = "وقت الحضور";
        worksheet.Cells[headerRow, 3].Value = "وقت المغادرة";
        worksheet.Cells[headerRow, 4].Value = "عدد الساعات";
        worksheet.Cells[headerRow, 5].Value = "الحالة";

        using (var range = worksheet.Cells[headerRow, 1, headerRow, 5])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(124, 135, 56));
            range.Style.Font.Color.SetColor(Color.White);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        var row = headerRow + 1;
        foreach (var item in items)
        {
            worksheet.Cells[row, 1].Value = item.TeacherName;
            worksheet.Cells[row, 2].Value = item.AttendanceDateTime;
            worksheet.Cells[row, 3].Value = item.DepartureDateTime ?? "-";
            worksheet.Cells[row, 4].Value = item.HoursWorked;
            worksheet.Cells[row, 5].Value = item.Status;
            row++;
        }

        worksheet.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }
}
