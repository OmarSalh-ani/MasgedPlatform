#!/usr/bin/env bash
# Create or update .env from .env.example with strong random secrets.
# Can run without root. Does not install Docker or start compose.
#
#   ./docker/scripts/generate-env.sh --domain customer.com --email admin@customer.com
#   ./docker/scripts/generate-env.sh --domain customer.com --email admin@customer.com --force

set -euo pipefail

DOMAIN=""
ACME_EMAIL=""
FORCE=0
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(cd "${SCRIPT_DIR}/../.." && pwd)"

usage() {
  cat <<'EOF'
Usage: ./docker/scripts/generate-env.sh --domain DOMAIN --email EMAIL [--force]

  --domain DOMAIN   Apex domain (customer.com)
  --email EMAIL     ACME / Let's Encrypt email
  --force           Overwrite existing .env and regenerate all secrets
  -h, --help
EOF
}

die() { printf 'ERROR: %s\n' "$*" >&2; exit 1; }

random_secret() { openssl rand -hex 24; }
random_sql_password() {
  local raw
  raw="$(openssl rand -base64 24 | tr -d '/+=' | head -c 20)"
  printf 'Aa1!%s' "${raw}"
}

set_env_value() {
  local file="$1" key="$2" value="$3"
  if grep -qE "^${key}=" "${file}"; then
    local escaped
    escaped="$(printf '%s' "${value}" | sed -e 's/[&\\]/\\&/g')"
    sed -i.bak "s|^${key}=.*|${key}=${escaped}|" "${file}"
    rm -f "${file}.bak"
  else
    printf '%s=%s\n' "${key}" "${value}" >> "${file}"
  fi
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --domain) DOMAIN="${2:-}"; shift 2 ;;
    --email) ACME_EMAIL="${2:-}"; shift 2 ;;
    --force) FORCE=1; shift ;;
    -h|--help) usage; exit 0 ;;
    *) die "Unknown option: $1" ;;
  esac
done

[[ -n "${DOMAIN}" && -n "${ACME_EMAIL}" ]] || { usage; exit 1; }
DOMAIN="${DOMAIN#https://}"; DOMAIN="${DOMAIN#http://}"; DOMAIN="${DOMAIN%/}"

example="${PROJECT_DIR}/.env.example"
envfile="${PROJECT_DIR}/.env"
[[ -f "${example}" ]] || die "Missing ${example}"

if [[ -f "${envfile}" && "${FORCE}" -eq 0 ]]; then
  echo ".env exists — updating DOMAIN / ACME_EMAIL only (pass --force to regenerate secrets)"
  set_env_value "${envfile}" "DOMAIN" "${DOMAIN}"
  set_env_value "${envfile}" "ACME_EMAIL" "${ACME_EMAIL}"
else
  cp "${example}" "${envfile}"
  set_env_value "${envfile}" "DOMAIN" "${DOMAIN}"
  set_env_value "${envfile}" "ACME_EMAIL" "${ACME_EMAIL}"
  set_env_value "${envfile}" "MSSQL_SA_PASSWORD" "$(random_sql_password)"
  set_env_value "${envfile}" "JWT_KEY" "$(random_secret)"
  set_env_value "${envfile}" "MOBILE_JWT_KEY" "$(random_secret)"
  set_env_value "${envfile}" "TEACHER_JWT_KEY" "$(random_secret)"
  set_env_value "${envfile}" "STUDENT_QR_KEY" "$(random_secret)"
  set_env_value "${envfile}" "CHAT_INTERNAL_KEY" "$(random_secret)"
  echo "Created ${envfile} with generated secrets"
fi

chmod 600 "${envfile}" 2>/dev/null || true
echo "Done: ${envfile}"
