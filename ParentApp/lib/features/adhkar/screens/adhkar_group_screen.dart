import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:masged_parent_app/core/theme/app_colors.dart';
import 'package:masged_parent_app/core/theme/app_fonts.dart';
import 'package:masged_parent_app/shared/router/app_routes.dart';

import '../config/adhkar_groups.dart';
import '../models/adhkar_category.dart';
import '../models/adhkar_session.dart';
import '../providers/adhkar_provider.dart';

class AdhkarGroupScreen extends ConsumerWidget {
  const AdhkarGroupScreen({
    super.key,
    required this.groupId,
  });

  final String groupId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final group = adhkarGroupById(groupId);
    if (group == null) {
      return Directionality(
        textDirection: TextDirection.rtl,
        child: Scaffold(
          appBar: AppBar(
            title: Text(
              'الأذكار',
              style: AppFonts.cairo(fontWeight: FontWeight.bold),
            ),
          ),
          body: Center(
            child: Text(
              'المجموعة غير موجودة',
              style: AppFonts.cairo(color: AppColors.textSecondary),
            ),
          ),
        ),
      );
    }

    final categoriesAsync = ref.watch(adhkarCategoryMapProvider);

    return Directionality(
      textDirection: TextDirection.rtl,
      child: Scaffold(
        backgroundColor: AppColors.background,
        appBar: AppBar(
          backgroundColor: AppColors.surface,
          elevation: 0,
          centerTitle: true,
          title: Text(
            group.title,
            style: AppFonts.cairo(fontWeight: FontWeight.bold),
          ),
        ),
        body: categoriesAsync.when(
          loading: () => const Center(child: CircularProgressIndicator()),
          error: (_, __) => Center(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  'تعذر تحميل الأذكار',
                  style: AppFonts.cairo(color: AppColors.textSecondary),
                ),
                const SizedBox(height: 16),
                TextButton(
                  onPressed: () => ref.invalidate(adhkarDataProvider),
                  child: Text(
                    'إعادة المحاولة',
                    style: AppFonts.cairo(
                      color: AppColors.primary,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ],
            ),
          ),
          data: (categoryMap) {
            final categories = _resolveCategories(categoryMap, group.categoryIds);
            if (categories.isEmpty) {
              return Center(
                child: Text(
                  'لا توجد أذكار في هذه المجموعة',
                  style: AppFonts.cairo(color: AppColors.textSecondary),
                ),
              );
            }

            return ListView.separated(
              padding: const EdgeInsets.all(16),
              itemCount: categories.length,
              separatorBuilder: (_, __) => const SizedBox(height: 10),
              itemBuilder: (context, index) {
                final category = categories[index];
                final session = AdhkarSession.sessionKeyFor(
                  groupId: groupId,
                  categoryId: category.id,
                );
                return _SubCategoryTile(
                  category: category,
                  onTap: () => context.push(
                    AppRoutes.adhkarCategoryPath(
                      category.id,
                      session: session,
                    ),
                  ),
                );
              },
            );
          },
        ),
      ),
    );
  }

  List<AdhkarCategory> _resolveCategories(
    Map<int, AdhkarCategory> categoryMap,
    List<int> categoryIds,
  ) {
    final seen = <int>{};
    final result = <AdhkarCategory>[];

    for (final id in categoryIds) {
      if (seen.contains(id)) continue;
      final category = categoryMap[id];
      if (category != null) {
        seen.add(id);
        result.add(category);
      }
    }

    return result;
  }
}

class _SubCategoryTile extends StatelessWidget {
  const _SubCategoryTile({
    required this.category,
    required this.onTap,
  });

  final AdhkarCategory category;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return Material(
      color: AppColors.surface,
      borderRadius: BorderRadius.circular(16),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(16),
        child: Container(
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(16),
            border: Border.all(color: AppColors.border),
          ),
          child: Row(
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      category.category,
                      style: AppFonts.cairo(
                        fontSize: 15,
                        fontWeight: FontWeight.bold,
                        color: AppColors.textPrimary,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      '${category.items.length} ذكر',
                      style: AppFonts.cairo(
                        fontSize: 12,
                        color: AppColors.textSecondary,
                      ),
                    ),
                  ],
                ),
              ),
              Icon(
                Icons.chevron_left_rounded,
                color: AppColors.primary,
              ),
            ],
          ),
        ),
      ),
    );
  }
}
