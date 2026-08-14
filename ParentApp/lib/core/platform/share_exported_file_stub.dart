import 'dart:ui';

Future<void> shareExportedFile({
  required List<int> bytes,
  required String fileName,
  required String mimeType,
  String? subject,
  String? text,
  Rect? sharePositionOrigin,
}) {
  throw UnsupportedError('shareExportedFile is not supported on this platform');
}
