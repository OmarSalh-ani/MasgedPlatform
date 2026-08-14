import 'package:masged_parent_app/core/platform/export_file_name.dart';
import 'package:masged_parent_app/teacher_core/network/api_client.dart';

import '../models/memorization_revision_report_models.dart';

class MemorizationRevisionReportApi {
  MemorizationRevisionReportApi(this._client);

  final TeacherApiClient _client;

  Future<ExportedReportFile> exportReport({
    required DateTime fromDate,
    required DateTime toDate,
    required ReportExportFormat format,
  }) async {
    final from = _formatDate(fromDate);
    final to = _formatDate(toDate);
    final result = await _client.getBytes(
      '/api/memorizationrevisionreport/export',
      queryParameters: {
        'fromDate': from,
        'toDate': to,
        'format': format.apiValue,
      },
    );

    final extension = format == ReportExportFormat.excel ? 'xlsx' : 'pdf';

    return ExportedReportFile(
      bytes: result.bytes,
      fileName: resolveExportFileName(
        serverFileName: result.fileName,
        fallbackBaseName: 'circle_memorization_report',
        extension: extension,
      ),
      format: format,
    );
  }

  static String _formatDate(DateTime date) {
    final y = date.year.toString().padLeft(4, '0');
    final m = date.month.toString().padLeft(2, '0');
    final d = date.day.toString().padLeft(2, '0');
    return '$y-$m-$d';
  }
}
