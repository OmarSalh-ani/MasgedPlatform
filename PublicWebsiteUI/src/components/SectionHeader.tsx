import type { ReactNode } from 'react'
import { Link } from 'react-router-dom'

interface SectionHeaderProps {
  badge?: string
  title: string
  subtitle?: string
  action?: { label: string; to: string }
  children?: ReactNode
}

export function SectionHeader({ badge, title, subtitle, action, children }: SectionHeaderProps) {
  return (
    <div className="section-header">
      <div className="section-header__main">
        {badge && <span className="section-badge">{badge}</span>}
        <h2 className="section-title">{title}</h2>
        {subtitle && <p className="section-subtitle">{subtitle}</p>}
      </div>
      {(action || children) && (
        <div className="section-header__actions">
          {action && (
            <Link to={action.to} className="btn btn-outline section-header__link">
              {action.label}
              <i className="fas fa-arrow-left" aria-hidden="true" />
            </Link>
          )}
          {children}
        </div>
      )}
    </div>
  )
}
