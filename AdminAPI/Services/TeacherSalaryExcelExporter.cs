using AdminAPI.DTOs.TeacherSalaries;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace AdminAPI.Services;

public static class TeacherSalaryExcelExporter
{
    public static byte[] Build(IReadOnlyList<TeacherSalaryReportItemDto> items)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("تقرير الرواتب");

        worksheet.Cells[1, 1].Value = "اسم المعلم";
        worksheet.Cells[1, 2].Value = "أيام الحضور";
        worksheet.Cells[1, 3].Value = "إجمالي الساعات";
        worksheet.Cells[1, 4].Value = "الراتب الأساسي";
        worksheet.Cells[1, 5].Value = "الخصومات";
        worksheet.Cells[1, 6].Value = "الراتب النهائي";
        worksheet.Cells[1, 7].Value = "الحالة";

        using (var range = worksheet.Cells[1, 1, 1, 7])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
        }

        var row = 2;
        foreach (var salary in items)
        {
            worksheet.Cells[row, 1].Value = salary.TeacherName;
            worksheet.Cells[row, 2].Value = salary.DaysAttended;
            worksheet.Cells[row, 3].Value = salary.TotalHours;
            worksheet.Cells[row, 4].Value = salary.BaseSalary ?? 0;
            worksheet.Cells[row, 5].Value = salary.Deduction;
            worksheet.Cells[row, 6].Value = salary.CalculatedSalary;
            worksheet.Cells[row, 7].Value = "paid";
            row++;
        }

        worksheet.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }
}
