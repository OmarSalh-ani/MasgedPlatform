using System.Globalization;
using AdminAPI.DTOs.CircleMemorizationRevisionReport;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AdminAPI.Services;

public static class CircleMemorizationRevisionReportPdfExporter
{
    private static readonly Color HeaderBlue = Color.FromRGB(33, 118, 166);
    private static readonly Color HeaderText = Colors.White;
    private static readonly Color BodyText = Color.FromRGB(33, 37, 41);
    private static readonly Color BorderColor = Color.FromRGB(214, 222, 230);
    private static readonly Color RowAltBackground = Color.FromRGB(248, 251, 253);
    private static readonly Color ChipBackground = Color.FromRGB(232, 244, 252);
    private static readonly Color ChipBorder = Color.FromRGB(147, 196, 225);
    private static readonly Color ChipTitle = Color.FromRGB(21, 87, 130);
    private static readonly Color ChipRange = Color.FromRGB(66, 99, 122);

    public static byte[] Build(CircleMemorizationRevisionReportMetaDto meta)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        CircleMemorizationRevisionReportAssets.EnsureFontsRegistered();

        var logoBytes = CircleMemorizationRevisionReportAssets.TryReadLogoBytes();
        var fontRegular = CircleMemorizationRevisionReportAssets.FontRegular;
        var fontBold = CircleMemorizationRevisionReportAssets.FontBold;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(24);
                page.DefaultTextStyle(x => x
                    .FontSize(9)
                    .FontFamily(fontRegular)
                    .FontColor(BodyText));
                page.ContentFromRightToLeft();

                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        if (logoBytes is { Length: > 0 })
                            row.ConstantItem(56).Height(56).Image(logoBytes).FitArea();

                        row.RelativeItem().Column(c =>
                        {
                            c.Item().AlignCenter().Text(meta.MosqueName)
                                .FontFamily(fontBold).Bold().FontSize(16).FontColor(HeaderBlue);
                            c.Item().AlignCenter()
                                .Text("تقرير الحفظ والمراجعة لحلقة " + meta.CircleName)
                                .FontFamily(fontBold).Bold().FontSize(13).FontColor(BodyText);
                            c.Item().AlignCenter()
                                .Text("تمت طباعته بواسطة المعلم " + meta.TeacherName)
                                .FontFamily(fontRegular).FontSize(10);
                            c.Item().AlignCenter()
                                .Text("بتاريخ " + meta.PrintedAt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture))
                                .FontFamily(fontRegular).FontSize(10);
                            c.Item().AlignCenter()
                                .Text("الفترة من " +
                                      meta.FromDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) +
                                      " الى " +
                                      meta.ToDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture))
                                .FontFamily(fontRegular).FontSize(9).FontColor(Colors.Grey.Darken1);
                        });

                        if (logoBytes is { Length: > 0 })
                            row.ConstantItem(56);
                    });
                    col.Item().PaddingTop(10).LineHorizontal(1.5f).LineColor(HeaderBlue);
                });

                page.Content().PaddingTop(12).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(36);
                        columns.RelativeColumn(2);
                        columns.ConstantColumn(52);
                        columns.ConstantColumn(64);
                        columns.RelativeColumn(3.2f);
                        columns.RelativeColumn(3.2f);
                    });

                    table.Header(header =>
                    {
                        foreach (var title in new[]
                                 { "التسلسل", "اسم الطالب", "اليوم", "التاريخ", "الجديد", "المراجعة" })
                        {
                            header.Cell().Element(HeaderCellStyle)
                                .Text(title)
                                .FontFamily(fontBold)
                                .Bold()
                                .FontColor(HeaderText)
                                .FontSize(10);
                        }
                    });

                    for (var index = 0; index < meta.Rows.Count; index++)
                    {
                        var row = meta.Rows[index];
                        var altRow = index % 2 == 1;

                        table.Cell().Element(c => BodyCellStyle(c, altRow))
                            .Text(row.Sequence.ToString(CultureInfo.InvariantCulture))
                            .FontFamily(fontRegular).FontSize(8.5f);
                        table.Cell().Element(c => BodyCellStyle(c, altRow))
                            .Text(row.StudentName)
                            .FontFamily(fontBold).Bold().FontSize(8.5f);
                        table.Cell().Element(c => BodyCellStyle(c, altRow))
                            .Text(row.DayName).FontSize(8.5f);
                        table.Cell().Element(c => BodyCellStyle(c, altRow))
                            .Text(row.Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture))
                            .FontSize(8.5f);
                        table.Cell().Element(c => BodyCellStyle(c, altRow))
                            .Element(cell => RenderChips(cell, row.NewMemorizationChips, fontRegular, fontBold));
                        table.Cell().Element(c => BodyCellStyle(c, altRow))
                            .Element(cell => RenderChips(cell, row.RevisionChips, fontRegular, fontBold));
                    }
                });

                page.Footer().AlignCenter().Text(t =>
                {
                    t.DefaultTextStyle(x => x.FontFamily(fontRegular).FontSize(8).FontColor(Colors.Grey.Darken1));
                    t.Span("صفحة ");
                    t.CurrentPageNumber();
                    t.Span(" من ");
                    t.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    private static IContainer HeaderCellStyle(IContainer container) =>
        container
            .Background(HeaderBlue)
            .Border(0.75f)
            .BorderColor(HeaderBlue)
            .PaddingVertical(8)
            .PaddingHorizontal(6)
            .AlignCenter()
            .AlignMiddle();

    private static IContainer BodyCellStyle(IContainer container, bool altRow) =>
        container
            .Background(altRow ? RowAltBackground : Colors.White)
            .Border(0.75f)
            .BorderColor(BorderColor)
            .PaddingVertical(6)
            .PaddingHorizontal(5)
            .AlignMiddle()
            .AlignCenter();

    private static void RenderChips(
        IContainer container,
        IReadOnlyList<CircleMemorizationSurahChipDto> chips,
        string fontRegular,
        string fontBold)
    {
        if (chips.Count == 0)
        {
            container.AlignCenter().Text("—")
                .FontFamily(fontRegular)
                .FontSize(8)
                .FontColor(Colors.Grey.Medium);
            return;
        }

        container.AlignRight().Column(column =>
        {
            column.Spacing(4);
            foreach (var chip in chips)
            {
                column.Item().AlignRight().Element(ChipStyle).Column(chipColumn =>
                {
                    chipColumn.Item().Text(chip.Title)
                        .FontFamily(fontBold)
                        .Bold()
                        .FontSize(8)
                        .FontColor(ChipTitle);

                    if (!string.IsNullOrWhiteSpace(chip.RangeText))
                    {
                        chipColumn.Item().PaddingTop(1).Text(chip.RangeText)
                            .FontFamily(fontRegular)
                            .FontSize(7.5f)
                            .FontColor(ChipRange);
                    }
                });
            }
        });
    }

    private static IContainer ChipStyle(IContainer container) =>
        container
            .Background(ChipBackground)
            .Border(1)
            .BorderColor(ChipBorder)
            .PaddingVertical(4)
            .PaddingHorizontal(8);
}
