using System.Drawing;
using System.Globalization;
using AdminAPI.DTOs.CircleMemorizationRevisionReport;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace AdminAPI.Services;

public static class CircleMemorizationRevisionReportExcelExporter
{
    public static byte[] Build(CircleMemorizationRevisionReportMetaDto meta)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("تقرير الحفظ والمراجعة");
        ws.View.RightToLeft = true;
        const int colCount = 6;

        ws.Cells[1, 1, 1, colCount].Merge = true;
        ws.Cells[1, 1].Value = meta.MosqueName;
        ws.Cells[1, 1].Style.Font.Size = 16;
        ws.Cells[1, 1].Style.Font.Bold = true;
        ws.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        ws.Cells[2, 1, 2, colCount].Merge = true;
        ws.Cells[2, 1].Value = "تقرير الحفظ والمراجعة لحلقة " + meta.CircleName;
        ws.Cells[2, 1].Style.Font.Size = 14;
        ws.Cells[2, 1].Style.Font.Bold = true;
        ws.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        ws.Cells[3, 1, 3, colCount].Merge = true;
        ws.Cells[3, 1].Value = "تمت طباعته بواسطة المعلم " + meta.TeacherName;
        ws.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        ws.Cells[4, 1, 4, colCount].Merge = true;
        ws.Cells[4, 1].Value =
            "بتاريخ " + meta.PrintedAt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) +
            " | الفترة من " + meta.FromDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) +
            " الى " + meta.ToDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        ws.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        TryAddLogo(ws);

        const int headerRow = 6;
        string[] headers = ["التسلسل", "اسم الطالب", "اليوم", "التاريخ", "الجديد", "المراجعة"];
        for (var c = 0; c < headers.Length; c++)
            ws.Cells[headerRow, c + 1].Value = headers[c];

        using (var hr = ws.Cells[headerRow, 1, headerRow, colCount])
        {
            hr.Style.Font.Bold = true;
            hr.Style.Fill.PatternType = ExcelFillStyle.Solid;
            hr.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(33, 118, 166));
            hr.Style.Font.Color.SetColor(Color.White);
            hr.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        var r = headerRow + 1;
        foreach (var row in meta.Rows)
        {
            ws.Cells[r, 1].Value = row.Sequence;
            ws.Cells[r, 2].Value = row.StudentName;
            ws.Cells[r, 3].Value = row.DayName;
            ws.Cells[r, 4].Value = row.Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            ws.Cells[r, 5].Value = row.NewMemorization;
            ws.Cells[r, 6].Value = row.Revision;
            ws.Cells[r, 1, r, colCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[r, 1, r, colCount].Style.WrapText = true;
            r++;
        }

        var last = Math.Max(headerRow, r - 1);
        ws.Cells[headerRow, 1, last, colCount].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        ws.Cells[headerRow, 1, last, colCount].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        ws.Cells[headerRow, 1, last, colCount].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        ws.Cells[headerRow, 1, last, colCount].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        ws.Cells.AutoFitColumns();
        ws.Column(5).Width = Math.Max(ws.Column(5).Width, 40);
        ws.Column(6).Width = Math.Max(ws.Column(6).Width, 40);
        return package.GetAsByteArray();
    }

    private static void TryAddLogo(ExcelWorksheet ws)
    {
        var logoPath = CircleMemorizationRevisionReportAssets.ResolveLogoPath();
        if (logoPath is null) return;
        try
        {
            var picture = ws.Drawings.AddPicture("MosqueLogo", new FileInfo(logoPath));
            picture.SetPosition(0, 0, 0, 0);
            picture.SetSize(70, 70);
        }
        catch { /* optional */ }
    }
}
