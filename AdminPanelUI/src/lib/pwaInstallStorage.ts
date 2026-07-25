const PWA_INSTALL_DISMISSED_KEY = 'admin_pwa_install_dismissed'

export function isPwaInstallDismissed(): boolean {
  if (typeof window === 'undefined') return false
  return localStorage.getItem(PWA_INSTALL_DISMISSED_KEY) === 'true'
}

export function setPwaInstallDismissed(): void {
  localStorage.setItem(PWA_INSTALL_DISMISSED_KEY, 'true')
}
