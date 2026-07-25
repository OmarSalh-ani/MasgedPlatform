import { Link } from 'react-router-dom'
import { SITE_ROUTES } from '@/lib/siteNav'

interface PageHeroProps {
  badge?: string
  title: string
  subtitle?: string
  homeLabel?: string
}

export function PageHero({ badge, title, subtitle, homeLabel = 'الرئيسية' }: PageHeroProps) {
  return (
    <header className="page-hero">
      <div className="page-hero__bg" aria-hidden="true" />
      <div className="container page-hero__content">
        <nav className="page-hero__breadcrumb" aria-label="مسار الصفحة">
          <Link to={SITE_ROUTES.home}>{homeLabel}</Link>
          <i className="fas fa-chevron-left" aria-hidden="true" />
          <span>{title}</span>
        </nav>
        {badge && <span className="page-hero__badge">{badge}</span>}
        <h1 className="page-hero__title">{title}</h1>
        {subtitle && <p className="page-hero__subtitle">{subtitle}</p>}
      </div>
    </header>
  )
}
