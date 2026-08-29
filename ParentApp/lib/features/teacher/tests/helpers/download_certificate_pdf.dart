import 'package:masged_parent_app/core/platform/save_exported_file.dart';

Future<String> downloadCertificatePdf({
  required List<int> bytes,
  required String fileName,
}) async {
  final saved = await saveExportedFile(bytes: bytes, fileName: fileName);
  return saved.userMessage;
}
