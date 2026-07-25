import { useEffect } from 'react'
import { Link } from 'react-router-dom'
import { PageHero } from '@/components/PageHero'
import {
  getPrivacyPolicyContent,
  type PrivacyLocale,
} from '@/content/privacyPolicyContent'
import { SITE_ROUTES } from '@/lib/siteNav'
import './privacy-policy.css'

interface PrivacyPolicyPageProps {
  locale?: PrivacyLocale
}

function formatLastUpdated(date: string, locale: PrivacyLocale): string {
  const parsed = new Date(`${date}T00:00:00`)
  if (Number.isNaN(parsed.getTime())) {
    return date
  }

  return parsed.toLocaleDateString(locale === 'ar' ? 'ar-KW' : 'en-GB', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  })
}

export function PrivacyPolicyPage({ locale = 'ar' }: PrivacyPolicyPageProps) {
  const copy = getPrivacyPolicyContent(locale)
  const switchPath =
    locale === 'ar' ? SITE_ROUTES.privacyPolicyEn : SITE_ROUTES.privacyPolicy

  useEffect(() => {
    document.documentElement.lang = copy.locale
    document.documentElement.dir = copy.dir

    return () => {
      document.documentElement.lang = 'ar'
      document.documentElement.dir = 'rtl'
    }
  }, [copy.dir, copy.locale])

  return (
    <main className="privacy-policy-page" dir={copy.dir} lang={copy.locale}>
      <PageHero
        badge={copy.badge}
        title={copy.title}
        subtitle={copy.subtitle}
        homeLabel={copy.homeLabel}
      />

      <section className="privacy-policy-section">
        <div className="container">
          <div className="privacy-policy-toolbar">
            <p className="privacy-policy-updated">
              {locale === 'ar' ? 'آخر تحديث: ' : 'Last updated: '}
              <time dateTime={copy.lastUpdated}>
                {formatLastUpdated(copy.lastUpdated, locale)}
              </time>
            </p>
            <Link
              to={switchPath}
              className="privacy-policy-lang-switch"
              hrefLang={locale === 'ar' ? 'en' : 'ar'}
            >
              {copy.switchLinkText}
            </Link>
          </div>

          <article className="privacy-policy-article">
            {copy.sections.map((section) => (
              <section
                key={section.id ?? section.title}
                id={section.id}
                className="privacy-policy-block"
              >
                <h2>{section.title}</h2>
                {section.paragraphs.map((paragraph) => (
                  <p key={paragraph}>{paragraph}</p>
                ))}
                {section.bullets && section.bullets.length > 0 && (
                  <ul>
                    {section.bullets.map((item) => (
                      <li key={item}>{item}</li>
                    ))}
                  </ul>
                )}
              </section>
            ))}
          </article>
        </div>
      </section>
    </main>
  )
}
