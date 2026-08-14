import 'package:flutter/material.dart';

class OnboardingSlide {
  const OnboardingSlide({
    required this.title,
    required this.description,
    required this.imageAsset,
    required this.icon,
  });

  final String title;
  final String description;
  final String imageAsset;
  final IconData icon;
}

const kOnboardingSlides = <OnboardingSlide>[
  OnboardingSlide(
    title: 'إدارة حلقات تحفيظ القرآن',
    description:
        'أنشئ حلقات، أضف الطلاب، وتابع التقدم في الحفظ والمراجعة بسهولة.',
    imageAsset:
        'assets/illustrations/onboarding/onboarding_1_quran_circles.png',
    icon: Icons.menu_book_rounded,
  ),
  OnboardingSlide(
    title: 'مواقيت الصلاة بدقة',
    description: 'اعرض مواقيت الصلاة حسب موقعك مع تنبيهات قبل الأذان.',
    imageAsset: 'assets/illustrations/onboarding/onboarding_2_prayer_times.png',
    icon: Icons.mosque_rounded,
  ),
  
  OnboardingSlide(
    title: 'التسبيح والذكر',
    description:
        'سبح، احمد، واستغفر الله في أي وقت مع عدّاد تسبيح بسيط واجهات مريحة.',
    imageAsset: 'assets/illustrations/onboarding/onboarding_4_tasbih_dhikr.png',
    icon: Icons.volunteer_activism_rounded,
  ),
];
