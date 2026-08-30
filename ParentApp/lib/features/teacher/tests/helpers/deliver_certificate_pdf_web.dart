// ignore: deprecated_member_use
import 'dart:html' as html;
import 'dart:typed_data';

Future<String> deliverCertificatePdf({
  required List<int> bytes,
  required String fileName,
}) async {
  final blob = html.Blob(
    [Uint8List.fromList(bytes)],
    'application/pdf',
  );
  final url = html.Url.createObjectUrlFromBlob(blob);
  html.window.open(url, '_blank');
  Future<void>.delayed(const Duration(minutes: 2), () {
    html.Url.revokeObjectUrl(url);
  });
  return 'تم فتح الشهادة في نافذة جديدة';
}
