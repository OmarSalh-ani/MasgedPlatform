import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:masged_parent_app/core/theme/app_fonts.dart';

import '../../../core/theme/app_colors.dart';
import '../../children/models/student_plan_models.dart';
import '../models/memorizing_archive_item.dart';
import '../utils/memorizing_archive_display.dart';
import '../widgets/memorizing_archive_card.dart';
import '../widgets/memorizing_archive_filters.dart';

typedef MemorizingArchiveLoader = Future<PagedResult<MemorizingArchiveItem>>
    Function(MemorizingArchiveQuery query);

class MemorizingArchiveQuery {
  const MemorizingArchiveQuery({
    required this.page,
    this.pageSize = 20,
    this.surahSearch = '',
    this.typeFilter = 'الكل',
  });

  final int page;
  final int pageSize;
  final String surahSearch;
  final String typeFilter;
}

class MemorizingArchiveScreen extends ConsumerStatefulWidget {
  const MemorizingArchiveScreen({
    super.key,
    required this.studentName,
    required this.loader,
  });

  final String studentName;
  final MemorizingArchiveLoader loader;

  @override
  ConsumerState<MemorizingArchiveScreen> createState() =>
      _MemorizingArchiveScreenState();
}

class _MemorizingArchiveScreenState extends ConsumerState<MemorizingArchiveScreen> {
  static const _pageSize = 20;

  final _searchController = TextEditingController();
  Timer? _debounce;
  int _page = 1;
  String _surahSearch = '';
  String _typeFilter = 'الكل';
  Future<PagedResult<MemorizingArchiveItem>>? _future;

  @override
  void initState() {
    super.initState();
    _load();
    _searchController.addListener(_onSearchChanged);
  }

  @override
  void dispose() {
    _debounce?.cancel();
    _searchController.dispose();
    super.dispose();
  }

  MemorizingArchiveQuery get _query => MemorizingArchiveQuery(
        page: _page,
        pageSize: _pageSize,
        surahSearch: _surahSearch,
        typeFilter: _typeFilter,
      );

  void _load() {
    setState(() {
      _future = widget.loader(_query);
    });
  }

  void _onSearchChanged() {
    _debounce?.cancel();
    _debounce = Timer(const Duration(milliseconds: 400), () {
      final term = _searchController.text.trim();
      if (term == _surahSearch) return;
      setState(() {
        _surahSearch = term;
        _page = 1;
      });
      _load();
    });
  }

  void _onTypeFilterChanged(String type) {
    if (type == _typeFilter) return;
    setState(() {
      _typeFilter = type;
      _page = 1;
    });
    _load();
  }

  void _clearSearch() {
    _searchController.clear();
    setState(() {
      _surahSearch = '';
      _page = 1;
    });
    _load();
  }

  void _goToPage(int page) {
    if (page == _page) return;
    setState(() => _page = page);
    _load();
  }

  String _emptyMessage() {
    if (_surahSearch.isNotEmpty) {
      return 'لا توجد سجلات مطابقة للبحث';
    }
    if (_typeFilter == kArchiveTypeMemorizing) {
      return 'لا توجد سجلات حفظ';
    }
    if (_typeFilter == kArchiveTypeRevision) {
      return 'لا توجد سجلات مراجعة';
    }
    return 'لا توجد سجلات في أرشيف الحفظ';
  }

  @override
  Widget build(BuildContext context) {
    return Directionality(
      textDirection: TextDirection.rtl,
      child: Scaffold(
        backgroundColor: AppColors.background,
        appBar: AppBar(
          backgroundColor: Colors.white,
          elevation: 0,
          title: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                'أرشيف الحفظ',
                style: AppFonts.cairo(fontWeight: FontWeight.bold),
              ),
              if (widget.studentName.trim().isNotEmpty)
                Text(
                  widget.studentName,
                  style: AppFonts.cairo(
                    fontSize: 12,
                    color: AppColors.textSecondary,
                  ),
                ),
            ],
          ),
        ),
        body: Column(
          children: [
            MemorizingArchiveFilters(
              selectedType: _typeFilter,
              onTypeChanged: _onTypeFilterChanged,
              searchController: _searchController,
              onSearchClear: _clearSearch,
              hasSearch: _surahSearch.isNotEmpty,
            ),
            Expanded(
              child: FutureBuilder<PagedResult<MemorizingArchiveItem>>(
                future: _future,
                builder: (context, snapshot) {
                  if (snapshot.connectionState == ConnectionState.waiting &&
                      !snapshot.hasData) {
                    return const Center(child: CircularProgressIndicator());
                  }
                  if (snapshot.hasError) {
                    return _buildError(snapshot.error.toString());
                  }
                  final paged = snapshot.data;
                  if (paged == null) {
                    return const Center(child: CircularProgressIndicator());
                  }
                  if (paged.items.isEmpty) {
                    return _buildEmpty();
                  }
                  return RefreshIndicator(
                    onRefresh: () async => _load(),
                    child: ListView(
                      padding: const EdgeInsets.fromLTRB(16, 8, 16, 24),
                      children: [
                        for (var i = 0; i < paged.items.length; i++) ...[
                          if (i > 0) const SizedBox(height: 12),
                          MemorizingArchiveCard(item: paged.items[i]),
                        ],
                        _buildPagination(paged),
                      ],
                    ),
                  );
                },
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildPagination(PagedResult<MemorizingArchiveItem> paged) {
    if (paged.totalPages <= 1) return const SizedBox(height: 16);

    return Padding(
      padding: const EdgeInsets.only(top: 16),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          TextButton(
            onPressed: paged.page > 1 ? () => _goToPage(paged.page - 1) : null,
            child: Text('السابق', style: AppFonts.cairo()),
          ),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 12),
            child: Text(
              'صفحة ${paged.page} من ${paged.totalPages}',
              style: AppFonts.cairo(
                fontSize: 13,
                color: AppColors.textSecondary,
              ),
            ),
          ),
          TextButton(
            onPressed: paged.page < paged.totalPages
                ? () => _goToPage(paged.page + 1)
                : null,
            child: Text('التالي', style: AppFonts.cairo()),
          ),
        ],
      ),
    );
  }

  Widget _buildEmpty() {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(32),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.menu_book_outlined,
              size: 48,
              color: AppColors.textHint.withValues(alpha: 0.6),
            ),
            const SizedBox(height: 12),
            Text(
              _emptyMessage(),
              textAlign: TextAlign.center,
              style: AppFonts.cairo(
                fontSize: 15,
                color: AppColors.textSecondary,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildError(String message) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(
              message,
              textAlign: TextAlign.center,
              style: AppFonts.cairo(color: AppColors.error),
            ),
            const SizedBox(height: 16),
            TextButton(
              onPressed: _load,
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
    );
  }
}
