import { useCallback, useEffect, useState } from 'react'
import { isPwaInstallDismissed, setPwaInstallDismissed } from '@/lib/pwaInstallStorage'

interface BeforeInstallPromptEvent extends Event {
  prompt: () => Promise<void>
  userChoice: Promise<{ outcome: 'accepted' | 'dismissed' }>
}

function isStandaloneMode(): boolean {
  return (
    window.matchMedia('(display-mode: standalone)').matches ||
    (window.navigator as Navigator & { standalone?: boolean }).standalone === true
  )
}

function isIosDevice(): boolean {
  return /iphone|ipad|ipod/i.test(navigator.userAgent)
}

export function usePwaInstallPrompt() {
  const [deferredPrompt, setDeferredPrompt] = useState<BeforeInstallPromptEvent | null>(null)
  const [dismissed, setDismissed] = useState(isPwaInstallDismissed)
  const [installed, setInstalled] = useState(isStandaloneMode)
  const isIos = isIosDevice()

  useEffect(() => {
    const onBeforeInstallPrompt = (event: Event) => {
      event.preventDefault()
      setDeferredPrompt(event as BeforeInstallPromptEvent)
    }

    const onAppInstalled = () => {
      setInstalled(true)
      setDeferredPrompt(null)
    }

    window.addEventListener('beforeinstallprompt', onBeforeInstallPrompt)
    window.addEventListener('appinstalled', onAppInstalled)

    return () => {
      window.removeEventListener('beforeinstallprompt', onBeforeInstallPrompt)
      window.removeEventListener('appinstalled', onAppInstalled)
    }
  }, [])

  const dismiss = useCallback(() => {
    setPwaInstallDismissed()
    setDismissed(true)
  }, [])

  const promptInstall = useCallback(async () => {
    if (!deferredPrompt) return

    await deferredPrompt.prompt()
    const choice = await deferredPrompt.userChoice

    setDeferredPrompt(null)

    if (choice.outcome === 'accepted') {
      setInstalled(true)
    }
  }, [deferredPrompt])

  const canInstall = Boolean(deferredPrompt)
  const showBanner = !installed && !dismissed && (canInstall || isIos)

  return {
    showBanner,
    canInstall,
    isIos,
    promptInstall,
    dismiss,
  }
}
