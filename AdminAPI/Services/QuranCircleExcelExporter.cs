using AdminAPI.DTOs.QuranCircles;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace AdminAPI.Services;

public static class QuranCircleExcelExporter
{
    public static byte[] Build(IReadOnlyList<QuranCircleListItemDto> items)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("الحلقات");

        worksheet.Cells[1, 1].Value = "رقم الحلقة";
        worksheet.Cells[1, 2].Value = "أسم الحلقة";
        worksheet.Cells[1, 3].Value = "المعلم";
        worksheet.Cells[1, 4].Value = "عدد الطلاب";
        worksheet.Cells[1, 5].Value = "تاريخ الإنشاء";
        worksheet.Cells[1, 6].Value = "المنشئ";

        using (var range = worksheet.Cells[1, 1, 1, 6])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(33, 118, 166));
            range.Style.Font.Color.SetColor(Color.White);
        }

        var row = 2;
        foreach (var circle in items)
        {
            worksheet.Cells[row, 1].Value = circle.Id;
            worksheet.Cells[row, 2].Value = circle.Name;
            worksheet.Cells[row, 3].Value = circle.TeacherName;
            worksheet.Cells[row, 4].Value = circle.StudentsCount;
            worksheet.Cells[row, 5].Value = GeorgianDateFormatter.FormatDate(circle.CreatedAt);
            worksheet.Cells[row, 6].Value = circle.CreatedBy;
            row++;
        }

        worksheet.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }
}
