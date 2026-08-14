/// Builds a safe ASCII filename for mobile exports (iOS rejects non-ASCII paths).
String resolveExportFileName({
  required String? serverFileName,
  required String fallbackBaseName,
  required String extension,
}) {
  final trimmed = serverFileName?.trim();
  if (trimmed != null && trimmed.isNotEmpty && _isAsciiFileName(trimmed)) {
    return trimmed;
  }

  final ext = extension.replaceAll('.', '');
  final stamp = DateTime.now().toUtc().millisecondsSinceEpoch;
  return '${fallbackBaseName}_$stamp.$ext';
}

bool _isAsciiFileName(String name) =>
    RegExp(r'^[A-Za-z0-9._-]+$').hasMatch(name);
