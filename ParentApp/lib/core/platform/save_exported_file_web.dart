// ignore: deprecated_member_use
import 'dart:html' as html;
import 'dart:typed_data';

import 'export_file_name.dart';
import 'saved_export_file.dart';

Future<SavedExportFileResult> saveExportedFile({
  required List<int> bytes,
  required String fileName,
}) async {
  final safeName = resolveExportFileName(
    serverFileName: fileName,
    fallbackBaseName: 'export',
    extension: fileName.contains('.') ? fileName.split('.').last : 'bin',
  );

  final blob = html.Blob([Uint8List.fromList(bytes)]);
  final url = html.Url.createObjectUrlFromBlob(blob);
  html.AnchorElement(href: url)
    ..download = safeName
    ..click();
  html.Url.revokeObjectUrl(url);

  return SavedExportFileResult(
    path: safeName,
    fileName: safeName,
    location: SavedExportLocation.downloads,
  );
}
