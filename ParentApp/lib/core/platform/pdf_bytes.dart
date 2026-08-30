import '../network/api_exception.dart';

bool isValidPdfBytes(List<int> bytes) {
  if (bytes.length < 5) return false;
  return String.fromCharCodes(bytes.take(5)).startsWith('%PDF-');
}

void assertValidPdfBytes(List<int> bytes) {
  if (isValidPdfBytes(bytes)) return;
  throw ApiException('ملف الشهادة غير صالح من الخادم');
}
