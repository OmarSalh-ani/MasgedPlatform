import { Link } from 'react-router-dom'
import { SITE_ROUTES } from '@/lib/siteNav'

export function RegisterCtaBand({ enabled }: { enabled: boolean }) {
  if (!enabled) return null

  return (
    <section className="cta-band" aria-labelledby="register-cta-title">
      <div className="container cta-band__inner">
        <div className="cta-band__copy">
          <span className="cta-band__eyebrow">انضم إلينا</span>
          <h2 id="register-cta-title" className="cta-band__title">
            ابدأ رحلتك التعليمية اليوم
          </h2>
          <p className="cta-band__desc">
            سجّل الآن في برامجنا واستفد من بيئة تعليمية متميزة
          </p>
        </div>
        <Link to={SITE_ROUTES.registration} className="btn btn-primary btn-lg cta-band__btn">
          <i className="fas fa-user-plus" aria-hidden="true" />
          سجّل الآن
        </Link>
      </div>
    </section>
  )
}
