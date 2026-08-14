enum ReportExportFormat {
  pdf,
  excel;

  String get apiValue => this == ReportExportFormat.excel ? 'excel' : 'pdf';

  String get label => this == ReportExportFormat.excel ? 'Excel' : 'PDF';

  String get mimeType => this == ReportExportFormat.excel
      ? 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
      : 'application/pdf';
}

class ExportedReportFile {
  const ExportedReportFile({
    required this.bytes,
    required this.fileName,
    required this.format,
  });

  final List<int> bytes;
  final String fileName;
  final ReportExportFormat format;
}
