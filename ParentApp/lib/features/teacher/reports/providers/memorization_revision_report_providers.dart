import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../auth/providers/auth_providers.dart';
import '../data/memorization_revision_report_api.dart';

final memorizationRevisionReportApiProvider =
    Provider<MemorizationRevisionReportApi>((ref) {
  return MemorizationRevisionReportApi(ref.watch(apiClientProvider));
});
