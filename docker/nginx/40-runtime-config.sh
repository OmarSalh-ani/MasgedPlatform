#!/bin/sh
# Runs via the nginx image's /docker-entrypoint.d hook before nginx starts.
# Writes config.js from container env so one prebuilt SPA image serves any domain
# (Vite would otherwise inline these URLs into the bundle at build time).
set -eu

target="${RUNTIME_CONFIG_PATH:-/usr/share/nginx/html/config.js}"

js_escape() {
  printf '%s' "${1:-}" | sed -e 's/\\/\\\\/g' -e 's/"/\\"/g'
}

cat > "${target}" <<EOF
window.__APP_CONFIG__ = {
  apiBaseUrl: "$(js_escape "${APP_API_BASE_URL:-}")",
  uploadsBaseUrl: "$(js_escape "${APP_UPLOADS_BASE_URL:-}")",
  publicSiteUrl: "$(js_escape "${APP_PUBLIC_SITE_URL:-}")",
  appStoreUrl: "$(js_escape "${APP_STORE_URL:-}")",
  googlePlayUrl: "$(js_escape "${APP_GOOGLE_PLAY_URL:-}")",
  mobileAppBannerImage: "$(js_escape "${APP_MOBILE_BANNER_IMAGE:-}")"
};
EOF

echo "runtime-config: wrote ${target} (api=${APP_API_BASE_URL:-unset})"
