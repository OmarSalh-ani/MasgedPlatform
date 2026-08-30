enum SavedExportLocation {
  downloads,
  appDocuments,
}

class SavedExportFileResult {
  const SavedExportFileResult({
    required this.path,
    required this.fileName,
    required this.location,
  });

  final String path;
  final String fileName;
  final SavedExportLocation location;

  String get userMessage => switch (location) {
        SavedExportLocation.downloads => 'تم حفظ الملف في مجلد التنزيلات',
        SavedExportLocation.appDocuments =>
          'تم حفظ الملف. افتحه من مدير الملفات داخل مجلد التطبيق',
      };
}
