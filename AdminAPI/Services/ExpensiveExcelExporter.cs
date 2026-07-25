using AdminAPI.DTOs.Expensives;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace AdminAPI.Services;

public static class ExpensiveExcelExporter
{
    public static byte[] Build(IReadOnlyList<ExpensiveListItemDto> items)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("المصروفات");

        worksheet.Cells[1, 1].Value = "رقم المصروف";
        worksheet.Cells[1, 2].Value = "سبب الصرف";
        worksheet.Cells[1, 3].Value = "القيمة";
        worksheet.Cells[1, 4].Value = "المورد";
        worksheet.Cells[1, 5].Value = "تاريخ الصرف";
        worksheet.Cells[1, 6].Value = "المسؤول";

        using (var range = worksheet.Cells[1, 1, 1, 6])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(33, 118, 166));
            range.Style.Font.Color.SetColor(Color.White);
        }

        var row = 2;
        foreach (var expense in items)
        {
            worksheet.Cells[row, 1].Value = expense.Id;
            worksheet.Cells[row, 2].Value = expense.Reason;
            worksheet.Cells[row, 3].Value = expense.TotalAmount;
            worksheet.Cells[row, 4].Value = expense.Supplier;
            worksheet.Cells[row, 5].Value = GeorgianDateFormatter.FormatDate(expense.CreatedAt);
            worksheet.Cells[row, 6].Value = expense.CreatedBy;
            row++;
        }

        worksheet.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }
}
