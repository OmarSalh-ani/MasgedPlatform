import { UPLOADS_BASE_URL } from '@/lib/constants'

const UPLOAD_PATH_PREFIX = /^\/(uploads|Uploads|FilesManager)\//i

function normalizePath(url: string): string {
  if (url.startsWith('/')) return url
  return `/${url.replace(/^~\//, '').replace(/^\//, '')}`
}

function toUploadsUrl(path: string): string {
  return `${UPLOADS_BASE_URL.replace(/\/$/, '')}${path}`
}

export function resolveImageUrl(url?: string | null): string {
  if (!url?.trim()) return ''
  const trimmed = url.trim()

  if (trimmed.startsWith('http://') || trimmed.startsWith('https://')) {
    if (!import.meta.env.DEV) {
      try {
        const { pathname } = new URL(trimmed)
        if (UPLOAD_PATH_PREFIX.test(pathname)) return toUploadsUrl(pathname)
      } catch {
        // ignore invalid URLs
      }
    }
    return trimmed
  }

  const path = normalizePath(trimmed)

  // In dev, Vite proxies /uploads to AdminAPI. In production, uploads live on the API host.
  if (!import.meta.env.DEV && UPLOAD_PATH_PREFIX.test(path)) {
    return toUploadsUrl(path)
  }

  return path
}
