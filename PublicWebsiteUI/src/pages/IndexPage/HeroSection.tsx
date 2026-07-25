import { Link } from 'react-router-dom'
import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'
import { useHeroSlider } from '@/hooks/useHeroSlider'
import { SITE_ROUTES } from '@/lib/siteNav'

export function HeroSection({
  slides,
  showRegisterButton,
  showActivitiesButton,
}: {
  slides: ReturnType<typeof useHeroSlider>
  showRegisterButton: boolean
  showActivitiesButton: boolean
}) {
  const { masgedName } = useMasgedBranding()

  return (
    <section id="home" className="hero">
      <div className="hero-slider" aria-hidden="true">
        {slides.map((slide) => (
          <div
            key={slide.id}
            className={`hero-slide${slide.isActive ? ' active' : ''}`}
            style={{ backgroundImage: `url('${slide.imageSrc}')` }}
          />
        ))}
      </div>
      <div className="hero-overlay" aria-hidden="true" />

      <div className="hero-content">
        <span className="hero-eyebrow">مرحباً بكم</span>
        <h1>{masgedName}</h1>
        <p className="hero-desc">
          نسعى لنشر العلم الشرعي والقيم الإسلامية في المجتمع من خلال برامج تعليمية متميزة وأنشطة اجتماعية مؤثرة
        </p>
        <div className="hero-actions">
          {showRegisterButton && (
            <Link to={SITE_ROUTES.registration} className="btn btn-primary btn-lg">
              <i className="fas fa-user-plus" aria-hidden="true" />
              سجل الآن
            </Link>
          )}
          {showActivitiesButton && (
            <Link to={SITE_ROUTES.activities} className="btn btn-outline btn-lg">
              <i className="fas fa-star" aria-hidden="true" />
              تعرف على الأنشطة
            </Link>
          )}
          <Link to={SITE_ROUTES.mosques} className="btn btn-ghost btn-lg hero-btn-light">
            <i className="fas fa-mosque" aria-hidden="true" />
            أماكننا
          </Link>
        </div>
      </div>

      {slides.length > 1 && (
        <div className="hero-dots" role="tablist" aria-label="شرائح العرض">
          {slides.map((slide, i) => (
            <button
              key={slide.id}
              type="button"
              className={`hero-dot${slide.isActive ? ' active' : ''}`}
              role="tab"
              aria-selected={slide.isActive}
              aria-label={`الشريحة ${i + 1}`}
              onClick={() => slide.goTo(i)}
            />
          ))}
        </div>
      )}

      <Link to={SITE_ROUTES.competitions} className="hero-scroll" aria-label="انتقل للمحتوى">
        <i className="fas fa-chevron-down" />
      </Link>
    </section>
  )
}
