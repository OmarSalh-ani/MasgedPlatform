import {
  PWA_APP_NAME,
  PWA_BACKGROUND_COLOR,
  PWA_DESCRIPTION,
  PWA_THEME_COLOR,
} from '@/lib/pwaConstants'

const STATIC_ICONS = [
  { src: '/pwa-icon-192.png', sizes: '192x192', type: 'image/png', purpose: 'any' },
  {
    src: '/pwa-icon-512.png',
    sizes: '512x512',
    type: 'image/png',
    purpose: 'any maskable',
  },
] as const

let manifestBlobUrl: string | null = null

function resolveIconType(logoUrl: string): string {
  if (logoUrl.endsWith('.svg')) return 'image/svg+xml'
  if (logoUrl.endsWith('.webp')) return 'image/webp'
  if (logoUrl.endsWith('.jpg') || logoUrl.endsWith('.jpeg')) return 'image/jpeg'
  return 'image/png'
}

function buildManifest(logoUrl: string) {
  const iconType = resolveIconType(logoUrl)

  return {
    name: PWA_APP_NAME,
    short_name: PWA_APP_NAME,
    description: PWA_DESCRIPTION,
    start_url: '/',
    scope: '/',
    display: 'standalone',
    lang: 'ar',
    dir: 'rtl',
    background_color: PWA_BACKGROUND_COLOR,
    theme_color: PWA_THEME_COLOR,
    icons: [
      { src: logoUrl, sizes: '192x192', type: iconType, purpose: 'any' },
      { src: logoUrl, sizes: '512x512', type: iconType, purpose: 'any maskable' },
      ...STATIC_ICONS,
    ],
  }
}

function upsertManifestLink(href: string) {
  let link = document.querySelector<HTMLLinkElement>('link[rel="manifest"]')
  if (!link) {
    link = document.createElement('link')
    link.rel = 'manifest'
    document.head.appendChild(link)
  }
  link.href = href
}

export function syncPwaManifest(logoUrl: string) {
  if (manifestBlobUrl) {
    URL.revokeObjectURL(manifestBlobUrl)
  }

  const manifest = buildManifest(logoUrl)
  const blob = new Blob([JSON.stringify(manifest)], { type: 'application/json' })
  manifestBlobUrl = URL.createObjectURL(blob)
  upsertManifestLink(manifestBlobUrl)
}

export function syncAppleTouchIcon(logoUrl: string) {
  let link = document.querySelector<HTMLLinkElement>('link[rel="apple-touch-icon"]')
  if (!link) {
    link = document.createElement('link')
    link.rel = 'apple-touch-icon'
    document.head.appendChild(link)
  }
  link.href = logoUrl
}
