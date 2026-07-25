const SIDEBAR_OPEN_KEY = 'admin_sidebar_open'

export function getStoredSidebarOpen(): boolean {
  if (typeof window === 'undefined') {
    return true
  }

  const stored = localStorage.getItem(SIDEBAR_OPEN_KEY)
  if (stored !== null) {
    return stored === 'true'
  }

  return window.matchMedia('(min-width: 768px)').matches
}

export function setStoredSidebarOpen(open: boolean): void {
  localStorage.setItem(SIDEBAR_OPEN_KEY, String(open))
}

export function isMobileViewport(): boolean {
  return window.matchMedia('(max-width: 767px)').matches
}
