import { Link } from 'react-router-dom'

interface SectionCtaProps {
  to: string
  label?: string
}

export function SectionCta({ to, label = 'استكشف المزيد' }: SectionCtaProps) {
  return (
    <div className="section-cta">
      <Link to={to} className="btn btn-outline section-cta__btn">
        {label}
        <i className="fas fa-arrow-left" aria-hidden="true" />
      </Link>
    </div>
  )
}
