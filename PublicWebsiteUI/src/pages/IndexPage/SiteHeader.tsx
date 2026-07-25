import { useEffect, useRef, useState } from 'react'
import { Link, NavLink, useLocation } from 'react-router-dom'
import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'
import { SITE_NAV_ITEMS, SITE_ROUTES } from '@/lib/siteNav'
import { useScrollHeader } from '@/hooks/useScrollHeader'

const LOGIN_LINKS = [
  { href: 'https://teacher.mosque-mbark-j.com/', icon: 'fas fa-chalkboard-teacher', label: 'لوحة المعلمين', external: true },
  { href: 'https://parent.mosque-mbark-j.com/', icon: 'fas fa-users', label: 'لوحة أولياء الأمور', external: true },
] as const

interface SiteHeaderProps {
  showActivitiesNav: boolean
}

export function SiteHeader({ showActivitiesNav }: SiteHeaderProps) {
  const { masgedName, logoUrl } = useMasgedBranding()
  const scrolled = useScrollHeader()
  const location = useLocation()
  const [menuOpen, setMenuOpen] = useState(false)
  const [dropdownOpen, setDropdownOpen] = useState(false)
  const dropdownRef = useRef<HTMLDivElement>(null)

  const visibleNav = SITE_NAV_ITEMS.filter(
    (item) => !item.requiresActivities || showActivitiesNav,
  )

  useEffect(() => {
    document.body.style.overflow = menuOpen ? 'hidden' : ''
    return () => { document.body.style.overflow = '' }
  }, [menuOpen])

  useEffect(() => {
    setMenuOpen(false)
    setDropdownOpen(false)
  }, [location.pathname])

  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target as Node)) {
        setDropdownOpen(false)
      }
    }
    document.addEventListener('mousedown', handleClickOutside)
    return () => document.removeEventListener('mousedown', handleClickOutside)
  }, [])

  return (
    <>
      <header className={`site-header${scrolled ? ' site-header--scrolled' : ''}`}>
        <nav className="site-nav" aria-label="التنقل الرئيسي">
          <Link to={SITE_ROUTES.home} className="site-logo">
            <img src={logoUrl} alt={masgedName} />
            <span>{masgedName}</span>
          </Link>

          <ul className="nav-links">
            {visibleNav.map((item) => (
              <li key={item.to}>
                <NavLink
                  to={item.to}
                  end={item.end}
                  className={({ isActive }) => (isActive ? 'active' : undefined)}
                >
                  {item.label}
                </NavLink>
              </li>
            ))}
            <li>
              <div
                className={`nav-dropdown${dropdownOpen ? ' nav-dropdown--open' : ''}`}
                ref={dropdownRef}
              >
                <button
                  type="button"
                  className="nav-dropdown-trigger"
                  aria-expanded={dropdownOpen}
                  onClick={() => setDropdownOpen((o) => !o)}
                >
                  <i className="fas fa-sign-in-alt" aria-hidden="true" />
                  تسجيل الدخول
                  <i className={`fas fa-chevron-down nav-dropdown-chevron${dropdownOpen ? ' nav-dropdown-chevron--open' : ''}`} aria-hidden="true" />
                </button>
                <div className="nav-dropdown-menu" role="menu">
                  {LOGIN_LINKS.map((link) => (
                    <a
                      key={link.href}
                      href={link.href}
                      className="nav-dropdown-item"
                      role="menuitem"
                      {...('external' in link && link.external ? { target: '_blank', rel: 'noreferrer' } : {})}
                    >
                      <i className={link.icon} aria-hidden="true" />
                      {link.label}
                    </a>
                  ))}
                </div>
              </div>
            </li>
          </ul>

          <button
            type="button"
            className={`nav-toggle${menuOpen ? ' nav-toggle--open' : ''}`}
            aria-label={menuOpen ? 'إغلاق القائمة' : 'فتح القائمة'}
            aria-expanded={menuOpen}
            onClick={() => setMenuOpen((o) => !o)}
          >
            <span /><span /><span />
          </button>
        </nav>
      </header>

      <div className={`mobile-drawer${menuOpen ? ' mobile-drawer--open' : ''}`} aria-hidden={!menuOpen}>
        <div className="mobile-drawer-backdrop" onClick={() => setMenuOpen(false)} />
        <div className="mobile-drawer-panel">
          <ul className="mobile-nav-links">
            {visibleNav.map((item) => (
              <li key={item.to}>
                <NavLink to={item.to} end={item.end}>
                  {item.label}
                </NavLink>
              </li>
            ))}
          </ul>
          <div className="mobile-nav-divider" />
          <div className="mobile-login-links">
            {LOGIN_LINKS.map((link) => (
              <a
                key={link.href}
                href={link.href}
                className="btn btn-ghost btn-block"
                {...('external' in link && link.external ? { target: '_blank', rel: 'noreferrer' } : {})}
              >
                <i className={link.icon} aria-hidden="true" />
                {link.label}
              </a>
            ))}
          </div>
        </div>
      </div>
    </>
  )
}

export { HeroSection } from '@/pages/IndexPage/HeroSection'
