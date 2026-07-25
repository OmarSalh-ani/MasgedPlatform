/**
 * Values injected by the container at startup (`/config.js`) so one prebuilt
 * image can serve any customer domain. Empty in local dev — Vite env wins there.
 */
export interface AppRuntimeConfig {
  apiBaseUrl?: string
  uploadsBaseUrl?: string
  publicSiteUrl?: string
}

declare global {
  interface Window {
    __APP_CONFIG__?: AppRuntimeConfig
  }
}

export const runtimeConfig: AppRuntimeConfig =
  typeof window === 'undefined' ? {} : (window.__APP_CONFIG__ ?? {})

/** Unset container env renders as an empty string, which `??` would not skip. */
export function firstNonEmpty(...values: Array<string | undefined>): string {
  for (const value of values) {
    if (typeof value === 'string' && value.trim() !== '') return value
  }
  return ''
}
