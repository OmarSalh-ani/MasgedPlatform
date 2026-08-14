import 'dart:io';

import 'package:flutter/foundation.dart';
import 'package:path_provider/path_provider.dart';

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

  final downloadsDir = await getDownloadsDirectory();
  if (downloadsDir != null) {
    try {
      final path = await _writeUniqueFile(
        directory: Directory(downloadsDir.path),
        fileName: safeName,
        bytes: bytes,
      );
      return SavedExportFileResult(
        path: path,
        fileName: safeName,
        location: SavedExportLocation.downloads,
      );
    } catch (e, stack) {
      if (kDebugMode) {
        debugPrint('saveExportedFile downloads fallback: $e\n$stack');
      }
    }
  }

  final documentsDir = await getApplicationDocumentsDirectory();
  final exportsDir = Directory('${documentsDir.path}/exports');
  if (!exportsDir.existsSync()) {
    exportsDir.createSync(recursive: true);
  }

  final path = await _writeUniqueFile(
    directory: exportsDir,
    fileName: safeName,
    bytes: bytes,
  );

  return SavedExportFileResult(
    path: path,
    fileName: safeName,
    location: SavedExportLocation.appDocuments,
  );
}

Future<String> _writeUniqueFile({
  required Directory directory,
  required String fileName,
  required List<int> bytes,
}) async {
  var candidate = File('${directory.path}/$fileName');
  if (await candidate.exists()) {
    final dot = fileName.lastIndexOf('.');
    final stem = dot > 0 ? fileName.substring(0, dot) : fileName;
    final ext = dot > 0 ? fileName.substring(dot + 1) : '';
    final stamp = DateTime.now().millisecondsSinceEpoch;
    final suffixed = ext.isEmpty ? '${stem}_$stamp' : '${stem}_$stamp.$ext';
    candidate = File('${directory.path}/$suffixed');
  }

  await candidate.writeAsBytes(bytes, flush: true);
  return candidate.path;
}
