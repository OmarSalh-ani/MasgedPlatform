#!/usr/bin/env bash
# One-shot Ubuntu deploy that BUILDS the images from source on the server.
# For the faster prebuilt-image path see docker/scripts/install.sh.
# Run as root or with sudo. DNS A records must already point at this server.
#
# Examples:
#   sudo ./docker/scripts/bootstrap.sh --domain customer.com --email admin@customer.com
#   sudo ./docker/scripts/bootstrap.sh --domain customer.com --email admin@customer.com \
#        --repo https://github.com/org/masged.git --dir /opt/masged
#   sudo ./docker/scripts/bootstrap.sh --domain customer.com --email admin@customer.com --skip-build

set -euo pipefail

DOMAIN=""
ACME_EMAIL=""
REPO_URL=""
INSTALL_DIR="/opt/masged"
SKIP_DOCKER_INSTALL=0
SKIP_FIREWALL=0
SKIP_BUILD=0
FORCE_ENV=0
START_COMPOSE=1

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
# Repo root when this file lives at docker/scripts/bootstrap.sh
REPO_ROOT_FROM_SCRIPT="$(cd "${SCRIPT_DIR}/../.." && pwd)"

usage() {
  cat <<'EOF'
Usage: sudo ./docker/scripts/bootstrap.sh --domain DOMAIN --email EMAIL [options]

Required:
  --domain DOMAIN       Apex domain only (e.g. customer.com)
  --email EMAIL         Let's Encrypt / ACME contact email

Options:
  --repo URL            Git clone URL (if project is not already on the server)
  --dir PATH            Install directory (default: /opt/masged)
  --skip-docker-install Skip Docker Engine install if already present
  --skip-firewall       Skip UFW rules
  --skip-build          Create .env / install only; do not run docker compose
  --force-env           Overwrite existing .env (regenerates secrets)
  --no-start            Same as --skip-build
  -h, --help            Show this help

What this script does:
  1. Installs Docker Engine + Compose plugin (Ubuntu)
  2. Opens UFW ports 22/80/443
  3. Clones the repo into --dir (or uses the current checkout)
  4. Creates .env from .env.example with strong random secrets
  5. Builds and starts the stack from source

DNS still must be set manually at your registrar before SSL works.
EOF
}

log()  { printf '\n==> %s\n' "$*"; }
warn() { printf 'WARNING: %s\n' "$*" >&2; }
die()  { printf 'ERROR: %s\n' "$*" >&2; exit 1; }

need_root() {
  if [[ "${EUID}" -ne 0 ]]; then
    die "Run as root: sudo $0 ..."
  fi
}

parse_args() {
  while [[ $# -gt 0 ]]; do
    case "$1" in
      --domain) DOMAIN="${2:-}"; shift 2 ;;
      --email) ACME_EMAIL="${2:-}"; shift 2 ;;
      --repo) REPO_URL="${2:-}"; shift 2 ;;
      --dir) INSTALL_DIR="${2:-}"; shift 2 ;;
      --skip-docker-install) SKIP_DOCKER_INSTALL=1; shift ;;
      --skip-firewall) SKIP_FIREWALL=1; shift ;;
      --skip-build|--no-start) SKIP_BUILD=1; START_COMPOSE=0; shift ;;
      --force-env) FORCE_ENV=1; shift ;;
      -h|--help) usage; exit 0 ;;
      *) die "Unknown option: $1 (use --help)" ;;
    esac
  done

  [[ -n "${DOMAIN}" ]] || die "--domain is required"
  [[ -n "${ACME_EMAIL}" ]] || die "--email is required"

  # Strip protocol / trailing slash if pasted by mistake
  DOMAIN="${DOMAIN#https://}"
  DOMAIN="${DOMAIN#http://}"
  DOMAIN="${DOMAIN%/}"
  if [[ "${DOMAIN}" == *"/"* ]]; then
    die "DOMAIN must be apex only (got: ${DOMAIN})"
  fi
}

random_secret() {
  # 48 hex chars = 24 bytes; always >= 32 for JWT keys
  openssl rand -hex 24
}

random_sql_password() {
  # SQL SA complexity: upper, lower, digit, symbol; avoid shell-hostile chars
  local raw
  raw="$(openssl rand -base64 24 | tr -d '/+=' | head -c 20)"
  printf 'Aa1!%s' "${raw}"
}

install_docker() {
  if command -v docker >/dev/null 2>&1 && docker compose version >/dev/null 2>&1; then
    log "Docker already installed: $(docker --version)"
    return 0
  fi

  if [[ "${SKIP_DOCKER_INSTALL}" -eq 1 ]]; then
    die "Docker not found and --skip-docker-install was set"
  fi

  log "Installing Docker Engine + Compose plugin"
  apt-get update -y
  apt-get install -y ca-certificates curl gnupg

  install -m 0755 -d /etc/apt/keyrings
  if [[ ! -f /etc/apt/keyrings/docker.gpg ]]; then
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg | gpg --dearmor -o /etc/apt/keyrings/docker.gpg
    chmod a+r /etc/apt/keyrings/docker.gpg
  fi

  # shellcheck disable=SC1091
  . /etc/os-release
  echo \
    "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
    ${VERSION_CODENAME} stable" > /etc/apt/sources.list.d/docker.list

  apt-get update -y
  apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

  systemctl enable --now docker

  # Allow the invoking sudo user to use docker without sudo next login
  if [[ -n "${SUDO_USER:-}" && "${SUDO_USER}" != "root" ]]; then
    usermod -aG docker "${SUDO_USER}" || true
    log "Added ${SUDO_USER} to the docker group (log out/in for non-sudo docker)"
  fi

  docker --version
  docker compose version
}

setup_firewall() {
  if [[ "${SKIP_FIREWALL}" -eq 1 ]]; then
    log "Skipping firewall (--skip-firewall)"
    return 0
  fi

  log "Configuring UFW (OpenSSH + 80/443)"
  apt-get install -y ufw

  ufw allow OpenSSH
  ufw allow 80/tcp
  ufw allow 443/tcp

  # Non-interactive enable
  ufw --force enable
  ufw status verbose
}

resolve_project_dir() {
  # Prefer an existing checkout that already has docker-compose.yml
  if [[ -f "${REPO_ROOT_FROM_SCRIPT}/docker-compose.yml" ]]; then
    PROJECT_DIR="${REPO_ROOT_FROM_SCRIPT}"
    log "Using existing project at ${PROJECT_DIR}"
    return 0
  fi

  if [[ -f "${INSTALL_DIR}/docker-compose.yml" ]]; then
    PROJECT_DIR="${INSTALL_DIR}"
    log "Using existing project at ${PROJECT_DIR}"
    return 0
  fi

  if [[ -z "${REPO_URL}" ]]; then
    die "Project not found. Pass --repo <git-url> or run this script from inside the cloned repo."
  fi

  log "Cloning ${REPO_URL} into ${INSTALL_DIR}"
  apt-get install -y git
  mkdir -p "$(dirname "${INSTALL_DIR}")"
  if [[ -d "${INSTALL_DIR}/.git" ]]; then
    git -C "${INSTALL_DIR}" pull --ff-only || warn "git pull failed; continuing with existing tree"
  else
    mkdir -p "${INSTALL_DIR}"
    # If dir exists but empty-ish, clone into it
    if [[ -z "$(ls -A "${INSTALL_DIR}" 2>/dev/null || true)" ]]; then
      git clone "${REPO_URL}" "${INSTALL_DIR}"
    else
      die "${INSTALL_DIR} is not empty and has no docker-compose.yml. Choose another --dir or clear it."
    fi
  fi

  PROJECT_DIR="${INSTALL_DIR}"
  [[ -f "${PROJECT_DIR}/docker-compose.yml" ]] || die "Clone succeeded but docker-compose.yml is missing"
}

set_env_value() {
  local file="$1" key="$2" value="$3"
  if grep -qE "^${key}=" "${file}"; then
    # Escape & and \ for sed replacement
    local escaped
    escaped="$(printf '%s' "${value}" | sed -e 's/[&\\]/\\&/g')"
    sed -i "s|^${key}=.*|${key}=${escaped}|" "${file}"
  else
    printf '%s=%s\n' "${key}" "${value}" >> "${file}"
  fi
}

create_env() {
  local example="${PROJECT_DIR}/.env.example"
  local envfile="${PROJECT_DIR}/.env"

  [[ -f "${example}" ]] || die "Missing ${example}"

  if [[ -f "${envfile}" && "${FORCE_ENV}" -eq 0 ]]; then
    log ".env already exists — updating DOMAIN / ACME_EMAIL only (use --force-env to regenerate secrets)"
    set_env_value "${envfile}" "DOMAIN" "${DOMAIN}"
    set_env_value "${envfile}" "ACME_EMAIL" "${ACME_EMAIL}"
    chmod 600 "${envfile}"
    return 0
  fi

  log "Creating .env with generated secrets"
  cp "${example}" "${envfile}"

  set_env_value "${envfile}" "DOMAIN" "${DOMAIN}"
  set_env_value "${envfile}" "ACME_EMAIL" "${ACME_EMAIL}"
  set_env_value "${envfile}" "MSSQL_SA_PASSWORD" "$(random_sql_password)"
  set_env_value "${envfile}" "JWT_KEY" "$(random_secret)"
  set_env_value "${envfile}" "MOBILE_JWT_KEY" "$(random_secret)"
  set_env_value "${envfile}" "TEACHER_JWT_KEY" "$(random_secret)"
  set_env_value "${envfile}" "STUDENT_QR_KEY" "$(random_secret)"
  set_env_value "${envfile}" "CHAT_INTERNAL_KEY" "$(random_secret)"

  chmod 600 "${envfile}"
  log "Wrote ${envfile} (mode 600). Secrets were auto-generated."
}

check_dns() {
  log "Checking DNS for ${DOMAIN}"
  local server_ip domain_ip
  server_ip="$(curl -4 -fsS --max-time 10 https://ifconfig.me 2>/dev/null || curl -4 -fsS --max-time 10 https://api.ipify.org 2>/dev/null || true)"

  if ! command -v dig >/dev/null 2>&1; then
    apt-get install -y dnsutils >/dev/null 2>&1 || true
  fi

  if command -v dig >/dev/null 2>&1; then
    domain_ip="$(dig +short "${DOMAIN}" A | tail -n1 || true)"
    printf '  Server public IP: %s\n' "${server_ip:-unknown}"
    printf '  %s → %s\n' "${DOMAIN}" "${domain_ip:-unresolved}"
    printf '  admin.%s → %s\n' "${DOMAIN}" "$(dig +short "admin.${DOMAIN}" A | tail -n1 || true)"
    printf '  api.%s → %s\n' "${DOMAIN}" "$(dig +short "api.${DOMAIN}" A | tail -n1 || true)"

    if [[ -n "${server_ip}" && -n "${domain_ip}" && "${server_ip}" != "${domain_ip}" ]]; then
      warn "DNS for ${DOMAIN} (${domain_ip}) does not match this server (${server_ip}). Let's Encrypt may fail until DNS propagates."
    elif [[ -z "${domain_ip}" ]]; then
      warn "DNS for ${DOMAIN} not resolving yet. Point A records (@, www, admin, api) to this server before expecting HTTPS."
    fi
  else
    warn "dig not available; skipped DNS check. Ensure A records point here before SSL."
  fi
}

start_stack() {
  if [[ "${START_COMPOSE}" -eq 0 ]]; then
    log "Skipping docker compose (--skip-build)"
    return 0
  fi

  log "Building and starting stack (first build can take 10–30 minutes)"
  cd "${PROJECT_DIR}"
  docker compose -f docker-compose.yml -f docker-compose.build.yml up -d --build

  log "Container status"
  docker compose ps
}

print_next_steps() {
  cat <<EOF

============================================================
Bootstrap finished.

Project:  ${PROJECT_DIR}
Domain:   ${DOMAIN}
Admin:    https://admin.${DOMAIN}/setup
Public:   https://${DOMAIN}
API:      https://api.${DOMAIN}

Next:
  1. Confirm DNS A records (@, www, admin, api) → this server
  2. Wait for containers:  cd ${PROJECT_DIR} && docker compose ps
  3. Open setup wizard:    https://admin.${DOMAIN}/setup
  4. Then login + Integrations (Wasender / Agora if needed)

Useful:
  docker compose -f ${PROJECT_DIR}/docker-compose.yml logs -f
  docker compose -f ${PROJECT_DIR}/docker-compose.yml logs -f traefik

.env path: ${PROJECT_DIR}/.env  (keep private)
============================================================
EOF
}

main() {
  need_root
  parse_args "$@"

  # Basic packages used by later steps
  export DEBIAN_FRONTEND=noninteractive
  apt-get update -y
  apt-get install -y ca-certificates curl openssl

  install_docker
  setup_firewall
  resolve_project_dir
  create_env
  check_dns
  start_stack
  print_next_steps
}

main "$@"
