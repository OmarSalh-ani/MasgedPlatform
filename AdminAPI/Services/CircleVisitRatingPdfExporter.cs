using System.Globalization;
using AdminAPI.DTOs.CircleVisitRating;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AdminAPI.Services;

public static class CircleVisitRatingPdfExporter
{
    public static byte[] Build(
        CircleVisitRatingDetailDto detail,
        string mosqueName,
        byte[]? logoBytes)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var headerBlue = Color.FromRGB(33, 118, 166);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(28);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));
                page.ContentFromRightToLeft();

                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        if (logoBytes is { Length: > 0 })
                            row.ConstantItem(64).Height(64).Image(logoBytes).FitArea();

                        row.RelativeItem().PaddingHorizontal(8).Column(c =>
                        {
                            c.Item().AlignCenter().Text(mosqueName).Bold().FontSize(16)
                                .FontColor(headerBlue);
                            c.Item().AlignCenter().PaddingTop(4)
                                .Text("تقرير تقييم زيارة الحلقة").Bold().FontSize(14);
                        });

                        if (logoBytes is { Length: > 0 })
                            row.ConstantItem(64);
                    });

                    col.Item().PaddingTop(10).LineHorizontal(1.5f).LineColor(headerBlue);
                    col.Item().PaddingTop(10).Column(meta =>
                    {
                        meta.Item().Text($"المعلم: {detail.TeacherName}");
                        meta.Item().Text($"الحلقة: {detail.CircleName}");
                        meta.Item().Text(
                            $"تاريخ الزيارة: {detail.VisitDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}");
                        meta.Item().Text($"وقت الزيارة: {detail.VisitTime}");
                        meta.Item().Text($"رقم الزيارة هذا الشهر: {detail.VisitNumberInMonth}");
                        meta.Item().Text($"بواسطة: {detail.CreatedByName}");
                    });
                    col.Item().PaddingTop(8).LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
                });

                page.Content().PaddingTop(12).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(45);
                        columns.RelativeColumn(2.2f);
                        columns.RelativeColumn(1.4f);
                        columns.RelativeColumn(2.4f);
                    });

                    static IContainer CellStyle(IContainer c) =>
                        c.Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(6).AlignMiddle();

                    table.Header(header =>
                    {
                        foreach (var title in new[] { "التسلسل", "البند", "التقييم", "الملاحظات" })
                        {
                            header.Cell().Element(CellStyle)
                                .Background(headerBlue)
                                .AlignCenter()
                                .Text(title).Bold().FontColor(Colors.White).FontSize(10);
                        }
                    });

                    foreach (var item in detail.Items.OrderBy(i => i.Sequence))
                    {
                        table.Cell().Element(CellStyle).AlignCenter()
                            .Text(item.Sequence.ToString(CultureInfo.InvariantCulture));
                        table.Cell().Element(CellStyle).AlignRight().Text(item.Criterion);
                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Rating).Bold();
                        table.Cell().Element(CellStyle).AlignRight().Text(item.Notes ?? "—");
                    }
                });

                page.Footer().AlignCenter().DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Darken1))
                    .Text(t =>
                    {
                        t.Span("صفحة ");
                        t.CurrentPageNumber();
                        t.Span(" من ");
                        t.TotalPages();
                    });
            });
        }).GeneratePdf();
    }
}
