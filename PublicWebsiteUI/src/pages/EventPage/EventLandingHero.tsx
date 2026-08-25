import { resolveImageUrl } from '@/lib/resolveImageUrl'
import type { PublicEventPage } from '@/types/eventPage'

interface EventLandingHeroProps {
  page: PublicEventPage
}

export function EventLandingHero({ page }: EventLandingHeroProps) {
  const imageUrl = resolveImageUrl(page.imageUrl)

  return (
    <header className={`event-hero ${imageUrl ? 'event-hero--with-media' : ''}`}>
      {imageUrl && (
        <div className="event-hero__media">
          <img src={imageUrl} alt={page.courseTitle} />
        </div>
      )}
      <div className="event-hero__copy">
        {page.invitationText && <p className="event-hero__invite">{page.invitationText}</p>}
        <h1 className="event-hero__title">{page.courseTitle}</h1>
        {page.mosqueName && (
          <p className="event-hero__mosque">
            <i className="fas fa-mosque" aria-hidden="true" />
            {page.mosqueName}
          </p>
        )}
        {page.subjectText && <p className="event-hero__subject">{page.subjectText}</p>}
      </div>
    </header>
  )
}
