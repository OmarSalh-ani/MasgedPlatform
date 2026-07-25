import { useState } from 'react'
import type { PublicMosqueItem } from '@/types/publicIndex'
import { SectionCta } from '@/components/SectionCta'
import { SectionHeader } from '@/components/SectionHeader'
import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'
import { useMosqueCarousel } from '@/hooks/useMosqueCarousel'
import { SECTION_META } from '@/lib/siteNav'

function EmptySection({ message }: { message: string }) {
  return (
    <div className="empty-state">
      <i className="fas fa-inbox" aria-hidden="true" />
      <p>{message}</p>
    </div>
  )
}

function MosqueCardImage({
  src,
  alt,
  isPlaceholder,
}: {
  src: string
  alt: string
  isPlaceholder: boolean
}) {
  const { logoUrl: fallbackLogoUrl } = useMasgedBranding()
  const [imageFailed, setImageFailed] = useState(false)
  const displaySrc = !imageFailed ? src : fallbackLogoUrl
  const showAsLogo = isPlaceholder || imageFailed

  return (
    <div className={`mosque-carousel-card__image${showAsLogo ? ' mosque-carousel-card__image--logo' : ''}`}>
      <img
        src={displaySrc}
        alt={alt}
        loading="lazy"
        onError={() => {
          if (!imageFailed) setImageFailed(true)
        }}
      />
    </div>
  )
}

export function MosquesSection({
  items,
  limit,
  viewAllHref,
  showCta = false,
  showHeader = true,
}: {
  items: PublicMosqueItem[]
  limit?: number
  viewAllHref?: string
  showCta?: boolean
  showHeader?: boolean
}) {
  const meta = SECTION_META.mosques
  const displayItems = limit ? items.slice(0, limit) : items
  const hasMore = limit ? items.length > limit : false
  const { logoUrl } = useMasgedBranding()
  const { viewportRef, cards, activeIndex, goTo, goNext, goPrev, count, pause, resume } =
    useMosqueCarousel(displayItems, logoUrl)

  return (
    <section id="mosques" className="section">
      <div className="container">
        {showHeader && (
          <SectionHeader
            badge={meta.badge}
            title={meta.title}
            subtitle={meta.subtitle}
            action={viewAllHref ? { label: 'كل الأماكن', to: viewAllHref } : undefined}
          />
        )}
        {displayItems.length === 0 ? (
          <EmptySection message="لا توجد مساجد مسجلة حالياً" />
        ) : (
          <div
            className="mosque-carousel"
            onMouseEnter={pause}
            onMouseLeave={resume}
            onFocusCapture={pause}
            onBlurCapture={resume}
          >
            {count > 1 && (
              <>
                <button
                  type="button"
                  className="mosque-carousel-nav mosque-carousel-nav--prev"
                  aria-label="المسجد السابق"
                  onClick={goPrev}
                >
                  <i className="fas fa-chevron-right" aria-hidden="true" />
                </button>
                <button
                  type="button"
                  className="mosque-carousel-nav mosque-carousel-nav--next"
                  aria-label="المسجد التالي"
                  onClick={goNext}
                >
                  <i className="fas fa-chevron-left" aria-hidden="true" />
                </button>
              </>
            )}

            <div ref={viewportRef} className="mosque-carousel-viewport">
              <div className="mosque-carousel-track">
                {cards.map((card) => (
                  <article
                    key={card.id}
                    data-carousel-index={card.index}
                    className={`mosque-carousel-card${card.index === activeIndex ? ' is-active' : ''}`}
                  >
                    <MosqueCardImage
                      src={card.imageSrc}
                      alt={card.name}
                      isPlaceholder={card.isPlaceholder}
                    />
                    <div className="mosque-carousel-card__body">
                      <h3 className="mosque-carousel-card__title">
                        <i className="fas fa-mosque" aria-hidden="true" />
                        <span className="mosque-carousel-card__title-text">{card.name}</span>
                      </h3>
                      <p className="mosque-carousel-card__desc">
                        {card.description || ' '}
                      </p>
                      {card.googleMapsUrl ? (
                        <a
                          href={card.googleMapsUrl}
                          className="mosque-carousel-card__link"
                          target="_blank"
                          rel="noreferrer"
                        >
                          <i className="fas fa-directions" aria-hidden="true" />
                          اتجاهات
                        </a>
                      ) : (
                        <span className="mosque-carousel-card__link-spacer" aria-hidden="true" />
                      )}
                    </div>
                  </article>
                ))}
              </div>
            </div>

            {count > 1 && (
              <div className="mosque-carousel-dots" role="tablist" aria-label="مساجدنا">
                {cards.map((card) => (
                  <button
                    key={card.id}
                    type="button"
                    className={`mosque-carousel-dot${card.index === activeIndex ? ' active' : ''}`}
                    role="tab"
                    aria-selected={card.index === activeIndex}
                    aria-label={`${card.name} (${card.index + 1} من ${count})`}
                    onClick={() => goTo(card.index)}
                  />
                ))}
              </div>
            )}
          </div>
        )}
        {showCta && hasMore && viewAllHref && (
          <SectionCta to={viewAllHref} label="عرض جميع الأماكن" />
        )}
      </div>
    </section>
  )
}
