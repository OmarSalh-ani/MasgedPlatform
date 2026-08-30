import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:masged_parent_app/core/theme/app_fonts.dart';

import '../../../core/theme/app_colors.dart';
import '../../teacher/tests/helpers/download_certificate_pdf.dart';
import '../models/parent_test_certificate_models.dart';
import '../providers/test_certificates_provider.dart';

class TestCertificatesScreen extends ConsumerStatefulWidget {
  const TestCertificatesScreen({
    super.key,
    this.initialStudentId,
    this.initialTestId,
  });

  final int? initialStudentId;
  final int? initialTestId;

  @override
  ConsumerState<TestCertificatesScreen> createState() =>
      _TestCertificatesScreenState();
}

class _TestCertificatesScreenState extends ConsumerState<TestCertificatesScreen> {
  int? _selectedStudentId;
  int? _printingTestId;
  bool _didAutoOpen = false;

  @override
  void initState() {
    super.initState();
    _selectedStudentId = widget.initialStudentId;
  }

  Future<void> _downloadCertificate(ParentTestCertificateListItem item) async {
    if (_printingTestId != null) return;
    setState(() => _printingTestId = item.testId);
    try {
      final api = ref.read(parentTestCertificateApiProvider);
      final browserUrl = await api.buildPdfBrowserUrl(item.testId);
      final message = await downloadCertificatePdfWithFallback(
        fetchPdf: () => api.getCertificatePdf(item.testId),
        browserPdfUrl: browserUrl,
      );
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(message, style: AppFonts.cairo())),
      );
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(e.toString(), style: AppFonts.cairo()),
          backgroundColor: AppColors.error,
        ),
      );
    } finally {
      if (mounted) setState(() => _printingTestId = null);
    }
  }

  @override
  Widget build(BuildContext context) {
    final certificatesAsync = ref.watch(parentTestCertificatesProvider);

    return Directionality(
      textDirection: TextDirection.rtl,
      child: Scaffold(
        backgroundColor: AppColors.background,
        appBar: AppBar(
          backgroundColor: Colors.white,
          elevation: 0,
          title: Text(
            'شهادات الاختبار',
            style: AppFonts.cairo(fontWeight: FontWeight.bold),
          ),
        ),
        body: certificatesAsync.when(
          loading: () => const Center(child: CircularProgressIndicator()),
          error: (error, _) => Center(
            child: Padding(
              padding: const EdgeInsets.all(24),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Text(
                    error.toString(),
                    textAlign: TextAlign.center,
                    style: AppFonts.cairo(color: AppColors.error),
                  ),
                  const SizedBox(height: 16),
                  FilledButton(
                    onPressed: () =>
                        ref.invalidate(parentTestCertificatesProvider),
                    child: Text('إعادة المحاولة', style: AppFonts.cairo()),
                  ),
                ],
              ),
            ),
          ),
          data: (items) {
            final studentIds = items.map((e) => e.studentId).toSet().toList();
            final filtered = _selectedStudentId == null
                ? items
                : items
                    .where((item) => item.studentId == _selectedStudentId)
                    .toList();

            if (widget.initialTestId != null && !_didAutoOpen) {
              ParentTestCertificateListItem? target;
              for (final item in items) {
                if (item.testId == widget.initialTestId) {
                  target = item;
                  break;
                }
              }
              if (target != null) {
                _didAutoOpen = true;
                WidgetsBinding.instance.addPostFrameCallback((_) {
                  _downloadCertificate(target!);
                });
              }
            }

            return RefreshIndicator(
              onRefresh: () async =>
                  ref.invalidate(parentTestCertificatesProvider),
              child: ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  if (studentIds.length > 1) ...[
                    Wrap(
                      spacing: 8,
                      runSpacing: 8,
                      children: [
                        ChoiceChip(
                          label: Text('الكل', style: AppFonts.cairo()),
                          selected: _selectedStudentId == null,
                          onSelected: (_) =>
                              setState(() => _selectedStudentId = null),
                        ),
                        for (final studentId in studentIds)
                          ChoiceChip(
                            label: Text(
                              items
                                  .firstWhere((e) => e.studentId == studentId)
                                  .studentName,
                              style: AppFonts.cairo(),
                            ),
                            selected: _selectedStudentId == studentId,
                            onSelected: (_) =>
                                setState(() => _selectedStudentId = studentId),
                          ),
                      ],
                    ),
                    const SizedBox(height: 16),
                  ],
                  if (filtered.isEmpty)
                    Padding(
                      padding: const EdgeInsets.only(top: 48),
                      child: Center(
                        child: Text(
                          'لا توجد شهادات اختبار حالياً',
                          style: AppFonts.cairo(
                            fontSize: 16,
                            color: AppColors.textSecondary,
                          ),
                        ),
                      ),
                    )
                  else
                    ...filtered.map(
                      (item) => Padding(
                        padding: const EdgeInsets.only(bottom: 12),
                        child: _CertificateCard(
                          item: item,
                          isPrinting: _printingTestId == item.testId,
                          onOpen: () => _downloadCertificate(item),
                        ),
                      ),
                    ),
                ],
              ),
            );
          },
        ),
      ),
    );
  }
}

class _CertificateCard extends StatelessWidget {
  const _CertificateCard({
    required this.item,
    required this.isPrinting,
    required this.onOpen,
  });

  final ParentTestCertificateListItem item;
  final bool isPrinting;
  final VoidCallback onOpen;

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.white,
      borderRadius: BorderRadius.circular(16),
      child: InkWell(
        borderRadius: BorderRadius.circular(16),
        onTap: isPrinting ? null : onOpen,
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Row(
            children: [
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: AppColors.primary.withValues(alpha: 0.1),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Icon(
                  Icons.workspace_premium_rounded,
                  color: AppColors.primary,
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      item.studentName,
                      style: AppFonts.cairo(
                        fontWeight: FontWeight.bold,
                        fontSize: 15,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      'تاريخ الاختبار: ${item.testDate}',
                      style: AppFonts.cairo(
                        fontSize: 13,
                        color: AppColors.textSecondary,
                      ),
                    ),
                    Text(
                      'التقدير: ${item.grade} — المجموع: ${item.totalScore}',
                      style: AppFonts.cairo(
                        fontSize: 13,
                        color: AppColors.textSecondary,
                      ),
                    ),
                  ],
                ),
              ),
              if (isPrinting)
                const SizedBox(
                  width: 24,
                  height: 24,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              else
                const Icon(Icons.download_rounded, color: AppColors.primary),
            ],
          ),
        ),
      ),
    );
  }
}
