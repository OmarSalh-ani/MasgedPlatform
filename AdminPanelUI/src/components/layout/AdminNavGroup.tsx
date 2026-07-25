import { useEffect, useState } from 'react'
import { useLocation } from 'react-router-dom'
import { ChevronDown } from 'lucide-react'
import { AdminSidebarLink } from '@/components/layout/AdminSidebarLink'
import { cn } from '@/lib/utils'
import type { AdminNavGroup } from '@/types/adminNav'

interface AdminNavGroupProps {
  group: AdminNavGroup
  onNavigate?: () => void
}

export function AdminNavGroupPanel({ group, onNavigate }: AdminNavGroupProps) {
  const { pathname } = useLocation()
  const shouldExpand = group.autoExpandPaths.some((segment) => pathname.includes(segment))
  const [open, setOpen] = useState(shouldExpand)

  useEffect(() => {
    if (shouldExpand) setOpen(true)
  }, [shouldExpand])

  const Icon = group.icon
  const isChildActive = group.autoExpandPaths.some((segment) => pathname.includes(segment))

  return (
    <div className="space-y-0.5">
      <button
        type="button"
        aria-expanded={open}
        onClick={() => setOpen((value) => !value)}
        className={cn(
          'group flex w-full items-center gap-3 rounded-xl px-3 py-2.5 text-sm transition-all duration-200',
          isChildActive
            ? 'bg-blue-500/12 font-medium text-white'
            : 'text-blue-100/70 hover:bg-white/7 hover:text-white',
        )}
      >
        <Icon
          className={cn(
            'size-[18px] shrink-0 transition-colors',
            isChildActive ? 'text-blue-200' : 'text-blue-300/55 group-hover:text-blue-200',
          )}
        />
        <span className="flex-1 text-right leading-snug">{group.label}</span>
        <ChevronDown
          className={cn(
            'size-4 shrink-0 text-blue-300/50 transition-transform duration-200',
            open && 'rotate-180',
          )}
        />
      </button>

      <ul
        className={cn(
          'mr-2 space-y-0.5 overflow-hidden border-r border-blue-400/15 pr-2 transition-all duration-200',
          open ? 'max-h-[28rem] opacity-100' : 'max-h-0 opacity-0',
        )}
      >
        {group.children.map((child) => (
          <li key={child.to}>
            <AdminSidebarLink item={child} nested onNavigate={onNavigate} />
          </li>
        ))}
      </ul>
    </div>
  )
}
