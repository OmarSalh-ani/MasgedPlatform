import type { ReactNode } from 'react'
import { useState } from 'react'
import type {
  PublicActivityItem,
  PublicCompetitionItem,
  PublicNewsItem,
} from '@/types/publicIndex'
import { openImageModal } from '@/components/ImageModal'
import { SectionCta } from '@/components/SectionCta'
import { SectionHeader } from '@/components/SectionHeader'
import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'
import { SECTION_META } from '@/lib/siteNav'
import { resolveImageUrl } from '@/lib/resolveImageUrl'

export { MosquesSection } from './MosquesSection'

interface SectionPreviewProps {
  limit?: number
  viewAllHref?: string
  showCta?: boolean
  showHeader?: boolean
}

function ImageCard({
  imageUrl,
  children,
}: {
  imageUrl?: string | null
  children: ReactNode
}) {
  const { logoUrl: fallbackLogoUrl } = useMasgedBranding()
  const resolved = resolveImageUrl(imageUrl)
  const [imageFailed, setImageFailed] = useState(false)
  const displaySrc = resolved && !imageFailed ? resolved : fallbackLogoUrl
  const isPlaceholder = !resolved || imageFailed
  const clickable = Boolean(resolved && !imageFailed)

  return (
    <article
      className={`content-card${clickable ? ' content-card--clickable' : ''}`}
      onClick={() => clickable && openImageModal(resolved)}
      onKeyDown={(e) => {
        if (clickable && (e.key === 'Enter' || e.key === ' ')) {
          e.preventDefault()
          openImageModal(resolved)
        }
      }}
      {...(clickable ? { role: 'button', tabIndex: 0 } : {})}
    >
      <div className={`content-card-image${isPlaceholder ? ' content-card-image--logo' : ''}`}>
        <img
          src={displaySrc}
          alt=""
          loading="lazy"
          onError={() => {
            if (resolved && !imageFailed) setImageFailed(true)
          }}
        />
      </div>
      {children}
    </article>
  )
}

function EmptySection({ message }: { message: string }) {
  return (
    <div className="empty-state">
      <i className="fas fa-inbox" aria-hidden="true" />
      <p>{message}</p>
    </div>
  )
}

export function CompetitionsSection({
  items,
  limit,
  viewAllHref,
  showCta = false,
  showHeader = true,
}: { items: PublicCompetitionItem[] } & SectionPreviewProps) {
  const meta = SECTION_META.competitions
  const displayItems = limit ? items.slice(0, limit) : items
  const hasMore = limit ? items.length > limit : false

  return (
    <section id="competitions" className="section section--pattern">
      <div className="container">
        {showHeader && (
          <SectionHeader
            badge={meta.badge}
            title={meta.title}
            subtitle={meta.subtitle}
            action={viewAllHref ? { label: 'كل المسابقات', to: viewAllHref } : undefined}
          />
        )}
        {displayItems.length === 0 ? (
          <EmptySection message="لا توجد مسابقات حالياً" />
        ) : (
          <div className="cards-grid cards-grid--uniform">
            {displayItems.map((item) => (
              <ImageCard key={item.id} imageUrl={item.imageUrl}>
                <div className="content-card-body">
                  <h3 className="content-card-title">{item.title}</h3>
                  <p className="content-card-desc">{item.description}</p>
                  {item.linkUrl && (
                    <a href={item.linkUrl} className="content-card-link" target="_blank" rel="noreferrer" onClick={(e) => e.stopPropagation()}>
                      المزيد <i className="fas fa-arrow-left" aria-hidden="true" />
                    </a>
                  )}
                </div>
              </ImageCard>
            ))}
          </div>
        )}
        {showCta && hasMore && viewAllHref && <SectionCta to={viewAllHref} label="عرض جميع المسابقات" />}
      </div>
    </section>
  )
}

export function NewsSection({
  items,
  limit,
  viewAllHref,
  showCta = false,
  showHeader = true,
}: { items: PublicNewsItem[] } & SectionPreviewProps) {
  const meta = SECTION_META.news
  const displayItems = limit ? items.slice(0, limit) : items
  const hasMore = limit ? items.length > limit : false

  return (
    <section id="news" className="section section--alt">
      <div className="container">
        {showHeader && (
          <SectionHeader
            badge={meta.badge}
            title={meta.title}
            subtitle={meta.subtitle}
            action={viewAllHref ? { label: 'كل الأخبار', to: viewAllHref } : undefined}
          />
        )}
        {displayItems.length === 0 ? (
          <EmptySection message="لا توجد أخبار حالياً" />
        ) : (
          <div className="cards-grid cards-grid--uniform">
            {displayItems.map((item) => (
              <ImageCard key={item.id} imageUrl={item.imageUrl}>
                <div className="content-card-body">
                  <time className="content-card-date" dateTime={item.newsDate}>
                    {new Date(item.newsDate).toLocaleDateString('ar-KW', {
                      year: 'numeric',
                      month: 'long',
                      day: 'numeric',
                    })}
                  </time>
                  <h3 className="content-card-title">{item.title}</h3>
                  <p className="content-card-desc">{item.description}</p>
                  {item.linkUrl && (
                    <a href={item.linkUrl} className="content-card-link" target="_blank" rel="noreferrer" onClick={(e) => e.stopPropagation()}>
                      اقرأ المزيد <i className="fas fa-arrow-left" aria-hidden="true" />
                    </a>
                  )}
                </div>
              </ImageCard>
            ))}
          </div>
        )}
        {showCta && hasMore && viewAllHref && <SectionCta to={viewAllHref} label="عرض جميع الأخبار" />}
      </div>
    </section>
  )
}

export function ActivitiesSection({
  items,
  limit,
  viewAllHref,
  showCta = false,
  showHeader = true,
}: { items: PublicActivityItem[] } & SectionPreviewProps) {
  const meta = SECTION_META.activities
  const displayItems = limit ? items.slice(0, limit) : items
  const hasMore = limit ? items.length > limit : false

  return (
    <section id="activities" className="section">
      <div className="container">
        {showHeader && (
          <SectionHeader
            badge={meta.badge}
            title={meta.title}
            subtitle={meta.subtitle}
            action={viewAllHref ? { label: 'كل الأنشطة', to: viewAllHref } : undefined}
          />
        )}
        {displayItems.length === 0 ? (
          <EmptySection message="لا توجد أنشطة حالياً" />
        ) : (
          <div className="cards-grid cards-grid--uniform">
            {displayItems.map((item) => (
              <ImageCard key={item.id} imageUrl={item.imageUrl}>
                <div className="content-card-body">
                  <h3 className="content-card-title">{item.title}</h3>
                  <p className="content-card-desc">{item.description}</p>
                </div>
              </ImageCard>
            ))}
          </div>
        )}
        {showCta && hasMore && viewAllHref && <SectionCta to={viewAllHref} label="عرض جميع الأنشطة" />}
      </div>
    </section>
  )
}

export function PortalSection() {
  return (
    <section className="section section--pattern" id="portals">
      <div className="container">
        <SectionHeader
          badge="البوابات"
          title="البوابات الإلكترونية"
          subtitle="دخول المعلمين وأولياء الأمور إلى منصاتهم الخاصة"
        />
        <div className="portals-grid">
          <article className="portal-card">
            <div className="portal-icon">
              <i className="fas fa-chalkboard-teacher" aria-hidden="true" />
            </div>
            <h3>لوحة المعلمين</h3>
            <p>منصة خاصة للمعلمين لإدارة الحلقات والطلاب</p>
            <a
              href="https://teacher.mosque-mbark-j.com/login.aspx"
              target="_blank"
              rel="noreferrer"
              className="btn btn-primary btn-block"
            >
              <i className="fas fa-sign-in-alt" aria-hidden="true" />
              دخول المعلمين
            </a>
          </article>
          <article className="portal-card">
            <div className="portal-icon">
              <i className="fas fa-users" aria-hidden="true" />
            </div>
            <h3>لوحة أولياء الأمور</h3>
            <p>منصة خاصة لأولياء الأمور لمتابعة أبنائهم</p>
            <a
              href="https://parent.mosque-mbark-j.com/login.aspx"
              target="_blank"
              rel="noreferrer"
              className="btn btn-ghost btn-block"
            >
              <i className="fas fa-sign-in-alt" aria-hidden="true" />
              دخول أولياء الأمور
            </a>
          </article>
        </div>
      </div>
    </section>
  )
}
