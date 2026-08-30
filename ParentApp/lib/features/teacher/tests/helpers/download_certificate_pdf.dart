import 'package:flutter/foundation.dart';
import 'package:masged_parent_app/core/platform/pdf_bytes.dart';
import 'package:url_launcher/url_launcher.dart';

import 'deliver_certificate_pdf.dart';

Future<String> downloadCertificatePdf({
  required List<int> bytes,
  required String fileName,
}) async {
  assertValidPdfBytes(bytes);
  return deliverCertificatePdf(bytes: bytes, fileName: fileName);
}

Future<bool> openCertificatePdfInBrowser(String url) async {
  final uri = Uri.parse(url);
  if (!await canLaunchUrl(uri)) return false;
  return launchUrl(uri, mode: LaunchMode.externalApplication);
}

/// Tries in-app download first; on failure opens the PDF URL in the browser.
Future<String> downloadCertificatePdfWithFallback({
  required Future<({List<int> bytes, String fileName})> Function() fetchPdf,
  required String browserPdfUrl,
}) async {
  Object? error;
  StackTrace? stackTrace;

  try {
    final file = await fetchPdf();
    return await downloadCertificatePdf(
      bytes: file.bytes,
      fileName: file.fileName,
    );
  } catch (e, stack) {
    error = e;
    stackTrace = stack;
    if (kDebugMode) {
      debugPrint('Certificate download failed, trying browser: $e\n$stack');
    }
  }

  final opened = await openCertificatePdfInBrowser(browserPdfUrl);
  if (opened) {
    return 'تم فتح الشهادة في المتصفح للتحميل';
  }

  Error.throwWithStackTrace(error!, stackTrace ?? StackTrace.empty);
}
