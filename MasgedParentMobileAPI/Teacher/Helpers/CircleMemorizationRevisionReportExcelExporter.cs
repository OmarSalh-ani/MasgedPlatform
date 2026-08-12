using System.Drawing;
using System.Globalization;
using MasgedTeacherMobileAPI.Dtos;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MasgedTeacherMobileAPI.Helpers;

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
        ws.Cells[headerRow, 1].Value = "التسلسل";
        ws.Cells[headerRow, 2].Value = "اسم الطالب";
        ws.Cells[headerRow, 3].Value = "اليوم";
        ws.Cells[headerRow, 4].Value = "التاريخ";
        ws.Cells[headerRow, 5].Value = "الجديد";
        ws.Cells[headerRow, 6].Value = "المراجعة";

        using (var hr = ws.Cells[headerRow, 1, headerRow, colCount])
        {
            hr.Style.Font.Bold = true;
            hr.Style.Fill.PatternType = ExcelFillStyle.Solid;
            hr.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(33, 118, 166));
            hr.Style.Font.Color.SetColor(Color.White);
            hr.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            hr.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
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

            using (var range = ws.Cells[r, 1, r, colCount])
            {
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.WrapText = true;
            }

            r++;
        }

        var lastDataRow = Math.Max(headerRow, r - 1);
        using (var table = ws.Cells[headerRow, 1, lastDataRow, colCount])
        {
            table.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            table.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            table.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            table.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        ws.Cells.AutoFitColumns();
        ws.Column(2).Width = Math.Max(ws.Column(2).Width, 22);
        ws.Column(5).Width = Math.Max(ws.Column(5).Width, 40);
        ws.Column(6).Width = Math.Max(ws.Column(6).Width, 40);

        return package.GetAsByteArray();
    }

    private static void TryAddLogo(ExcelWorksheet ws)
    {
        var logoPath = CircleMemorizationRevisionReportAssets.ResolveLogoPath();
        if (logoPath is null)
            return;

        try
        {
            var picture = ws.Drawings.AddPicture("MosqueLogo", new FileInfo(logoPath));
            picture.SetPosition(0, 0, 0, 0);
            picture.SetSize(70, 70);
        }
        catch
        {
            // Logo is optional; report content still exports without it.
        }
    }
}
