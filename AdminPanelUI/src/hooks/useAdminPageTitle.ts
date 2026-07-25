import { useMemo } from 'react'
import { useLocation } from 'react-router-dom'
import { adminNavEntries } from '@/lib/adminNavConfig'
import type { AdminNavEntry } from '@/types/adminNav'

function findNavLabel(entries: AdminNavEntry[], pathname: string): string | null {
  for (const entry of entries) {
    if (entry.type === 'link') {
      if (pathname === entry.to || pathname.startsWith(`${entry.to}/`)) {
        return entry.label
      }
    }
    if (entry.type === 'group') {
      for (const child of entry.children) {
        if (pathname === child.to || pathname.startsWith(`${child.to}/`)) {
          return child.label
        }
      }
    }
  }
  return null
}

export function useAdminPageTitle() {
  const { pathname } = useLocation()
  return useMemo(() => findNavLabel(adminNavEntries, pathname) ?? 'لوحة التحكم', [pathname])
}
