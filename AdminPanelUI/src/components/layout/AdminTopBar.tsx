import { Bell } from 'lucide-react'

import { AdminUserMenu } from '@/components/layout/AdminUserMenu'

import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'
import { useAdminPageTitle } from '@/hooks/useAdminPageTitle'

import { getAdminSession } from '@/lib/authStorage'

import { cn } from '@/lib/utils'

interface AdminTopBarProps {
  sidebarOpen: boolean
  onMenuToggle: () => void
  onLogout: () => void
}

function formatTodayLabel() {
  return new Intl.DateTimeFormat('ar-EG', {
    weekday: 'long',
    day: 'numeric',
    month: 'long',
    year: 'numeric',
  }).format(new Date())
}

export function AdminTopBar({ sidebarOpen, onMenuToggle, onLogout }: AdminTopBarProps) {
  const session = getAdminSession()
  const pageTitle = useAdminPageTitle()
  const { masgedName } = useMasgedBranding()
  const username = session?.username ?? 'مستخدم'

  return (
    <header className="sticky top-0 z-30 border-b border-blue-200/70 bg-white/85 backdrop-blur-xl">
      <div className="flex items-center justify-between gap-4 px-4 py-3 md:px-6 md:py-4">
        <div className="flex min-w-0 items-center gap-3">
          <button
            type="button"
            className="flex size-10 shrink-0 items-center justify-center rounded-xl border border-blue-200 bg-blue-50/80 transition hover:bg-blue-100/80"
            onClick={onMenuToggle}
            aria-label={sidebarOpen ? 'إغلاق القائمة' : 'فتح القائمة'}
          >
            <i
              className={cn(
                sidebarOpen ? 'fas fa-times' : 'fas fa-bars',
                'text-lg text-[#2563eb]',
              )}
              aria-hidden="true"
            />
          </button>

          <div className="min-w-0">
            <p className="truncate text-xs font-medium text-blue-500/80">{masgedName}</p>
            <h1 className="truncate text-lg font-bold text-slate-800 md:text-xl">{pageTitle}</h1>
          </div>
        </div>

        <div className="flex shrink-0 items-center gap-2 md:gap-3">
          <span className="hidden text-xs text-slate-500 lg:inline">{formatTodayLabel()}</span>

          <button
            type="button"
            aria-label="الإشعارات"
            className="hidden size-10 items-center justify-center rounded-xl border border-blue-100 bg-blue-50/80 text-[var(--color-primary)] transition hover:bg-blue-100/80 sm:flex"
          >
            <Bell className="size-[18px]" />
          </button>

          <AdminUserMenu username={username} onLogout={onLogout} />
        </div>
      </div>
    </header>
  )
}
