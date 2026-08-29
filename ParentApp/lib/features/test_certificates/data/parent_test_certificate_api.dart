import 'package:dio/dio.dart';

import '../../../core/network/api_client.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/platform/export_file_name.dart';
import '../models/parent_test_certificate_models.dart';

class ParentTestCertificateApi {
  ParentTestCertificateApi({Dio? dio}) : _dio = dio ?? ApiClient.instance.dio;

  final Dio _dio;

  Future<List<ParentTestCertificateListItem>> getCertificates() async {
    try {
      final response = await _dio.get('/api/parent/test-certificates');
      final list = response.data as List<dynamic>;
      return list
          .map(
            (item) => ParentTestCertificateListItem.fromJson(
              item as Map<String, dynamic>,
            ),
          )
          .toList();
    } on DioException catch (e) {
      if (e.error is ApiException) throw e.error as ApiException;
      throw ApiException('تعذر تحميل شهادات الاختبار');
    }
  }

  Future<({List<int> bytes, String fileName})> getCertificatePdf(
    int testId, {
    String? testPeriod,
  }) async {
    final result = await ApiClient.instance.getBytes(
      '/api/parent/test-certificates/$testId/pdf',
      queryParameters: {
        if (testPeriod != null && testPeriod.isNotEmpty) 'testPeriod': testPeriod,
      },
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
