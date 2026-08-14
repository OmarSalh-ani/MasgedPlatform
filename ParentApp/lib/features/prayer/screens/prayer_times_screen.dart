import 'package:flutter/material.dart';
import 'package:masged_parent_app/core/theme/app_fonts.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:go_router/go_router.dart';
import 'package:masged_parent_app/core/services/prayer_service.dart';
import 'package:masged_parent_app/core/theme/app_colors.dart';
import '../models/prayer_schedule_item.dart';
import '../models/prayer_times_data.dart';
import '../widgets/prayer_location_error.dart';
import '../widgets/prayer_schedule_card.dart';
import '../widgets/prayer_times_hero.dart';

class PrayerTimesScreen extends StatefulWidget {
  const PrayerTimesScreen({super.key});

  @override
  State<PrayerTimesScreen> createState() => _PrayerTimesScreenState();
}

class _PrayerTimesScreenState extends State<PrayerTimesScreen> {
  Future<PrayerTimesData>? _prayerFuture;

  @override
  void initState() {
    super.initState();
    _loadPrayerTimes();
  }

  void _loadPrayerTimes() {
    setState(() {
      _prayerFuture = PrayerService().getPrayerTimes();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Directionality(
      textDirection: TextDirection.rtl,
      child: Scaffold(
        backgroundColor: AppColors.background,
        body: FutureBuilder<PrayerTimesData>(
          future: _prayerFuture,
          builder: (context, snapshot) {
            if (snapshot.connectionState == ConnectionState.waiting) {
              return const _PrayerTimesLoading();
            }

            if (snapshot.hasError) {
              if (snapshot.error is PrayerLocationException) {
                return _PrayerTimesLocationError(onRetry: _loadPrayerTimes);
              }
              return _PrayerTimesError(onRetry: _loadPrayerTimes);
            }

            if (!snapshot.hasData) {
              return _PrayerTimesError(onRetry: _loadPrayerTimes);
            }

            final times = snapshot.data!;
            final items = PrayerScheduleItem.fromPrayerTimes(times);

            return CustomScrollView(
              slivers: [
                SliverAppBar(
                  expandedHeight: 88.h,
                  floating: false,
                  pinned: true,
                  backgroundColor: AppColors.primary,
                  leading: IconButton(
                    icon: const Icon(Icons.arrow_back_ios_new_rounded,
                        color: Colors.white),
                    onPressed: () => context.pop(),
                  ),
                  flexibleSpace: FlexibleSpaceBar(
                    titlePadding: EdgeInsets.only(right: 56.w, bottom: 14.h),
                    title: Text(
                      'أوقات الصلاة',
                      style: AppFonts.cairo(
                        color: Colors.white,
                        fontWeight: FontWeight.bold,
                        fontSize: 18.sp,
                      ),
                    ),
                    background: Container(
                      decoration: const BoxDecoration(
                        gradient: AppColors.primaryGradient,
                      ),
                    ),
                  ),
                ),
                SliverToBoxAdapter(
                  child: PrayerTimesHero(times: times),
                ),
                SliverPadding(
                  padding: EdgeInsets.fromLTRB(16.w, 4.h, 16.w, 8.h),
                  sliver: SliverToBoxAdapter(
                    child: Text(
                      'جدول اليوم',
                      style: AppFonts.cairo(
                        fontSize: 16.sp,
                        fontWeight: FontWeight.bold,
                        color: AppColors.textPrimary,
                      ),
                    ),
                  ),
                ),
                SliverPadding(
                  padding: EdgeInsets.symmetric(horizontal: 16.w),
                  sliver: SliverList(
                    delegate: SliverChildBuilderDelegate(
                      (context, index) => PrayerScheduleCard(
                        item: items[index],
                        times: times,
                      ),
                      childCount: items.length,
                    ),
                  ),
                ),
                SliverPadding(
                  padding: EdgeInsets.fromLTRB(16.w, 8.h, 16.w, 32.h),
                  sliver: SliverToBoxAdapter(child: _SunnahLegendCard()),
                ),
              ],
            );
          },
        ),
      ),
    );
  }
}

class _SunnahLegendCard extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Container(
      padding: EdgeInsets.all(16.w),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20.r),
        border: Border.all(color: AppColors.gold.withValues(alpha: 0.3)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(Icons.info_outline_rounded,
                  color: AppColors.gold, size: 20.sp),
              SizedBox(width: 8.w),
              Text(
                'السنن الرواتب',
                style: AppFonts.cairo(
                  fontSize: 14.sp,
                  fontWeight: FontWeight.bold,
                  color: AppColors.textPrimary,
                ),
              ),
            ],
          ),
          SizedBox(height: 10.h),
          Text(
            'السنن الرواتب مرتبطة بالصلوات المفروضة. الوتر بعد العشاء حتى آخر الثلث من الليل. '
            'العصر والشروق ليس لهما سنة راتبة.',
            style: AppFonts.cairo(
              fontSize: 12.sp,
              color: AppColors.textSecondary,
              height: 1.5,
            ),
          ),
        ],
      ),
    );
  }
}

class _PrayerTimesLoading extends StatelessWidget {
  const _PrayerTimesLoading();

  @override
  Widget build(BuildContext context) {
    return CustomScrollView(
      slivers: [
        SliverAppBar(
          pinned: true,
          backgroundColor: AppColors.primary,
          leading: IconButton(
            icon: const Icon(Icons.arrow_back_ios_new_rounded,
                color: Colors.white),
            onPressed: () => context.pop(),
          ),
          title: Text(
            'أوقات الصلاة',
            style: AppFonts.cairo(
              color: Colors.white,
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
        SliverFillRemaining(
          child: Center(
            child: CircularProgressIndicator(
              color: AppColors.primary,
              strokeWidth: 2.5,
            ),
          ),
        ),
      ],
    );
  }
}

class _PrayerTimesLocationError extends StatelessWidget {
  const _PrayerTimesLocationError({required this.onRetry});

  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    return CustomScrollView(
      slivers: [
        SliverAppBar(
          pinned: true,
          backgroundColor: AppColors.primary,
          leading: IconButton(
            icon: const Icon(Icons.arrow_back_ios_new_rounded,
                color: Colors.white),
            onPressed: () => context.pop(),
          ),
        ),
        SliverFillRemaining(
          child: Center(
            child: PrayerLocationError(onRetry: onRetry),
          ),
        ),
      ],
    );
  }
}

class _PrayerTimesError extends StatelessWidget {
  const _PrayerTimesError({required this.onRetry});

  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    return CustomScrollView(
      slivers: [
        SliverAppBar(
          pinned: true,
          backgroundColor: AppColors.primary,
          leading: IconButton(
            icon: const Icon(Icons.arrow_back_ios_new_rounded,
                color: Colors.white),
            onPressed: () => context.pop(),
          ),
        ),
        SliverFillRemaining(
          child: Center(
            child: Padding(
              padding: EdgeInsets.all(24.w),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Icon(Icons.error_outline_rounded,
                      size: 48.sp, color: AppColors.error),
                  SizedBox(height: 16.h),
                  Text(
                    'تعذر تحميل أوقات الصلاة',
                    style: AppFonts.cairo(
                      fontSize: 16.sp,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  SizedBox(height: 16.h),
                  FilledButton(
                    onPressed: onRetry,
                    style: FilledButton.styleFrom(
                      backgroundColor: AppColors.primary,
                    ),
                    child: Text(
                      'إعادة المحاولة',
                      style: AppFonts.cairo(fontWeight: FontWeight.bold),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ],
    );
  }
}
