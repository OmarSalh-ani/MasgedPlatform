import { useEffect } from 'react'
import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'
import { PWA_APP_NAME } from '@/lib/pwaConstants'
import { syncAppleTouchIcon, syncPwaManifest } from '@/lib/pwaManifest'

export function usePwaBrandingSync() {
  const { logoUrl } = useMasgedBranding()

  useEffect(() => {
    if (!logoUrl) return

    document.title = PWA_APP_NAME
    syncPwaManifest(logoUrl)
    syncAppleTouchIcon(logoUrl)
  }, [logoUrl])
}
