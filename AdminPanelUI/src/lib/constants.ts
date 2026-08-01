export const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ?? 'https://admin-api.mosque-mbark-j.com/api'

/** AdminAPI host — uploads are stored and served here, not on the admin panel site. */
export const UPLOADS_BASE_URL =
  import.meta.env.VITE_UPLOADS_BASE_URL ?? API_BASE_URL.replace(/\/api\/?$/, '')
export const PUBLIC_SITE_URL = import.meta.env.VITE_PUBLIC_SITE_URL ?? 'https://mosque-mbark-j.com'
