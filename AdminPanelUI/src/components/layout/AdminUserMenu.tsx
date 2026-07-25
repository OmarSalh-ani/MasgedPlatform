import { useEffect, useRef, useState } from 'react'
import { ChevronDown, KeyRound, LogOut, ShieldCheck } from 'lucide-react'
import { ChangePasswordDialog } from '@/components/layout/dialogs/ChangePasswordDialog'
import { cn } from '@/lib/utils'

interface AdminUserMenuProps {
  username: string
  onLogout: () => void
}

export function AdminUserMenu({ username, onLogout }: AdminUserMenuProps) {
  const [open, setOpen] = useState(false)
  const [changePasswordOpen, setChangePasswordOpen] = useState(false)
  const containerRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    if (!open) return

    const handleClickOutside = (event: MouseEvent) => {
      if (containerRef.current?.contains(event.target as Node)) return
      setOpen(false)
    }

    document.addEventListener('mousedown', handleClickOutside)
    return () => document.removeEventListener('mousedown', handleClickOutside)
  }, [open])

  const handleChangePassword = () => {
    setOpen(false)
    setChangePasswordOpen(true)
  }

  const handleLogout = () => {
    setOpen(false)
    onLogout()
  }

  return (
    <>
      <div ref={containerRef} className="relative">
        <button
          type="button"
          aria-expanded={open}
          aria-haspopup="menu"
          onClick={() => setOpen((value) => !value)}
          className={cn(
            'flex items-center gap-2 rounded-xl border border-blue-100 bg-gradient-to-l from-blue-50 to-white px-2.5 py-1.5 transition hover:border-blue-200 hover:bg-blue-50/80 md:px-3 md:py-2',
            open && 'border-blue-200 bg-blue-50/80',
          )}
        >
          <div className="flex size-8 items-center justify-center rounded-lg bg-[var(--color-primary)] text-white shadow-sm shadow-blue-500/30">
            <ShieldCheck className="size-4" />
          </div>
          <div className="hidden min-w-0 text-right sm:block">
            <p className="truncate text-sm font-semibold text-slate-800">{username}</p>
            <p className="text-[11px] text-blue-600/70">مسؤول النظام</p>
          </div>
          <ChevronDown
            className={cn(
              'size-4 shrink-0 text-blue-500/70 transition-transform',
              open && 'rotate-180',
            )}
          />
        </button>

        {open && (
          <div
            role="menu"
            className="absolute left-0 top-[calc(100%+0.5rem)] z-50 w-56 overflow-hidden rounded-xl border border-blue-100 bg-white py-1 shadow-lg shadow-blue-900/10"
          >
            <button
              type="button"
              role="menuitem"
              onClick={handleChangePassword}
              className="flex w-full items-center gap-3 px-4 py-2.5 text-sm text-slate-700 transition hover:bg-blue-50"
            >
              <KeyRound className="size-4 text-[var(--color-primary)]" />
              <span>تغيير كلمة المرور</span>
            </button>
            <div className="my-1 h-px bg-blue-100" />
            <button
              type="button"
              role="menuitem"
              onClick={handleLogout}
              className="flex w-full items-center gap-3 px-4 py-2.5 text-sm text-red-600 transition hover:bg-red-50"
            >
              <LogOut className="size-4" />
              <span>تسجيل الخروج</span>
            </button>
          </div>
        )}
      </div>

      <ChangePasswordDialog open={changePasswordOpen} onOpenChange={setChangePasswordOpen} />
    </>
  )
}
