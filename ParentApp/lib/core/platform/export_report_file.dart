import 'dart:ui';

import 'package:flutter/foundation.dart';

import 'save_exported_file.dart';
import 'saved_export_file.dart';
import 'share_exported_file.dart';

enum ExportReportDelivery {
  share,
  saved,
}

class ExportReportFileResult {
  const ExportReportFileResult({
    required this.delivery,
    this.saved,
  });

  final ExportReportDelivery delivery;
  final SavedExportFileResult? saved;

  String get successMessage => switch (delivery) {
        ExportReportDelivery.share => 'تم تجهيز التقرير',
        ExportReportDelivery.saved =>
          saved?.userMessage ?? 'تم حفظ التقرير على الجهاز',
      };
}

/// Tries the native share sheet first; on failure saves directly to device storage.
Future<ExportReportFileResult> exportReportFileWithFallback({
  required List<int> bytes,
  required String fileName,
  required String mimeType,
  String? subject,
  String? text,
  Rect? sharePositionOrigin,
}) async {
  try {
    await shareExportedFile(
      bytes: bytes,
      fileName: fileName,
      mimeType: mimeType,
      subject: subject,
      text: text,
      sharePositionOrigin: sharePositionOrigin,
    );
    return const ExportReportFileResult(delivery: ExportReportDelivery.share);
  } catch (shareError, shareStack) {
    if (kDebugMode) {
      debugPrint('Share export failed, falling back to save: $shareError\n$shareStack');
    }
  }

  final saved = await saveExportedFile(bytes: bytes, fileName: fileName);
  return ExportReportFileResult(
    delivery: ExportReportDelivery.saved,
    saved: saved,
  );
}
