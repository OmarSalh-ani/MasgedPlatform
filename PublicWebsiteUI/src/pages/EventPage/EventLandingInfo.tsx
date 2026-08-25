import type { PublicEventPage } from '@/types/eventPage'

interface EventLandingInfoProps {
  page: PublicEventPage
}

function InfoCard({
  icon,
  title,
  body,
}: {
  icon: string
  title: string
  body: string
}) {
  return (
    <article className="event-info-card">
      <div className="event-info-card__icon" aria-hidden="true">
        <i className={icon} />
      </div>
      <h3>{title}</h3>
      <p>{body}</p>
    </article>
  )
}

export function EventLandingInfo({ page }: EventLandingInfoProps) {
  const supervisors = (page.supervisorsText ?? '')
    .split('\n')
    .map((line) => line.trim())
    .filter(Boolean)

  return (
    <section className="event-info">
      <div className="event-info__grid">
        {page.dateText && (
          <InfoCard icon="fas fa-calendar-alt" title="تاريخ الدورة" body={page.dateText} />
        )}
        {page.timeText && (
          <InfoCard icon="fas fa-clock" title="الوقت" body={page.timeText} />
        )}
        {page.locationNote && (
          <InfoCard icon="fas fa-map-marker-alt" title="موقع المسجد" body={page.locationNote} />
        )}
        {page.contactPhone && (
          <InfoCard icon="fas fa-phone" title="للتسجيل والاستفسار" body={page.contactPhone} />
        )}
        {page.socialAccounts && (
          <InfoCard icon="fas fa-at" title="حسابات المسجد" body={page.socialAccounts} />
        )}
      </div>
      {page.extraNotes && (
        <div className="event-notes">
          {page.extraNotes}
        </div>
      )}
      {supervisors.length > 0 && (
        <div className="event-supervisors">
          <h3>الدورة تحت إشراف</h3>
          <ul>
            {supervisors.map((name) => (
              <li key={name}>{name}</li>
            ))}
          </ul>
        </div>
      )}
    </section>
  )
}
