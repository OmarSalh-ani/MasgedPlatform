import type { PublicEventPageTrack } from '@/types/eventPage'

interface EventLandingTracksProps {
  tracks: PublicEventPageTrack[]
}

export function EventLandingTracks({ tracks }: EventLandingTracksProps) {
  if (tracks.length === 0) return null

  return (
    <section className="event-tracks">
      <h2>مسارات الدورة</h2>
      <div className="event-tracks__grid">
        {tracks.map((track, index) => (
          <article key={`${track.title}-${index}`} className="event-track-card">
            <span className="event-track-card__num">{index + 1}</span>
            <h3>{track.title}</h3>
            {track.description && <p>{track.description}</p>}
          </article>
        ))}
      </div>
    </section>
  )
}
