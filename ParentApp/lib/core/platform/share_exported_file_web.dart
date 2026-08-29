import 'dart:typed_data';
import 'dart:ui';

import 'package:share_plus/share_plus.dart';

import 'export_file_name.dart';

Future<void> shareExportedFile({
  required List<int> bytes,
  required String fileName,
  required String mimeType,
  String? subject,
  String? text,
  Rect? sharePositionOrigin,
}) async {
  final safeName = resolveExportFileName(
    serverFileName: fileName,
    fallbackBaseName: 'export',
    extension: fileName.contains('.') ? fileName.split('.').last : 'bin',
  );

  await SharePlus.instance.share(
    ShareParams(
      files: [
        XFile.fromData(
          Uint8List.fromList(bytes),
          name: safeName,
          mimeType: mimeType,
        ),
      ],
      fileNameOverrides: [safeName],
      subject: subject,
      text: text,
      downloadFallbackEnabled: true,
      sharePositionOrigin: sharePositionOrigin,
    ),
  );
}
