import 'dart:convert';

import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../models/adhkar_category.dart';

final adhkarDataProvider = FutureProvider<List<AdhkarCategory>>((ref) async {
  final json = await rootBundle.loadString('assets/json/adhkar.json');
  final decoded = jsonDecode(json) as List<dynamic>;
  return decoded
      .map((item) => AdhkarCategory.fromJson(item as Map<String, dynamic>))
      .toList();
});

final adhkarCategoryMapProvider =
    FutureProvider<Map<int, AdhkarCategory>>((ref) async {
  final categories = await ref.watch(adhkarDataProvider.future);
  return {for (final category in categories) category.id: category};
});
