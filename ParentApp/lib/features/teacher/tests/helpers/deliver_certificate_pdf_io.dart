import 'package:masged_parent_app/core/platform/export_report_file.dart';

Future<String> deliverCertificatePdf({
  required List<int> bytes,
  required String fileName,
}) async {
  final result = await exportReportFileWithFallback(
    bytes: bytes,
    fileName: fileName,
    mimeType: 'application/pdf',
    subject: 'شهادة اختبار',
    text: 'شهادة اختبار الطالب',
  );
  return switch (result.delivery) {
    ExportReportDelivery.share => 'اختر «فتح» أو «حفظ» من قائمة المشاركة',
    ExportReportDelivery.saved =>
      result.saved?.userMessage ?? 'تم حفظ الشهادة على الجهاز',
  };
}
