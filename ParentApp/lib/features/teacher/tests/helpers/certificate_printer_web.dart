// ignore: avoid_web_libraries_in_flutter
import 'dart:html' as html;
import 'dart:ui';

Future<String> openCertificateForPrint(
  String htmlContent, {
  String? title,
  Rect? sharePositionOrigin,
}) async {
  final blob = html.Blob([htmlContent], 'text/html;charset=utf-8');
  final url = html.Url.createObjectUrlFromBlob(blob);
  html.window.open(url, '_blank');
  Future<void>.delayed(const Duration(minutes: 1), () {
    html.Url.revokeObjectUrl(url);
  });
  return 'تم فتح الشهادة للطباعة';
}
