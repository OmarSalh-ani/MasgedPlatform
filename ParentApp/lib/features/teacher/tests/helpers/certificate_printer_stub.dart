import 'dart:convert';
import 'dart:ui';

import 'package:masged_parent_app/core/platform/export_report_file.dart';

Future<String> openCertificateForPrint(
  String html, {
  String? title,
  Rect? sharePositionOrigin,
}) async {
  final result = await exportReportFileWithFallback(
    bytes: utf8.encode(html),
    fileName: 'test_certificate.html',
    mimeType: 'text/html;charset=utf-8',
    subject: title ?? 'شهادة اختبار',
    text: title ?? 'شهادة اختبار الطالب',
    sharePositionOrigin: sharePositionOrigin,
  );
  return result.successMessage;
}
