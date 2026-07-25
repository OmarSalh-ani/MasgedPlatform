import { NavLink } from 'react-router-dom'
import { cn } from '@/lib/utils'
import type { AdminNavLink } from '@/types/adminNav'

interface AdminSidebarLinkProps {
  item: AdminNavLink
  nested?: boolean
  onNavigate?: () => void
}

export function AdminSidebarLink({ item, nested, onNavigate }: AdminSidebarLinkProps) {
  const Icon = item.icon
  const className = ({ isActive }: { isActive: boolean }) =>
    cn(
      'group relative flex items-center gap-3 rounded-xl px-3 text-sm transition-all duration-200',
      nested ? 'py-2 text-xs' : 'py-2.5',
      isActive
        ? 'bg-blue-500/20 font-semibold text-white shadow-inner shadow-blue-950/25'
        : 'text-blue-100/70 hover:bg-white/7 hover:text-white',
    )

  const iconClassName = (isActive: boolean) =>
    cn(
      'size-[18px] shrink-0 transition-colors',
      isActive ? 'text-blue-200' : 'text-blue-300/55 group-hover:text-blue-200',
    )

  const activeIndicator = (isActive: boolean) =>
    isActive ? (
      <span className="absolute inset-y-2 right-0 w-1 rounded-l-full bg-blue-300 shadow-[0_0_10px_rgba(147,197,253,0.65)]" />
    ) : null

  if (item.external) {
    return (
      <a
        href={item.to}
        target="_blank"
        rel="noreferrer"
        className={className({ isActive: false })}
        onClick={onNavigate}
      >
        <Icon className={iconClassName(false)} />
        <span className="leading-snug">{item.label}</span>
      </a>
    )
  }

  return (
    <NavLink to={item.to} className={className} onClick={onNavigate}>
      {({ isActive }) => (
        <>
          {activeIndicator(isActive)}
          <Icon className={iconClassName(isActive)} />
          <span className="leading-snug">{item.label}</span>
        </>
      )}
    </NavLink>
  )
}
