import { useMemo } from 'react'
import { adminNavEntries } from '@/lib/adminNavConfig'
import { filterAdminNav } from '@/lib/adminNavVisibility'
import { getAdminSession } from '@/lib/authStorage'

export function useAdminNav() {
  const session = getAdminSession()

  return useMemo(() => {
    if (!session) return []
    return filterAdminNav(adminNavEntries, session)
  }, [session])
}
