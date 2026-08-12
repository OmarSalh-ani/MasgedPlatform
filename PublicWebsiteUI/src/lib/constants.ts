export const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ?? 'https://admin-api.mosque-mbark-j.com/api'

/** AdminAPI host — uploads (hero, mosques, news, etc.) are stored and served here, not on the public site. */
export const UPLOADS_BASE_URL =
  import.meta.env.VITE_UPLOADS_BASE_URL ?? API_BASE_URL.replace(/\/api\/?$/, '')
export const DEFAULT_LOGO_URL = '/assets/images/logo.png'

export const APP_STORE_URL = import.meta.env.VITE_APP_STORE_URL ?? '#'
export const GOOGLE_PLAY_URL = import.meta.env.VITE_GOOGLE_PLAY_URL ?? '#'
export const MOBILE_APP_BANNER_IMAGE = import.meta.env.VITE_MOBILE_APP_BANNER_IMAGE ?? ''

export const HOME_PREVIEW_LIMIT = {
  tips: 3,
  mosques: 6,
  news: 3,
  activities: 3,
} as const
