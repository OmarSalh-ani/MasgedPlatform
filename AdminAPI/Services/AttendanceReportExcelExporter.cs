using AdminAPI.DTOs.AttendanceReport;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace AdminAPI.Services;

public static class AttendanceReportExcelExporter
{
    public static byte[] Build(IReadOnlyList<AttendanceReportRowDto> rows)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("تقرير الحضور");
        worksheet.View.RightToLeft = true;

        string[] headers =
        [
            "اسم الطالب", "الحلقة", "اسم المعلم", "هاتف ولي الأمر",
            "التاريخ", "اليوم", "الحالة", "الانصراف", "وقت الانصراف",
        ];

        for (var i = 0; i < headers.Length; i++)
            worksheet.Cells[1, i + 1].Value = headers[i];

        using (var headerRange = worksheet.Cells[1, 1, 1, headers.Length])
        {
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            headerRange.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        var absentColor = Color.FromArgb(255, 235, 238);
        var presentNoDepartureColor = Color.FromArgb(255, 248, 225);
        var presentDepartedColor = Color.FromArgb(232, 245, 232);

        var row = 2;
        foreach (var item in rows)
        {
            worksheet.Cells[row, 1].Value = item.StudentName;
            worksheet.Cells[row, 2].Value = item.CircleName;
            worksheet.Cells[row, 3].Value = item.TeacherName;
            worksheet.Cells[row, 4].Value = item.FatherPhone ?? "-";
            worksheet.Cells[row, 5].Value = item.Date;
            worksheet.Cells[row, 6].Value = item.DayOfWeek;
            worksheet.Cells[row, 7].Value = item.Status;
            worksheet.Cells[row, 8].Value = item.IsDeparted ? "انصرف" : "لم ينصرف";
            worksheet.Cells[row, 9].Value = item.DepartureTime ?? "-";

            using var rowRange = worksheet.Cells[row, 1, row, 9];
            rowRange.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            rowRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var fillColor = item.Status switch
            {
                "غائب" => absentColor,
                "حاضر ولم ينصرف" => presentNoDepartureColor,
                "حاضر وانصرف" => presentDepartedColor,
                _ => (Color?)null,
            };

            if (fillColor.HasValue)
            {
                rowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rowRange.Style.Fill.BackgroundColor.SetColor(fillColor.Value);
            }

            row++;
        }

        worksheet.Cells.AutoFitColumns();
        AddSummarySheet(package, rows);
        return package.GetAsByteArray();
    }

    private static void AddSummarySheet(ExcelPackage package, IReadOnlyList<AttendanceReportRowDto> rows)
    {
        var summarySheet = package.Workbook.Worksheets.Add("ملخص");
        summarySheet.View.RightToLeft = true;

        var absentCount = rows.Count(x => x.Status == "غائب");
        var presentNotDepartedCount = rows.Count(x => x.Status == "حاضر ولم ينصرف");
        var presentDepartedCount = rows.Count(x => x.Status == "حاضر وانصرف");

        summarySheet.Cells[1, 1].Value = "إحصائيات التقرير";
        summarySheet.Cells[1, 1].Style.Font.Bold = true;
        summarySheet.Cells[1, 1].Style.Font.Size = 14;
        summarySheet.Cells[3, 1].Value = "إجمالي السجلات:";
        summarySheet.Cells[3, 2].Value = rows.Count;
        summarySheet.Cells[4, 1].Value = "الغائبون:";
        summarySheet.Cells[4, 2].Value = absentCount;
        summarySheet.Cells[5, 1].Value = "الحضور بدون انصراف:";
        summarySheet.Cells[5, 2].Value = presentNotDepartedCount;
        summarySheet.Cells[6, 1].Value = "الحضور مع الانصراف:";
        summarySheet.Cells[6, 2].Value = presentDepartedCount;

        for (var i = 3; i <= 6; i++)
        {
            summarySheet.Cells[i, 1].Style.Font.Bold = true;
            summarySheet.Cells[i, 1, i, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        }

        summarySheet.Column(1).Width = 25;
        summarySheet.Column(2).Width = 15;
    }
}
