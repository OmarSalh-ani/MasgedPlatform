/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_BASE_URL?: string
  readonly VITE_UPLOADS_BASE_URL?: string
  readonly VITE_APP_STORE_URL?: string
  readonly VITE_GOOGLE_PLAY_URL?: string
  readonly VITE_MOBILE_APP_BANNER_IMAGE?: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
