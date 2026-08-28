import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../data/parent_test_certificate_api.dart';
import '../models/parent_test_certificate_models.dart';

final parentTestCertificateApiProvider =
    Provider((ref) => ParentTestCertificateApi());

final parentTestCertificatesProvider =
    FutureProvider<List<ParentTestCertificateListItem>>((ref) async {
  return ref.read(parentTestCertificateApiProvider).getCertificates();
});
