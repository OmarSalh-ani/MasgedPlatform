import 'package:flutter/material.dart';
import 'package:intl/intl.dart' as intl;
import 'package:masged_parent_app/core/theme/app_colors.dart';
import 'package:masged_parent_app/core/theme/app_fonts.dart';

import '../models/memorizing_archive_item.dart';
import '../utils/memorizing_archive_display.dart';

class MemorizingArchiveCard extends StatelessWidget {
  const MemorizingArchiveCard({
    super.key,
    required this.item,
  });

  final MemorizingArchiveItem item;

  String _formatDate(DateTime date) {
    if (date.millisecondsSinceEpoch == 0) return '';
    return intl.DateFormat('yyyy/MM/dd').format(date);
  }

  Color _typeColor() {
    if (item.isRevision) return AppColors.warning;
    return AppColors.primary;
  }

  Color _doneColor() {
    final normalized = item.isDone.trim();
    if (normalized == 'نعم') return AppColors.success;
    return AppColors.textSecondary;
  }

  @override
  Widget build(BuildContext context) {
    final typeColor = _typeColor();
    final dateLabel = _formatDate(item.createdAt);

    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: AppColors.border),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.03),
            blurRadius: 8,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Row(
              children: [
                Container(
                  padding:
                      const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
                  decoration: BoxDecoration(
                    color: typeColor.withValues(alpha: 0.12),
                    borderRadius: BorderRadius.circular(8),
                    border: Border.all(color: typeColor.withValues(alpha: 0.35)),
                  ),
                  child: Text(
                    item.theType.isEmpty ? '—' : item.theType,
                    style: AppFonts.cairo(
                      fontSize: 12,
                      fontWeight: FontWeight.bold,
                      color: typeColor,
                    ),
                  ),
                ),
                const Spacer(),
                if (dateLabel.isNotEmpty)
                  Container(
                    padding:
                        const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                    decoration: BoxDecoration(
                      color: AppColors.inputFill,
                      borderRadius: BorderRadius.circular(8),
                    ),
                    child: Row(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        const Icon(
                          Icons.calendar_today_outlined,
                          size: 12,
                          color: AppColors.textHint,
                        ),
                        const SizedBox(width: 4),
                        Text(
                          dateLabel,
                          style: AppFonts.cairo(
                            fontSize: 11,
                            color: AppColors.textSecondary,
                          ),
                        ),
                      ],
                    ),
                  ),
              ],
            ),
            const Padding(
              padding: EdgeInsets.symmetric(vertical: 12),
              child: Divider(height: 1, color: AppColors.border),
            ),
            if (item.isJuzHizbReview) ...[
              _detailRow('النوع', kArchiveTypeRevision),
              const SizedBox(height: 8),
              _detailRow('جزء/حزب', item.surahName),
              const SizedBox(height: 8),
              _detailRow('رقم الجزء/الحزب', item.unitNumberLabel),
            ] else if (item.isMemorizing) ...[
              _detailRow('اسم السورة', item.surahName),
              const SizedBox(height: 8),
              _detailRow('سورة البداية', item.testFrom),
              const SizedBox(height: 8),
              _detailRow('سورة النهاية', item.testTo),
              const SizedBox(height: 8),
              _doneRow('تم الحفظ', item.isDone, _doneColor()),
            ] else ...[
              _detailRow('سورة البداية', item.testFrom),
              const SizedBox(height: 8),
              _detailRow('سورة النهاية', item.testTo),
              const SizedBox(height: 8),
              _doneRow('تم المراجعة', item.isDone, _doneColor()),
            ],
            if (item.notes != null && item.notes!.trim().isNotEmpty) ...[
              const SizedBox(height: 8),
              _detailRow('ملاحظات', item.notes!.trim()),
            ],
          ],
        ),
      ),
    );
  }

  Widget _detailRow(String label, String value) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        SizedBox(
          width: 110,
          child: Text(
            label,
            style: AppFonts.cairo(
              fontSize: 12,
              color: AppColors.textSecondary,
            ),
          ),
        ),
        Expanded(
          child: Text(
            value.isEmpty ? '—' : value,
            style: AppFonts.cairo(
              fontSize: 13,
              fontWeight: FontWeight.w600,
              color: AppColors.textPrimary,
            ),
          ),
        ),
      ],
    );
  }

  Widget _doneRow(String label, String value, Color color) {
    return Row(
      children: [
        SizedBox(
          width: 110,
          child: Text(
            label,
            style: AppFonts.cairo(
              fontSize: 12,
              color: AppColors.textSecondary,
            ),
          ),
        ),
        Text(
          value.isEmpty ? '—' : value,
          style: AppFonts.cairo(
            fontSize: 13,
            fontWeight: FontWeight.bold,
            color: color,
          ),
        ),
      ],
    );
  }
}
