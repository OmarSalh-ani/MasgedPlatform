using System.Globalization;
using MasgedTeacherMobileAPI.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MasgedTeacherMobileAPI.Helpers;

public static class CircleMemorizationRevisionReportPdfExporter
{
    public static byte[] Build(CircleMemorizationRevisionReportMetaDto meta)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var logoBytes = CircleMemorizationRevisionReportAssets.TryReadLogoBytes();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));
                page.ContentFromRightToLeft();

                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        if (logoBytes is { Length: > 0 })
                            row.ConstantItem(60).Height(60).Image(logoBytes).FitArea();

                        row.RelativeItem().Column(c =>
                        {
                            c.Item().AlignCenter().Text(meta.MosqueName).Bold().FontSize(16);
                            c.Item().AlignCenter()
                                .Text("تقرير الحفظ والمراجعة لحلقة " + meta.CircleName)
                                .Bold().FontSize(13);
                            c.Item().AlignCenter()
                                .Text("تمت طباعته بواسطة المعلم " + meta.TeacherName)
                                .FontSize(11);
                            c.Item().AlignCenter()
                                .Text("بتاريخ " +
                                      meta.PrintedAt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture))
                                .FontSize(11);
                            c.Item().AlignCenter()
                                .Text("الفترة من " +
                                      meta.FromDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) +
                                      " الى " +
                                      meta.ToDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture))
                                .FontSize(10).FontColor(Colors.Grey.Darken2);
                        });

                        if (logoBytes is { Length: > 0 })
                            row.ConstantItem(60);
                    });

                    col.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                });

                page.Content().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(40);
                        columns.RelativeColumn(2);
                        columns.ConstantColumn(55);
                        columns.ConstantColumn(70);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(3);
                    });

                    static IContainer CellStyle(IContainer container) =>
                        container.Border(0.5f).Padding(4).AlignCenter().AlignMiddle();

                    table.Header(header =>
                    {
                        foreach (var title in new[]
                                 {
                                     "التسلسل", "اسم الطالب", "اليوم", "التاريخ", "الجديد", "المراجعة"
                                 })
                        {
                            header.Cell().Element(CellStyle)
                                .Background(Color.FromRGB(33, 118, 166))
                                .Text(title).Bold().FontColor(Colors.White).FontSize(9);
                        }
                    });

                    foreach (var row in meta.Rows)
                    {
                        table.Cell().Element(CellStyle)
                            .Text(row.Sequence.ToString(CultureInfo.InvariantCulture)).FontSize(8);
                        table.Cell().Element(CellStyle).Text(row.StudentName).FontSize(8);
                        table.Cell().Element(CellStyle).Text(row.DayName).FontSize(8);
                        table.Cell().Element(CellStyle)
                            .Text(row.Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)).FontSize(8);
                        table.Cell().Element(CellStyle).Text(row.NewMemorization).FontSize(8);
                        table.Cell().Element(CellStyle).Text(row.Revision).FontSize(8);
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("صفحة ");
                    text.CurrentPageNumber();
                    text.Span(" من ");
                    text.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }
}
