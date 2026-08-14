import 'dart:async';

import 'package:flutter/material.dart';
import 'package:masged_parent_app/core/theme/app_fonts.dart';
import 'package:flutter_animate/flutter_animate.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart' as intl;
import 'package:masged_parent_app/core/services/prayer_service.dart';
import 'package:masged_parent_app/core/theme/app_colors.dart';
import 'package:masged_parent_app/features/prayer/models/prayer_times_data.dart';
import 'package:masged_parent_app/features/prayer/widgets/prayer_location_error.dart';
import 'package:masged_parent_app/shared/router/app_routes.dart';

class NextPrayerCard extends StatefulWidget {
  const NextPrayerCard({super.key});

  @override
  State<NextPrayerCard> createState() => _NextPrayerCardState();
}

class _NextPrayerCardState extends State<NextPrayerCard> {
  Future<PrayerTimesData>? _prayerFuture;
  Timer? _ticker;
  DateTime? _loadedDate;

  @override
  void initState() {
    super.initState();
    _loadPrayerTimes();
    _ticker = Timer.periodic(const Duration(seconds: 1), (_) {
      if (!mounted) return;
      final today = DateTime.now();
      final todayDate = DateTime(today.year, today.month, today.day);
      if (_loadedDate != null && !_isSameDay(_loadedDate!, todayDate)) {
        _loadPrayerTimes();
      } else {
        setState(() {});
      }
    });
  }

  @override
  void dispose() {
    _ticker?.cancel();
    super.dispose();
  }

  static bool _isSameDay(DateTime a, DateTime b) =>
      a.year == b.year && a.month == b.month && a.day == b.day;

  void _loadPrayerTimes() {
    final now = DateTime.now();
    _loadedDate = DateTime(now.year, now.month, now.day);
    setState(() {
      _prayerFuture = PrayerService().getPrayerTimes();
    });
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<PrayerTimesData>(
      future: _prayerFuture,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const _NextPrayerSkeleton();
        }

        if (snapshot.hasError) {
          if (snapshot.error is PrayerLocationException) {
            return Padding(
              padding: EdgeInsets.only(top: 12.h, bottom: 16.h),
              child: Container(
                width: double.infinity,
                padding: EdgeInsets.symmetric(vertical: 8.h),
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(20.r),
                  border: Border.all(color: AppColors.border),
                ),
                child: PrayerLocationError(
                  compact: true,
                  onRetry: _loadPrayerTimes,
                ),
              ),
            );
          }
          return const SizedBox.shrink();
        }

        final times = snapshot.data;
        if (times == null) {
          return const SizedBox.shrink();
        }

        final next = PrayerService.nextPrayer(times);
        final nextTime = PrayerService.nextPrayerDateTime(times);
        final isTomorrow = next == PrayerName.fajrAfter;

        return _NextPrayerContent(
          times: times,
          next: next,
          nextTime: nextTime,
          isTomorrow: isTomorrow,
          onTap: () => context.push(AppRoutes.prayerTimes),
        );
      },
    );
  }
}

class _NextPrayerContent extends StatelessWidget {
  const _NextPrayerContent({
    required this.times,
    required this.next,
    required this.nextTime,
    required this.isTomorrow,
    required this.onTap,
  });

  final PrayerTimesData times;
  final PrayerName next;
  final DateTime nextTime;
  final bool isTomorrow;
  final VoidCallback onTap;

  static String _prayerName(PrayerName prayer) {
    switch (prayer) {
      case PrayerName.fajr:
      case PrayerName.fajrAfter:
        return 'الفجر';
      case PrayerName.sunrise:
        return 'الشروق';
      case PrayerName.dhuhr:
        return 'الظهر';
      case PrayerName.asr:
        return 'العصر';
      case PrayerName.maghrib:
        return 'المغرب';
      case PrayerName.isha:
      case PrayerName.ishaBefore:
        return 'العشاء';
    }
  }

  static IconData _prayerIcon(PrayerName prayer) {
    switch (prayer) {
      case PrayerName.fajr:
      case PrayerName.fajrAfter:
        return Icons.nights_stay_rounded;
      case PrayerName.sunrise:
        return Icons.wb_twilight_rounded;
      case PrayerName.dhuhr:
        return Icons.wb_sunny_rounded;
      case PrayerName.asr:
        return Icons.cloud_rounded;
      case PrayerName.maghrib:
        return Icons.nightlight_round;
      case PrayerName.isha:
      case PrayerName.ishaBefore:
        return Icons.dark_mode_rounded;
    }
  }

  double _progressUntilNext() {
    final now = DateTime.now();
    final current = times.currentPrayer();
    final start = times.timeForPrayer(current);
    final end = nextTime;
    if (!end.isAfter(start)) return 1;
    final total = end.difference(start).inSeconds;
    if (total <= 0) return 1;
    final elapsed = now.difference(start).inSeconds.clamp(0, total);
    return (elapsed / total).clamp(0.0, 1.0);
  }

  @override
  Widget build(BuildContext context) {
    final now = DateTime.now();
    final diff = nextTime.difference(now);
    final isNow = !isTomorrow && diff.inSeconds <= 0;
    final prayerName = _prayerName(next);
    final progress = isNow ? 1.0 : _progressUntilNext();

    final totalSeconds = diff.inSeconds.clamp(0, 86400);
    final hours = totalSeconds ~/ 3600;
    final minutes = (totalSeconds % 3600) ~/ 60;
    final seconds = totalSeconds % 60;

    final accent = isNow ? AppColors.gold : AppColors.primary;
    final gradient = isNow
        ? const LinearGradient(
            begin: Alignment.topLeft,
            end: Alignment.bottomRight,
            colors: [Color(0xFFE8C97A), Color(0xFFC9A96E), Color(0xFFAD8850)],
          )
        : const LinearGradient(
            begin: Alignment.topLeft,
            end: Alignment.bottomRight,
            colors: [Color(0xFF5BBCAE), Color(0xFF4A9B8F), Color(0xFF357A6F)],
          );

    return Padding(
      padding: EdgeInsets.only(top: 12.h, bottom: 16.h),
      child: LayoutBuilder(
        builder: (context, constraints) {
          final compact = constraints.maxWidth < 340;

          Widget card = Material(
            color: Colors.transparent,
            child: InkWell(
              onTap: onTap,
              borderRadius: BorderRadius.circular(20.r),
              child: Ink(
                decoration: BoxDecoration(
                  gradient: gradient,
                  borderRadius: BorderRadius.circular(20.r),
                  boxShadow: [
                    BoxShadow(
                      color: accent.withValues(alpha: 0.3),
                      blurRadius: 16.r,
                      offset: Offset(0, 8.h),
                    ),
                  ],
                ),
                child: Stack(
                  clipBehavior: Clip.none,
                  children: [
                    Positioned(
                      top: -20.h,
                      left: -14.w,
                      child: _DecorCircle(size: 80.r, opacity: 0.12),
                    ),
                    Positioned(
                      bottom: -28.h,
                      right: -8.w,
                      child: _DecorCircle(size: 70.r, opacity: 0.1),
                    ),
                    Padding(
                      padding: EdgeInsets.fromLTRB(14.w, 12.h, 14.w, 14.h),
                      child: compact
                          ? _CompactLayout(
                              isNow: isNow,
                              isTomorrow: isTomorrow,
                              prayerName: prayerName,
                              next: next,
                              nextTime: nextTime,
                              hours: hours,
                              minutes: minutes,
                              seconds: seconds,
                              progress: progress,
                              showSeconds: hours == 0,
                            )
                          : _ExpandedLayout(
                              isNow: isNow,
                              isTomorrow: isTomorrow,
                              prayerName: prayerName,
                              next: next,
                              nextTime: nextTime,
                              hours: hours,
                              minutes: minutes,
                              seconds: seconds,
                              progress: progress,
                              showSeconds: hours == 0,
                            ),
                    ),
                  ],
                ),
              ),
            ),
          );

          card = card
              .animate()
              .fadeIn(duration: 400.ms, curve: Curves.easeOut)
              .slideY(
                begin: 0.06,
                end: 0,
                duration: 400.ms,
                curve: Curves.easeOutCubic,
              );

          if (isNow) {
            card = card
                .animate(onPlay: (c) => c.repeat(reverse: true))
                .shimmer(duration: 2.seconds, color: Colors.white24);
          }

          return card;
        },
      ),
    );
  }
}

class _CompactLayout extends StatelessWidget {
  const _CompactLayout({
    required this.isNow,
    required this.isTomorrow,
    required this.prayerName,
    required this.next,
    required this.nextTime,
    required this.hours,
    required this.minutes,
    required this.seconds,
    required this.progress,
    required this.showSeconds,
  });

  final bool isNow;
  final bool isTomorrow;
  final String prayerName;
  final PrayerName next;
  final DateTime nextTime;
  final int hours;
  final int minutes;
  final int seconds;
  final double progress;
  final bool showSeconds;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        _HeaderRow(
          isNow: isNow,
          isTomorrow: isTomorrow,
          next: next,
          compact: true,
        ),
        SizedBox(height: 8.h),
        FittedBox(
          fit: BoxFit.scaleDown,
          child: Text(
            isNow ? 'حان وقت الصلاة' : prayerName,
            textAlign: TextAlign.center,
            style: AppFonts.cairo(
              color: Colors.white,
              fontSize: 22.sp,
              fontWeight: FontWeight.bold,
              height: 1.1,
            ),
          ),
        ),
        if (isNow) ...[
          SizedBox(height: 2.h),
          Text(
            prayerName,
            textAlign: TextAlign.center,
            style: AppFonts.cairo(
              color: Colors.white.withValues(alpha: 0.9),
              fontSize: 13.sp,
              fontWeight: FontWeight.w600,
            ),
          ),
        ],
        SizedBox(height: 10.h),
        if (!isNow)
          _CountdownRow(
            hours: hours,
            minutes: minutes,
            seconds: seconds,
            showSeconds: showSeconds,
            centered: true,
          ),
        SizedBox(height: 10.h),
        _FooterRow(nextTime: nextTime, progress: progress, isNow: isNow),
      ],
    );
  }
}

class _ExpandedLayout extends StatelessWidget {
  const _ExpandedLayout({
    required this.isNow,
    required this.isTomorrow,
    required this.prayerName,
    required this.next,
    required this.nextTime,
    required this.hours,
    required this.minutes,
    required this.seconds,
    required this.progress,
    required this.showSeconds,
  });

  final bool isNow;
  final bool isTomorrow;
  final String prayerName;
  final PrayerName next;
  final DateTime nextTime;
  final int hours;
  final int minutes;
  final int seconds;
  final double progress;
  final bool showSeconds;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        _HeaderRow(
          isNow: isNow,
          isTomorrow: isTomorrow,
          next: next,
          compact: false,
        ),
        SizedBox(height: 10.h),
        Row(
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            Expanded(
              flex: 5,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    isNow ? 'حان وقت الصلاة' : 'الصلاة القادمة',
                    style: AppFonts.cairo(
                      color: Colors.white.withValues(alpha: 0.85),
                      fontSize: 11.sp,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                  SizedBox(height: 2.h),
                  Text(
                    prayerName,
                    style: AppFonts.cairo(
                      color: Colors.white,
                      fontSize: 24.sp,
                      fontWeight: FontWeight.bold,
                      height: 1.05,
                    ),
                  ),
                  if (isTomorrow) ...[
                    SizedBox(height: 4.h),
                    Text(
                      'غداً',
                      style: AppFonts.cairo(
                        color: Colors.white.withValues(alpha: 0.75),
                        fontSize: 11.sp,
                      ),
                    ),
                  ],
                ],
              ),
            ),
            if (!isNow) ...[
              SizedBox(width: 8.w),
              Expanded(
                flex: 6,
                child: _CountdownRow(
                  hours: hours,
                  minutes: minutes,
                  seconds: seconds,
                  showSeconds: showSeconds,
                  centered: false,
                ),
              ),
            ] else
              Container(
                padding: EdgeInsets.all(10.r),
                decoration: BoxDecoration(
                  color: Colors.white.withValues(alpha: 0.2),
                  shape: BoxShape.circle,
                ),
                child: Icon(
                  Icons.notifications_active_rounded,
                  color: Colors.white,
                  size: 28.sp,
                ),
              ),
          ],
        ),
        SizedBox(height: 10.h),
        _FooterRow(nextTime: nextTime, progress: progress, isNow: isNow),
      ],
    );
  }
}

class _HeaderRow extends StatelessWidget {
  const _HeaderRow({
    required this.isNow,
    required this.isTomorrow,
    required this.next,
    required this.compact,
  });

  final bool isNow;
  final bool isTomorrow;
  final PrayerName next;
  final bool compact;

  @override
  Widget build(BuildContext context) {
    final label = isNow
        ? 'الآن'
        : (isTomorrow ? 'صلاة الغد' : 'الصلاة القادمة');

    return Row(
      children: [
        Container(
          padding: EdgeInsets.all(compact ? 7.r : 8.r),
          decoration: BoxDecoration(
            color: Colors.white.withValues(alpha: 0.2),
            borderRadius: BorderRadius.circular(12.r),
          ),
          child: Icon(
            _NextPrayerContent._prayerIcon(next),
            color: Colors.white,
            size: compact ? 18.sp : 20.sp,
          ),
        ),
        SizedBox(width: 8.w),
        Expanded(
          child: Text(
            label,
            style: AppFonts.cairo(
              color: Colors.white,
              fontSize: compact ? 11.sp : 12.sp,
              fontWeight: FontWeight.w600,
            ),
          ),
        ),
        Icon(
          Icons.chevron_left_rounded,
          color: Colors.white.withValues(alpha: 0.7),
          size: 18.sp,
        ),
      ],
    );
  }
}

class _CountdownRow extends StatelessWidget {
  const _CountdownRow({
    required this.hours,
    required this.minutes,
    required this.seconds,
    required this.showSeconds,
    required this.centered,
  });

  final int hours;
  final int minutes;
  final int seconds;
  final bool showSeconds;
  final bool centered;

  @override
  Widget build(BuildContext context) {
    final units = <_CountdownUnit>[
      if (hours > 0)
        _CountdownUnit(value: hours, label: 'ساعة'),
      _CountdownUnit(value: minutes, label: 'دقيقة'),
      if (showSeconds)
        _CountdownUnit(value: seconds, label: 'ثانية'),
    ];

    final children = <Widget>[
      for (var i = 0; i < units.length; i++) ...[
        if (i > 0) _CountdownSeparator(),
        _CountdownBox(unit: units[i]),
      ],
    ];

    if (centered) {
      return Wrap(
        alignment: WrapAlignment.center,
        crossAxisAlignment: WrapCrossAlignment.center,
        spacing: 2.w,
        runSpacing: 8.h,
        children: children,
      );
    }

    return Align(
      alignment: Alignment.centerRight,
      child: Wrap(
        alignment: WrapAlignment.end,
        crossAxisAlignment: WrapCrossAlignment.center,
        spacing: 2.w,
        children: children,
      ),
    );
  }
}

class _CountdownUnit {
  const _CountdownUnit({required this.value, required this.label});
  final int value;
  final String label;
}

class _CountdownBox extends StatelessWidget {
  const _CountdownBox({required this.unit});

  final _CountdownUnit unit;

  @override
  Widget build(BuildContext context) {
    return Container(
      constraints: BoxConstraints(minWidth: 44.w),
      padding: EdgeInsets.symmetric(horizontal: 7.w, vertical: 6.h),
      decoration: BoxDecoration(
        color: Colors.white.withValues(alpha: 0.18),
        borderRadius: BorderRadius.circular(10.r),
        border: Border.all(color: Colors.white.withValues(alpha: 0.25)),
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            unit.value.toString().padLeft(2, '0'),
            style: AppFonts.cairo(
              color: Colors.white,
              fontSize: 17.sp,
              fontWeight: FontWeight.bold,
              height: 1,
            ),
          ),
          SizedBox(height: 1.h),
          Text(
            unit.label,
            style: AppFonts.cairo(
              color: Colors.white.withValues(alpha: 0.8),
              fontSize: 9.sp,
              fontWeight: FontWeight.w500,
            ),
          ),
        ],
      ),
    );
  }
}

class _CountdownSeparator extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.symmetric(horizontal: 2.w),
      child: Text(
        ':',
        style: AppFonts.cairo(
          color: Colors.white.withValues(alpha: 0.6),
          fontSize: 16.sp,
          fontWeight: FontWeight.bold,
        ),
      ),
    );
  }
}

class _FooterRow extends StatelessWidget {
  const _FooterRow({
    required this.nextTime,
    required this.progress,
    required this.isNow,
  });

  final DateTime nextTime;
  final double progress;
  final bool isNow;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        ClipRRect(
          borderRadius: BorderRadius.circular(6.r),
          child: LinearProgressIndicator(
            value: progress,
            minHeight: 4.h,
            backgroundColor: Colors.white.withValues(alpha: 0.2),
            valueColor: const AlwaysStoppedAnimation<Color>(Colors.white),
          ),
        ),
        SizedBox(height: 8.h),
        Row(
          children: [
            Icon(
              Icons.schedule_rounded,
              color: Colors.white.withValues(alpha: 0.85),
              size: 14.sp,
            ),
            SizedBox(width: 4.w),
            Text(
              intl.DateFormat('h:mm a', 'ar').format(nextTime),
              style: AppFonts.cairo(
                color: Colors.white,
                fontSize: 12.sp,
                fontWeight: FontWeight.w600,
              ),
            ),
            const Spacer(),
            Text(
              isNow ? 'اضغط لأوقات الصلاة' : 'عرض كل الأوقات',
              style: AppFonts.cairo(
                color: Colors.white.withValues(alpha: 0.75),
                fontSize: 10.sp,
              ),
            ),
          ],
        ),
      ],
    );
  }
}

class _DecorCircle extends StatelessWidget {
  const _DecorCircle({required this.size, required this.opacity});

  final double size;
  final double opacity;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: size,
      height: size,
      decoration: BoxDecoration(
        shape: BoxShape.circle,
        color: Colors.white.withValues(alpha: opacity),
      ),
    );
  }
}

class _NextPrayerSkeleton extends StatelessWidget {
  const _NextPrayerSkeleton();

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.only(top: 12.h, bottom: 16.h),
      child: Container(
        height: 110.h,
        decoration: BoxDecoration(
          color: AppColors.primaryLight,
          borderRadius: BorderRadius.circular(20.r),
        ),
        child: Center(
          child: SizedBox(
            width: 28.r,
            height: 28.r,
            child: const CircularProgressIndicator(
              strokeWidth: 2.5,
              color: AppColors.primary,
            ),
          ),
        ),
      ),
    );
  }
}
