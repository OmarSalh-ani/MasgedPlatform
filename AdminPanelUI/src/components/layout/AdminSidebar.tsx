import { AdminNavGroupPanel } from '@/components/layout/AdminNavGroup'
import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'

import { AdminSidebarLink } from '@/components/layout/AdminSidebarLink'

import { useAdminNav } from '@/hooks/useAdminNav'

import { cn } from '@/lib/utils'

import type { AdminNavEntry } from '@/types/adminNav'



interface AdminSidebarProps {

  open: boolean

  onClose: () => void

}



function AdminSidebarSection({ label }: { label: string }) {

  return (

    <div className="px-3 pb-1 pt-5 first:pt-2">

      <p className="text-[10px] font-bold tracking-[0.14em] text-blue-300/45 uppercase">{label}</p>

      <div className="mt-2 h-px bg-gradient-to-l from-transparent via-blue-400/25 to-transparent" />

    </div>

  )

}



function renderNavEntry(entry: AdminNavEntry, index: number, onClose: () => void) {

  if (entry.type === 'section') {

    return <AdminSidebarSection key={`section-${entry.label}-${index}`} label={entry.label} />

  }

  if (entry.type === 'group') {

    return <AdminNavGroupPanel key={entry.id} group={entry} onNavigate={onClose} />

  }

  if (entry.type === 'link') {

    return <AdminSidebarLink key={entry.to} item={entry} onNavigate={onClose} />

  }

  return null

}



export function AdminSidebar({ open, onClose }: AdminSidebarProps) {

  const entries = useAdminNav()

  const { masgedName, logoUrl } = useMasgedBranding()



  return (

    <>

      <div

        className={cn(

          'fixed inset-0 z-40 bg-slate-950/55 backdrop-blur-[2px] transition-opacity md:hidden',

          open ? 'opacity-100' : 'pointer-events-none opacity-0',

        )}

        onClick={onClose}

        aria-hidden={!open}

      />



      <aside

        className={cn(

          'fixed top-0 right-0 z-50 flex h-full w-[var(--sidebar-width)] flex-col border-l border-blue-400/15 shadow-2xl shadow-blue-950/40 transition-transform duration-300',

          'bg-[linear-gradient(180deg,var(--color-sidebar-from)_0%,var(--color-sidebar-to)_55%,#0a1628_100%)]',

          open ? 'translate-x-0' : 'translate-x-full',

        )}

      >

        <div className="relative shrink-0 overflow-hidden px-5 pb-4 pt-6">

          <div className="pointer-events-none absolute inset-0 bg-[radial-gradient(circle_at_top,rgba(96,165,250,0.22),transparent_62%)]" />

          <div className="relative mx-auto mb-3 flex size-[4.5rem] items-center justify-center rounded-2xl bg-white/10 p-2 ring-1 ring-blue-200/20 backdrop-blur-sm">

            <img

              src={logoUrl}

              alt="شعار المسجد"

              className="size-full rounded-xl object-contain"

            />

          </div>

          <p className="relative text-center text-[15px] font-bold leading-snug text-white">

            {masgedName}

          </p>

          <p className="relative mt-1 text-center text-[11px] font-medium tracking-wide text-blue-200/65">

            لوحة التحكم الإدارية

          </p>

        </div>



        <nav className="flex-1 space-y-0.5 overflow-y-auto px-3 pb-4 [scrollbar-color:rgba(147,197,253,0.35)_transparent] [scrollbar-width:thin]">

          {entries.map((entry, index) => renderNavEntry(entry, index, onClose))}

        </nav>

      </aside>

    </>

  )

}


