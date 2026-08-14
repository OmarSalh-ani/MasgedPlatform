import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:masged_parent_app/core/theme/app_colors.dart';
import 'package:masged_parent_app/core/theme/app_fonts.dart';
import 'package:masged_parent_app/features/memorizing_archive/utils/memorizing_archive_display.dart';
import 'package:masged_parent_app/shared/widgets/custom_button.dart';
import 'package:masged_parent_app/teacher_core/network/api_exception.dart';

import '../providers/memorizing_archive_providers.dart';

class NewJuzHizbReviewSheet extends ConsumerStatefulWidget {
  const NewJuzHizbReviewSheet({
    super.key,
    required this.studentId,
    required this.studentName,
  });

  final int studentId;
  final String studentName;

  static Future<bool?> show(
    BuildContext context, {
    required int studentId,
    required String studentName,
  }) {
    return showModalBottomSheet<bool>(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      builder: (context) => Padding(
        padding: EdgeInsets.only(
          bottom: MediaQuery.viewInsetsOf(context).bottom,
        ),
        child: NewJuzHizbReviewSheet(
          studentId: studentId,
          studentName: studentName,
        ),
      ),
    );
  }

  @override
  ConsumerState<NewJuzHizbReviewSheet> createState() =>
      _NewJuzHizbReviewSheetState();
}

class _NewJuzHizbReviewSheetState extends ConsumerState<NewJuzHizbReviewSheet> {
  String _unitType = kArchiveUnitJozz;
  final _numberController = TextEditingController();
  bool _isSaving = false;

  @override
  void dispose() {
    _numberController.dispose();
    super.dispose();
  }

  int? get _maxNumber => _unitType == kArchiveUnitJozz ? 30 : 60;

  String get _numberHint =>
      _unitType == kArchiveUnitJozz ? 'رقم الجزء' : 'رقم الحزب';

  Future<void> _save() async {
    final number = int.tryParse(_numberController.text.trim());
    final max = _maxNumber ?? 30;

    if (number == null || number < 1 || number > max) {
      _showMessage('يرجى إدخال رقم صحيح بين 1 و $max', isError: true);
      return;
    }

    setState(() => _isSaving = true);
    try {
      await ref.read(memorizingArchiveApiProvider).createJuzHizbReview(
            widget.studentId,
            unitType: _unitType,
            number: number,
          );
      if (mounted) Navigator.pop(context, true);
    } on ApiException catch (e) {
      if (mounted) _showMessage(e.message, isError: true);
    } catch (_) {
      if (mounted) _showMessage('تعذر حفظ المراجعة', isError: true);
    } finally {
      if (mounted) setState(() => _isSaving = false);
    }
  }

  void _showMessage(String text, {bool isError = false}) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(text, style: AppFonts.cairo()),
        backgroundColor: isError ? AppColors.error : null,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final sheetHeight = MediaQuery.sizeOf(context).height * 0.5;

    return Directionality(
      textDirection: TextDirection.rtl,
      child: SizedBox(
        height: sheetHeight,
        child: SafeArea(
          top: false,
          child: Padding(
            padding: const EdgeInsets.fromLTRB(20, 12, 20, 24),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Center(
                  child: Container(
                    width: 40,
                    height: 4,
                    decoration: BoxDecoration(
                      color: AppColors.border,
                      borderRadius: BorderRadius.circular(2),
                    ),
                  ),
                ),
                const SizedBox(height: 16),
                Text(
                  'مراجعة جديدة',
                  style: AppFonts.cairo(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                    color: AppColors.textPrimary,
                  ),
                ),
                if (widget.studentName.trim().isNotEmpty) ...[
                  const SizedBox(height: 4),
                  Text(
                    widget.studentName,
                    style: AppFonts.cairo(
                      fontSize: 13,
                      color: AppColors.textSecondary,
                    ),
                  ),
                ],
                const SizedBox(height: 24),
                Text(
                  'اختر النوع',
                  style: AppFonts.cairo(
                    fontSize: 14,
                    fontWeight: FontWeight.w600,
                    color: AppColors.textSecondary,
                  ),
                ),
                const SizedBox(height: 10),
                Row(
                  children: [
                    Expanded(
                      child: _UnitChip(
                        label: kArchiveUnitJozz,
                        selected: _unitType == kArchiveUnitJozz,
                        onTap: () => setState(() {
                          _unitType = kArchiveUnitJozz;
                          _numberController.clear();
                        }),
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: _UnitChip(
                        label: kArchiveUnitHezb,
                        selected: _unitType == kArchiveUnitHezb,
                        onTap: () => setState(() {
                          _unitType = kArchiveUnitHezb;
                          _numberController.clear();
                        }),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 20),
                TextField(
                  controller: _numberController,
                  keyboardType: TextInputType.number,
                  inputFormatters: [FilteringTextInputFormatter.digitsOnly],
                  style: AppFonts.cairo(fontSize: 15),
                  decoration: InputDecoration(
                    hintText: _numberHint,
                    hintStyle: AppFonts.cairo(color: AppColors.textHint),
                    filled: true,
                    fillColor: AppColors.inputFill,
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(14),
                      borderSide: const BorderSide(color: AppColors.inputBorder),
                    ),
                    enabledBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(14),
                      borderSide: const BorderSide(color: AppColors.inputBorder),
                    ),
                    focusedBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(14),
                      borderSide: const BorderSide(
                        color: AppColors.primary,
                        width: 1.5,
                      ),
                    ),
                  ),
                ),
                const Spacer(),
                CustomButton(
                  text: 'حفظ',
                  isLoading: _isSaving,
                  onPressed: _isSaving ? null : _save,
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _UnitChip extends StatelessWidget {
  const _UnitChip({
    required this.label,
    required this.selected,
    required this.onTap,
  });

  final String label;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(12),
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 180),
        padding: const EdgeInsets.symmetric(vertical: 12),
        decoration: BoxDecoration(
          color: selected ? AppColors.primaryLight : Colors.white,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(
            color: selected ? AppColors.primary : AppColors.border,
            width: selected ? 1.5 : 1,
          ),
        ),
        alignment: Alignment.center,
        child: Text(
          label,
          style: AppFonts.cairo(
            fontSize: 15,
            fontWeight: FontWeight.bold,
            color: selected ? AppColors.primary : AppColors.textSecondary,
          ),
        ),
      ),
    );
  }
}
