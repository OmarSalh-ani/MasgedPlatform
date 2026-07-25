using AdminAPI.DTOs.WomansActivities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace AdminAPI.Services;

public static class WomansActivityExcelExporter
{
    public static byte[] Build(IReadOnlyList<WomanActivityListItemDto> items)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Activities");

        worksheet.Cells[1, 1].Value = "رقم النشاط";
        worksheet.Cells[1, 2].Value = "اسم النشاط";
        worksheet.Cells[1, 3].Value = "مرئي";

        using (var range = worksheet.Cells[1, 1, 1, 3])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
        }

        var row = 2;
        foreach (var activity in items)
        {
            worksheet.Cells[row, 1].Value = activity.Id;
            worksheet.Cells[row, 2].Value = activity.Name;
            worksheet.Cells[row, 3].Value = activity.IsVisible ? "نعم" : "لا";
            row++;
        }

        worksheet.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }
}
