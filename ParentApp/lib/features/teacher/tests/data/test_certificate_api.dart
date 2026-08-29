import 'package:masged_parent_app/core/platform/export_file_name.dart';
import 'package:masged_parent_app/teacher_core/network/api_client.dart';
import '../models/test_certificate_models.dart';

class TestCertificateApi {
  TestCertificateApi(this._client);

  final TeacherApiClient _client;

  Future<TestCertificate> getCertificate(
    int testId, {
    String testPeriod = 'الفصل الأول',
  }) {
    return _client.get<TestCertificate>(
      '/api/test-certificates/$testId',
      queryParameters: {'testPeriod': testPeriod},
      parseData: (json) =>
          TestCertificate.fromJson(json as Map<String, dynamic>),
    );
  }

  Future<({List<int> bytes, String fileName})> getCertificatePdf(
    int testId, {
    String testPeriod = 'الفصل الأول',
  }) async {
    final result = await _client.getBytes(
      '/api/test-certificates/$testId/pdf',
      queryParameters: {'testPeriod': testPeriod},
    );

    return (
      bytes: result.bytes,
      fileName: resolveExportFileName(
        serverFileName: result.fileName,
        fallbackBaseName: 'test_certificate',
        extension: 'pdf',
      ),
    );
  }
}
