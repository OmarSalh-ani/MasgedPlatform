import 'package:masged_parent_app/teacher_core/network/api_client.dart';

import '../../../children/models/student_plan_models.dart';
import '../../../memorizing_archive/models/memorizing_archive_item.dart';

class MemorizingArchiveApi {
  MemorizingArchiveApi(this._client);

  final TeacherApiClient _client;

  Future<PagedResult<MemorizingArchiveItem>> getArchive(
    int studentId, {
    required int page,
    int pageSize = 20,
    String? surahSearch,
    String? typeFilter,
  }) {
    final trimmedSearch = surahSearch?.trim();
    final trimmedFilter = typeFilter?.trim();
    return _client.get<PagedResult<MemorizingArchiveItem>>(
      '/api/memorizing-archive/$studentId',
      queryParameters: {
        'page': page,
        'pageSize': pageSize,
        if (trimmedSearch != null && trimmedSearch.isNotEmpty)
          'surahSearch': trimmedSearch,
        if (trimmedFilter != null &&
            trimmedFilter.isNotEmpty &&
            trimmedFilter != 'الكل')
          'typeFilter': trimmedFilter,
      },
      parseData: (json) => PagedResult.fromJson(
        Map<String, dynamic>.from(json as Map),
        MemorizingArchiveItem.fromJson,
      ),
    );
  }

  Future<MemorizingArchiveItem> createJuzHizbReview(
    int studentId, {
    required String unitType,
    required int number,
  }) {
    return _client.post<MemorizingArchiveItem>(
      '/api/memorizing-archive/$studentId/review',
      body: {
        'unitType': unitType,
        'number': number,
      },
      parseData: (json) => MemorizingArchiveItem.fromJson(
        Map<String, dynamic>.from(json as Map),
      ),
    );
  }
}
