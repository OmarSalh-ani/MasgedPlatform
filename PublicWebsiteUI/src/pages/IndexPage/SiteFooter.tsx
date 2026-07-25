import { Link } from 'react-router-dom'
import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'
import { FOOTER_NAV_ITEMS, SITE_ROUTES } from '@/lib/siteNav'
import type { PublicAbout, PublicSocialLinkItem } from '@/types/publicIndex'

export function SiteFooter({
  about,
  socialLinks,
}: {
  about: PublicAbout
  socialLinks: PublicSocialLinkItem[]
}) {
  const { masgedName, logoUrl } = useMasgedBranding()

  return (
    <footer className="site-footer">
      <div className="footer-grid">
        <div className="footer-block">
          <h3>عن الجمعية</h3>
          <p id="AboutContent">{about.content}</p>
          {about.address && (
            <p className="footer-address">
              <i className="fas fa-map-marker-alt" aria-hidden="true" />
              <span id="AboutAddressText">{about.address}</span>
            </p>
          )}
          {about.mapsUrl && (
            <a
              id="AboutMapsUrl"
              href={about.mapsUrl}
              target="_blank"
              rel="noreferrer"
              className="footer-maps-link"
            >
              <i className="fas fa-external-link-alt" aria-hidden="true" />
              عرض على الخريطة
            </a>
          )}
          {socialLinks.length > 0 && (
            <div className="social-row">
              {socialLinks.map((link) => (
                <a
                  key={link.id}
                  href={link.url || '#'}
                  className="social-btn"
                  target="_blank"
                  rel="noreferrer"
                  title={link.platformName}
                  aria-label={link.platformName}
                >
                  <i className={link.resolvedIconClass} aria-hidden="true" />
                </a>
              ))}
            </div>
          )}
        </div>

        <div className="footer-block">
          <h3>روابط سريعة</h3>
          <ul className="footer-links">
            {FOOTER_NAV_ITEMS.map((link) => (
              <li key={link.to}>
                <Link to={link.to}>
                  <i className="fas fa-chevron-left" aria-hidden="true" />
                  {link.label}
                </Link>
              </li>
            ))}
            <li>
              <Link to={SITE_ROUTES.privacyPolicy}>
                <i className="fas fa-chevron-left" aria-hidden="true" />
                سياسة الخصوصية
              </Link>
            </li>
          </ul>
        </div>

        <div className="footer-block">
          <h3>شعاراتنا</h3>
          <div className="footer-logos">
            <img src={logoUrl} alt={`شعار ${masgedName}`} loading="lazy" />
          </div>
          <Link to={SITE_ROUTES.registration} className="btn btn-primary btn-block footer-register-btn">
            <i className="fas fa-user-plus" aria-hidden="true" />
            سجّل الآن
          </Link>
        </div>
      </div>

      <div className="footer-bottom">
        <p>&copy; {new Date().getFullYear()} {masgedName}. جميع الحقوق محفوظة.</p>
      </div>
    </footer>
  )
}
