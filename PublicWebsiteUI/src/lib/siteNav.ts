export const SITE_ROUTES = {
  home: '/',
  competitions: '/competitions',
  mosques: '/mosques',
  news: '/news',
  activities: '/activities',
  registration: '/registration',
  registerSuccess: '/register-success',
  privacyPolicy: '/privacy-policy',
  privacyPolicyEn: '/privacy-policy/en',
} as const

export interface SiteNavItem {
  to: string
  label: string
  requiresActivities?: boolean
  end?: boolean
}

export const SITE_NAV_ITEMS: SiteNavItem[] = [
  { to: SITE_ROUTES.home, label: 'الرئيسية', end: true },
  { to: SITE_ROUTES.competitions, label: 'مسابقاتنا' },
  { to: SITE_ROUTES.mosques, label: 'أماكننا' },
  { to: SITE_ROUTES.news, label: 'الأخبار' },
  { to: SITE_ROUTES.activities, label: 'الأنشطة', requiresActivities: true },
  { to: SITE_ROUTES.registration, label: 'التسجيل' },
]

export const FOOTER_NAV_ITEMS: SiteNavItem[] = [
  ...SITE_NAV_ITEMS,
]

export interface SectionMeta {
  badge: string
  title: string
  subtitle: string
  route: string
}

export const SECTION_META: Record<string, SectionMeta> = {
  competitions: {
    badge: 'مسابقاتنا',
    title: 'شارك واربح',
    subtitle: 'شارك في مسابقاتنا المتنوعة والمثيرة واربح جوائز قيمة',
    route: SITE_ROUTES.competitions,
  },
  mosques: {
    badge: 'مساجدنا',
    title: 'أماكننا',
    subtitle: 'تعرف على مواقع مساجدنا وزرنا في أي وقت',
    route: SITE_ROUTES.mosques,
  },
  news: {
    badge: 'الأخبار',
    title: 'آخر المستجدات',
    subtitle: 'تابع أحدث أخبارنا والمستجدات',
    route: SITE_ROUTES.news,
  },
  activities: {
    badge: 'الأنشطة',
    title: 'برامجنا وأنشطتنا',
    subtitle: 'تعرف على أنشطتنا المتنوعة والمفيدة',
    route: SITE_ROUTES.activities,
  },
  registration: {
    badge: 'التسجيل',
    title: 'انضم إلينا',
    subtitle: 'سجّل الآن وكن جزءاً من برامجنا التعليمية',
    route: SITE_ROUTES.registration,
  },
}
