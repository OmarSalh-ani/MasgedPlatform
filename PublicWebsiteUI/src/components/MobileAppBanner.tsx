import { useState } from 'react'
import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'
import {
  APP_STORE_URL,
  GOOGLE_PLAY_URL,
  MOBILE_APP_BANNER_IMAGE,
} from '@/lib/constants'
import { resolveImageUrl } from '@/lib/resolveImageUrl'

type AppAudience = 'parent' | 'teacher'

const AUDIENCE_TABS: { id: AppAudience; label: string }[] = [
  { id: 'parent', label: 'أولياء الأمور' },
  { id: 'teacher', label: 'المعلمون' },
]

const AUDIENCE_COPY: Record<
  AppAudience,
  { description: string; appStoreFallback: string; playFallback: string }
> = {
  parent: {
    description: 'تابع أبناءك، استلم الإشعارات، وابقَ على تواصل مع الجمعية من أي مكان',
    appStoreFallback: APP_STORE_URL,
    playFallback: GOOGLE_PLAY_URL,
  },
  teacher: {
    description: 'سجّل الحضور، تابع الحلقات، وأرسل الملاحظات لأولياء الأمور بسهولة',
    appStoreFallback: '#',
    playFallback: '#',
  },
}

function resolveStoreUrl(apiUrl: string | null, fallback: string) {
  const trimmed = apiUrl?.trim()
  return trimmed || fallback
}

export function MobileAppBanner() {
  const { masgedName, logoUrl, mobileAppLinks } = useMasgedBranding()
  const [audience, setAudience] = useState<AppAudience>('parent')
  const bannerSrc = resolveImageUrl(MOBILE_APP_BANNER_IMAGE)
  const copy = AUDIENCE_COPY[audience]

  const appStoreUrl =
    audience === 'parent'
      ? resolveStoreUrl(mobileAppLinks.parentAppStoreUrl, copy.appStoreFallback)
      : resolveStoreUrl(mobileAppLinks.teacherAppStoreUrl, copy.appStoreFallback)

  const googlePlayUrl =
    audience === 'parent'
      ? resolveStoreUrl(mobileAppLinks.parentGooglePlayUrl, copy.playFallback)
      : resolveStoreUrl(mobileAppLinks.teacherGooglePlayUrl, copy.playFallback)

  const showAppStore = appStoreUrl !== '#'
  const showGooglePlay = googlePlayUrl !== '#'

  return (
    <section className="mobile-app-banner" aria-labelledby="mobile-app-title">
      <div
        className="mobile-app-banner__visual"
        style={bannerSrc ? { backgroundImage: `url('${bannerSrc}')` } : undefined}
        aria-hidden="true"
      />
      <div className="mobile-app-banner__overlay" aria-hidden="true" />
      <div className="container mobile-app-banner__content">
        <div className="mobile-app-banner__copy">
          <span className="mobile-app-banner__eyebrow">تطبيق الجوال</span>
          <h2 id="mobile-app-title" className="mobile-app-banner__title">
            حمّل تطبيق {masgedName}
          </h2>

          <div
            className="mobile-app-banner__chips"
            role="tablist"
            aria-label="نوع التطبيق"
          >
            {AUDIENCE_TABS.map((tab) => (
              <button
                key={tab.id}
                type="button"
                role="tab"
                aria-selected={audience === tab.id}
                className={`mobile-app-banner__chip${audience === tab.id ? ' mobile-app-banner__chip--active' : ''}`}
                onClick={() => setAudience(tab.id)}
              >
                {tab.label}
              </button>
            ))}
          </div>

          <p className="mobile-app-banner__desc">{copy.description}</p>

          {(showAppStore || showGooglePlay) && (
            <div className="store-buttons">
              {showAppStore && (
                <a
                  href={appStoreUrl}
                  className="store-btn store-btn--apple"
                  target="_blank"
                  rel="noreferrer"
                  aria-label={`تحميل تطبيق ${AUDIENCE_TABS.find((t) => t.id === audience)?.label} من App Store`}
                >
                  <i className="fab fa-apple" aria-hidden="true" />
                  <span className="store-btn__text">
                    <small>حمّل من</small>
                    App Store
                  </span>
                </a>
              )}
              {showGooglePlay && (
                <a
                  href={googlePlayUrl}
                  className="store-btn store-btn--google"
                  target="_blank"
                  rel="noreferrer"
                  aria-label={`تحميل تطبيق ${AUDIENCE_TABS.find((t) => t.id === audience)?.label} من Google Play`}
                >
                  <i className="fab fa-google-play" aria-hidden="true" />
                  <span className="store-btn__text">
                    <small>حمّل من</small>
                    Google Play
                  </span>
                </a>
              )}
            </div>
          )}
        </div>
        <div className="mobile-app-banner__device" aria-hidden="true">
          <div className="mobile-app-banner__phone">
            <div className="mobile-app-banner__phone-screen">
              <img src={logoUrl} alt="" />
            </div>
          </div>
        </div>
      </div>
    </section>
  )
}
