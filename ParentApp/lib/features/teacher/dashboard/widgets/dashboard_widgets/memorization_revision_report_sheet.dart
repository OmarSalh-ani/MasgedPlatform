import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart' as intl;
import 'package:masged_parent_app/core/platform/export_report_file.dart';
import 'package:masged_parent_app/core/theme/app_colors.dart';
import 'package:masged_parent_app/core/theme/app_fonts.dart';
import 'package:masged_parent_app/shared/widgets/custom_button.dart';
import 'package:masged_parent_app/teacher_core/network/api_exception.dart';

import '../../../reports/models/memorization_revision_report_models.dart';
import '../../../reports/providers/memorization_revision_report_providers.dart';

class MemorizationRevisionReportSheet extends ConsumerStatefulWidget {
  const MemorizationRevisionReportSheet({super.key});

  static Future<void> show(BuildContext context) {
    return showModalBottomSheet<void>(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (context) => Padding(
        padding: EdgeInsets.only(
          bottom: MediaQuery.viewInsetsOf(context).bottom,
        ),
        child: const MemorizationRevisionReportSheet(),
      ),
    );
  }

  @override
  ConsumerState<MemorizationRevisionReportSheet> createState() =>
      _MemorizationRevisionReportSheetState();
}

class _MemorizationRevisionReportSheetState
    extends ConsumerState<MemorizationRevisionReportSheet> {
  late DateTime _fromDate;
  late DateTime _toDate;
  ReportExportFormat _format = ReportExportFormat.pdf;
  bool _isGenerating = false;
  OverlayEntry? _toastEntry;
  Timer? _toastTimer;

  @override
  void initState() {
    super.initState();
    final now = DateTime.now();
    _toDate = DateTime(now.year, now.month, now.day);
    _fromDate = _toDate.subtract(const Duration(days: 7));
  }

  @override
  void dispose() {
    _toastTimer?.cancel();
    _toastEntry?.remove();
    _toastEntry = null;
    super.dispose();
  }

  String _formatDate(DateTime date) =>
      intl.DateFormat('yyyy-MM-dd').format(date);

  Future<void> _pickDate({required bool isFrom}) async {
    final initial = isFrom ? _fromDate : _toDate;
    final picked = await showDatePicker(
      context: context,
      initialDate: initial,
      firstDate: DateTime(2020),
      lastDate: DateTime(2100),
    );
    if (picked == null || !mounted) return;

    setState(() {
      if (isFrom) {
        _fromDate = picked;
        if (_toDate.isBefore(picked)) {
          _toDate = picked;
        }
      } else {
        _toDate = picked;
        if (picked.isBefore(_fromDate)) {
          _fromDate = picked;
        }
      }
    });
  }

  Future<void> _generate() async {
    if (_toDate.isBefore(_fromDate)) {
      _showMessage('تاريخ النهاية يجب أن يكون بعد أو يساوي تاريخ البداية', isError: true);
      return;
    }

    setState(() => _isGenerating = true);
    try {
      final file = await ref
          .read(memorizationRevisionReportApiProvider)
          .exportReport(
            fromDate: _fromDate,
            toDate: _toDate,
            format: _format,
          );

      final size = MediaQuery.sizeOf(context);
      final shareOrigin = Rect.fromCenter(
        center: Offset(size.width / 2, size.height / 2),
        width: 1,
        height: 1,
      );

      final outcome = await exportReportFileWithFallback(
        bytes: file.bytes,
        fileName: file.fileName,
        mimeType: file.format.mimeType,
        subject: 'تقرير الحفظ والمراجعة',
        text: 'تقرير الحفظ والمراجعة',
        sharePositionOrigin: shareOrigin,
      );

      if (mounted) {
        _showMessage(outcome.successMessage, isError: false);
        Navigator.of(context).pop();
      }
    } on ApiException catch (e) {
      _showMessage(e.message, isError: true);
    } catch (e, stack) {
      if (kDebugMode) {
        debugPrint('Memorization report export failed: $e\n$stack');
      }
      final detail = e.toString().trim();
      _showMessage(
        kDebugMode && detail.isNotEmpty
            ? 'تعذر توليد التقرير: $detail'
            : 'تعذر توليد التقرير',
        isError: true,
      );
    } finally {
      if (mounted) setState(() => _isGenerating = false);
    }
  }

  void _showMessage(String message, {bool isError = false}) {
    if (!mounted) return;

    _toastTimer?.cancel();
    _toastEntry?.remove();
    _toastEntry = null;

    // Use the root overlay so the toast paints above the modal bottom sheet.
    final overlay = Overlay.of(context, rootOverlay: true);
    late final OverlayEntry entry;
    entry = OverlayEntry(
      builder: (ctx) {
        final bottom = MediaQuery.viewPaddingOf(ctx).bottom + 24;
        return Positioned(
          left: 16,
          right: 16,
          bottom: bottom,
          child: IgnorePointer(
            child: Material(
              color: Colors.transparent,
              child: Container(
                padding:
                    const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                decoration: BoxDecoration(
                  color: isError ? AppColors.error : AppColors.success,
                  borderRadius: BorderRadius.circular(14),
                  boxShadow: const [
                    BoxShadow(
                      color: Color(0x33000000),
                      blurRadius: 12,
                      offset: Offset(0, 4),
                    ),
                  ],
                ),
                child: Row(
                  children: [
                    Icon(
                      isError
                          ? Icons.error_outline
                          : Icons.check_circle_outline,
                      color: Colors.white,
                      size: 20,
                    ),
                    const SizedBox(width: 10),
                    Expanded(
                      child: Text(
                        message,
                        style: AppFonts.cairo(
                          color: Colors.white,
                          fontSize: 14,
                          fontWeight: FontWeight.w600,
                        ),
                        textDirection: TextDirection.rtl,
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
        );
      },
    );

    _toastEntry = entry;
    overlay.insert(entry);
    _toastTimer = Timer(const Duration(seconds: 4), () {
      entry.remove();
      if (_toastEntry == entry) _toastEntry = null;
    });
  }

  @override
  Widget build(BuildContext context) {
    return SafeArea(
      child: Padding(
        padding: const EdgeInsets.fromLTRB(20, 12, 20, 20),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Center(
              child: Container(
                width: 40,
                height: 4,
                decoration: BoxDecoration(
                  color: AppColors.border,
                  borderRadius: BorderRadius.circular(4),
                ),
              ),
            ),
            const SizedBox(height: 16),
            Text(
              'تقرير الحفظ والمراجعة',
              textAlign: TextAlign.center,
              style: AppFonts.cairo(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: AppColors.textPrimary,
              ),
            ),
            const SizedBox(height: 20),
            _DateField(
              label: 'من تاريخ',
              value: _formatDate(_fromDate),
              onTap: _isGenerating ? null : () => _pickDate(isFrom: true),
            ),
            const SizedBox(height: 12),
            _DateField(
              label: 'الى تاريخ',
              value: _formatDate(_toDate),
              onTap: _isGenerating ? null : () => _pickDate(isFrom: false),
            ),
            const SizedBox(height: 16),
            Text(
              'صيغة التقرير',
              style: AppFonts.cairo(
                fontSize: 13,
                fontWeight: FontWeight.w600,
                color: AppColors.textSecondary,
              ),
            ),
            const SizedBox(height: 8),
            SegmentedButton<ReportExportFormat>(
              segments: const [
                ButtonSegment(
                  value: ReportExportFormat.pdf,
                  label: Text('PDF'),
                  icon: Icon(Icons.picture_as_pdf_outlined, size: 18),
                ),
                ButtonSegment(
                  value: ReportExportFormat.excel,
                  label: Text('Excel'),
                  icon: Icon(Icons.table_chart_outlined, size: 18),
                ),
              ],
              selected: {_format},
              onSelectionChanged: _isGenerating
                  ? null
                  : (value) {
                      if (value.isEmpty) return;
                      setState(() => _format = value.first);
                    },
            ),
            const SizedBox(height: 24),
            CustomButton(
              text: 'توليد التقرير',
              icon: Icons.file_download_outlined,
              isLoading: _isGenerating,
              onPressed: _isGenerating ? null : _generate,
            ),
          ],
        ),
      ),
    );
  }
}

class _DateField extends StatelessWidget {
  const _DateField({
    required this.label,
    required this.value,
    required this.onTap,
  });

  final String label;
  final String value;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(12),
      child: InputDecorator(
        decoration: InputDecoration(
          labelText: label,
          labelStyle: AppFonts.cairo(color: AppColors.textSecondary),
          filled: true,
          fillColor: AppColors.inputFill,
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(12),
            borderSide: const BorderSide(color: AppColors.inputBorder),
          ),
          enabledBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(12),
            borderSide: const BorderSide(color: AppColors.inputBorder),
          ),
          suffixIcon: const Icon(Icons.calendar_today_outlined, size: 18),
        ),
        child: Text(
          value,
          style: AppFonts.cairo(
            fontSize: 14,
            fontWeight: FontWeight.w600,
            color: AppColors.textPrimary,
          ),
        ),
      ),
    );
  }
}
