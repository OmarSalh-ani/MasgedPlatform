import { useCallback, useState } from 'react'
import {
  getStoredSidebarOpen,
  isMobileViewport,
  setStoredSidebarOpen,
} from '@/lib/adminSidebarStorage'

export function useAdminSidebar() {
  const [sidebarOpen, setSidebarOpenState] = useState(getStoredSidebarOpen)

  const setSidebarOpen = useCallback((value: boolean | ((prev: boolean) => boolean)) => {
    setSidebarOpenState((prev) => {
      const next = typeof value === 'function' ? value(prev) : value
      setStoredSidebarOpen(next)
      return next
    })
  }, [])

  const toggleSidebar = useCallback(() => {
    setSidebarOpen((prev) => !prev)
  }, [setSidebarOpen])

  const closeSidebarOnMobile = useCallback(() => {
    if (isMobileViewport()) {
      setSidebarOpen(false)
    }
  }, [setSidebarOpen])

  return {
    sidebarOpen,
    setSidebarOpen,
    toggleSidebar,
    closeSidebarOnMobile,
  }
}
