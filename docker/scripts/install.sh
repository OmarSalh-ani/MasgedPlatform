#!/usr/bin/env bash
# Plug-and-play install for Masged Parent App using prebuilt images from GHCR.
# No source code, no build. Installs Docker, opens the firewall, writes .env,
# logs in to the registry, pulls images, and starts the stack.
#
#   # One-time: put the PAT on the server (never commit this file)
#   sudo mkdir -p /opt/masged && sudo chmod 700 /opt/masged
#   echo 'ghp_xxxxxxxx' | sudo tee /opt/masged/.ghcr-token >/dev/null
#   sudo chmod 600 /opt/masged/.ghcr-token
#
#   sudo ./install.sh --domain customer.com --email admin@customer.com \
#        --repo YourOrg/YourRepo
#
# Token resolution order: --token → --token-file → $INSTALL_DIR/.ghcr-token →
# GHCR_TOKEN / GITHUB_TOKEN env. Required scopes: read:packages (+ repo if private).

set -euo pipefail

DOMAIN=""
ACME_EMAIL=""
REPO=""
TOKEN=""
TOKEN_FILE=""
BRANCH="main"
INSTALL_DIR="/opt/masged"
IMAGE_REGISTRY=""
IMAGE_TAG="latest"
COMPOSE_URL=""
SKIP_FIREWALL=0
START_STACK=1
FORCE_ENV=0
SAVE_TOKEN=0
LOCAL_MODE=0

usage() {
  cat <<'EOF'
Usage: sudo ./install.sh --domain DOMAIN --email EMAIL --repo OWNER/REPO [options]

Required:
  --domain DOMAIN     Apex domain (customer.com or masged.local for VMware)
  --email EMAIL       Let's Encrypt contact email (use any address with --local)
  --repo OWNER/REPO   GitHub repo holding the compose file and packages

Token (one of these):
  --token TOKEN           GitHub PAT (read:packages, plus repo if private)
  --token-file PATH       Read the PAT from a file (chmod 600 recommended)
  /opt/masged/.ghcr-token Default file (created/updated when --token is passed with --save-token)
  $GHCR_TOKEN / $GITHUB_TOKEN   Environment variables

Options:
  --local             VMware / lab mode: HTTP only, no Let's Encrypt, no public DNS
  --save-token        Write --token into $INSTALL_DIR/.ghcr-token for next runs
  --branch NAME       Branch to fetch compose from (default: main)
  --dir PATH          Install directory (default: /opt/masged)
  --tag TAG           Image tag to deploy (default: latest)
  --registry PATH     Image registry path (default: ghcr.io/<owner lowercased>)
  --compose-url URL   Direct URL to docker-compose.yml (overrides --repo/--branch)
  --skip-firewall     Do not configure UFW
  --no-start          Set everything up but do not pull/start containers
  --force-env         Overwrite an existing .env (regenerates all secrets)
  -h, --help          Show this help

Production needs DNS A records for @, www, admin and api.
With --local, edit the hosts file on your Windows PC instead (script prints the lines).
EOF
}

log()  { printf '\n==> %s\n' "$*"; }
warn() { printf 'WARNING: %s\n' "$*" >&2; }
die()  { printf 'ERROR: %s\n' "$*" >&2; exit 1; }

read_token_file() {
  local path="$1"
  [[ -f "${path}" ]] || return 1
  # Strip CR and trailing newlines; reject empty/whitespace-only
  TOKEN="$(tr -d '\r' < "${path}" | sed -e 's/^[[:space:]]*//' -e 's/[[:space:]]*$//' | head -n1)"
  [[ -n "${TOKEN}" ]] || return 1
  return 0
}

resolve_token() {
  if [[ -n "${TOKEN}" ]]; then
    return 0
  fi
  if [[ -n "${TOKEN_FILE}" ]]; then
    read_token_file "${TOKEN_FILE}" \
      || die "Could not read token from --token-file ${TOKEN_FILE}"
    return 0
  fi
  if read_token_file "${INSTALL_DIR}/.ghcr-token"; then
    log "Using token from ${INSTALL_DIR}/.ghcr-token"
    return 0
  fi
  if [[ -n "${GHCR_TOKEN:-}" ]]; then
    TOKEN="${GHCR_TOKEN}"
    return 0
  fi
  if [[ -n "${GITHUB_TOKEN:-}" ]]; then
    TOKEN="${GITHUB_TOKEN}"
    return 0
  fi
  die "No GitHub token found. Pass --token, --token-file, set GHCR_TOKEN, or create ${INSTALL_DIR}/.ghcr-token (chmod 600)"
}

save_token_file() {
  [[ "${SAVE_TOKEN}" -eq 1 ]] || return 0
  [[ -n "${TOKEN}" ]] || return 0
  mkdir -p "${INSTALL_DIR}"
  umask 077
  printf '%s\n' "${TOKEN}" > "${INSTALL_DIR}/.ghcr-token"
  chmod 600 "${INSTALL_DIR}/.ghcr-token"
  log "Saved token to ${INSTALL_DIR}/.ghcr-token (mode 600) — do not commit this file"
}

parse_args() {
  while [[ $# -gt 0 ]]; do
    case "$1" in
      --domain) DOMAIN="${2:-}"; shift 2 ;;
      --email) ACME_EMAIL="${2:-}"; shift 2 ;;
      --repo) REPO="${2:-}"; shift 2 ;;
      --token) TOKEN="${2:-}"; shift 2 ;;
      --token-file) TOKEN_FILE="${2:-}"; shift 2 ;;
      --save-token) SAVE_TOKEN=1; shift ;;
      --local) LOCAL_MODE=1; shift ;;
      --branch) BRANCH="${2:-}"; shift 2 ;;
      --dir) INSTALL_DIR="${2:-}"; shift 2 ;;
      --tag) IMAGE_TAG="${2:-}"; shift 2 ;;
      --registry) IMAGE_REGISTRY="${2:-}"; shift 2 ;;
      --compose-url) COMPOSE_URL="${2:-}"; shift 2 ;;
      --skip-firewall) SKIP_FIREWALL=1; shift ;;
      --no-start) START_STACK=0; shift ;;
      --force-env) FORCE_ENV=1; shift ;;
      -h|--help) usage; exit 0 ;;
      *) die "Unknown option: $1 (use --help)" ;;
    esac
  done

  [[ "${EUID}" -eq 0 ]] || die "Run as root: sudo $0 ..."
  [[ -n "${DOMAIN}" ]] || die "--domain is required"
  [[ -n "${ACME_EMAIL}" ]] || die "--email is required"

  DOMAIN="${DOMAIN#https://}"; DOMAIN="${DOMAIN#http://}"; DOMAIN="${DOMAIN%/}"
  [[ "${DOMAIN}" != *"/"* ]] || die "DOMAIN must be apex only (got: ${DOMAIN})"

  if [[ -z "${COMPOSE_URL}" ]]; then
    [[ -n "${REPO}" ]] || die "--repo OWNER/REPO is required (or pass --compose-url)"
    COMPOSE_URL="https://raw.githubusercontent.com/${REPO}/${BRANCH}/docker-compose.yml"
  fi

  if [[ -z "${IMAGE_REGISTRY}" ]]; then
    [[ -n "${REPO}" ]] || die "--registry is required when --repo is not given"
    local owner="${REPO%%/*}"
    IMAGE_REGISTRY="ghcr.io/${owner,,}"
  fi

  resolve_token
  # If they passed --token once, remember it on disk for updates/reinstalls
  if [[ "${SAVE_TOKEN}" -eq 0 && -n "${TOKEN}" && ! -f "${INSTALL_DIR}/.ghcr-token" ]]; then
    SAVE_TOKEN=1
  fi
  save_token_file
}

install_docker() {
  if command -v docker >/dev/null 2>&1 && docker compose version >/dev/null 2>&1; then
    log "Docker already installed: $(docker --version)"
    return 0
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

  if [[ -n "${SUDO_USER:-}" && "${SUDO_USER}" != "root" ]]; then
    usermod -aG docker "${SUDO_USER}" || true
    log "Added ${SUDO_USER} to the docker group (log out/in for non-sudo docker)"
  fi
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
  ufw --force enable
  ufw status verbose
}

fetch_compose() {
  log "Downloading compose file"
  mkdir -p "${INSTALL_DIR}"
  if ! curl -fsSL -H "Authorization: Bearer ${TOKEN}" \
       -H "Accept: application/vnd.github.raw" \
       "${COMPOSE_URL}" -o "${INSTALL_DIR}/docker-compose.yml"; then
    die "Could not download ${COMPOSE_URL} — check --repo/--branch and that the token has 'repo' scope for private repositories"
  fi
  grep -q '^services:' "${INSTALL_DIR}/docker-compose.yml" \
    || die "Downloaded file is not a compose file (got an HTML error page?)"
  log "Saved ${INSTALL_DIR}/docker-compose.yml"

  if [[ "${LOCAL_MODE}" -eq 1 ]]; then
    local local_url="https://raw.githubusercontent.com/${REPO}/${BRANCH}/docker-compose.local.yml"
    log "Downloading local / VMware overlay"
    if ! curl -fsSL -H "Authorization: Bearer ${TOKEN}" \
         -H "Accept: application/vnd.github.raw" \
         "${local_url}" -o "${INSTALL_DIR}/docker-compose.override.yml"; then
      die "Could not download ${local_url}"
    fi
    grep -q '^services:' "${INSTALL_DIR}/docker-compose.override.yml" \
      || die "Downloaded local overlay is not a compose file"
    log "Saved ${INSTALL_DIR}/docker-compose.override.yml (HTTP lab mode, no Let's Encrypt)"
  else
    # Avoid a stale lab overlay on a production install
    rm -f "${INSTALL_DIR}/docker-compose.override.yml"
  fi

  # Bind-mounted into both APIs; Firebase credentials are never baked into images
  mkdir -p "${INSTALL_DIR}/secrets"
  chmod 700 "${INSTALL_DIR}/secrets"
}

random_secret() { openssl rand -hex 24; }
random_sql_password() {
  local raw
  raw="$(openssl rand -base64 24 | tr -d '/+=' | head -c 20)"
  printf 'Aa1!%s' "${raw}"
}

write_env() {
  local envfile="${INSTALL_DIR}/.env"

  if [[ -f "${envfile}" && "${FORCE_ENV}" -eq 0 ]]; then
    log ".env exists — updating DOMAIN / ACME_EMAIL / image settings only"
    sed -i \
      -e "s|^DOMAIN=.*|DOMAIN=${DOMAIN}|" \
      -e "s|^ACME_EMAIL=.*|ACME_EMAIL=${ACME_EMAIL}|" \
      -e "s|^IMAGE_REGISTRY=.*|IMAGE_REGISTRY=${IMAGE_REGISTRY}|" \
      -e "s|^IMAGE_TAG=.*|IMAGE_TAG=${IMAGE_TAG}|" \
      "${envfile}"
    chmod 600 "${envfile}"
    return 0
  fi

  log "Writing .env with generated secrets"
  cat > "${envfile}" <<EOF
# Generated by install.sh on $(date -u +%Y-%m-%dT%H:%M:%SZ)
DOMAIN=${DOMAIN}
ACME_EMAIL=${ACME_EMAIL}

# Prebuilt images
IMAGE_REGISTRY=${IMAGE_REGISTRY}
IMAGE_TAG=${IMAGE_TAG}

# Database
MSSQL_SA_PASSWORD=$(random_sql_password)

# Secrets (min 32 chars each)
JWT_KEY=$(random_secret)
MOBILE_JWT_KEY=$(random_secret)
TEACHER_JWT_KEY=$(random_secret)
STUDENT_QR_KEY=$(random_secret)
CHAT_INTERNAL_KEY=$(random_secret)

# Optional — can also be set later in Admin -> Integrations
WASENDER_API_TOKEN=
WASENDER_SESSION_API_KEY=
AGORA_APP_ID=
AGORA_APP_CERTIFICATE=

# Push notifications: copy firebase-service-account.json into ./secrets/ first
FIREBASE_ENABLED=false
FIREBASE_PROJECT_ID=

# Optional store links shown on the public website
APP_STORE_URL=
GOOGLE_PLAY_URL=
MOBILE_APP_BANNER_IMAGE=
EOF
  chmod 600 "${envfile}"
}

check_dns() {
  if [[ "${LOCAL_MODE}" -eq 1 ]]; then
    log "Local / VMware mode — skipping public DNS check"
    local vm_ip
    vm_ip="$(hostname -I 2>/dev/null | awk '{print $1}')"
    printf '  VM IP (use this in your Windows hosts file): %s\n' "${vm_ip:-unknown}"
    return 0
  fi

  log "Checking DNS for ${DOMAIN}"
  command -v dig >/dev/null 2>&1 || apt-get install -y dnsutils >/dev/null 2>&1 || true

  local server_ip domain_ip
  server_ip="$(curl -4 -fsS --max-time 10 https://ifconfig.me 2>/dev/null || curl -4 -fsS --max-time 10 https://api.ipify.org 2>/dev/null || true)"
  printf '  Server public IP: %s\n' "${server_ip:-unknown}"

  if command -v dig >/dev/null 2>&1; then
    domain_ip="$(dig +short "${DOMAIN}" A | tail -n1 || true)"
    printf '  %s -> %s\n' "${DOMAIN}" "${domain_ip:-unresolved}"
    printf '  admin.%s -> %s\n' "${DOMAIN}" "$(dig +short "admin.${DOMAIN}" A | tail -n1 || true)"
    printf '  api.%s -> %s\n' "${DOMAIN}" "$(dig +short "api.${DOMAIN}" A | tail -n1 || true)"
    if [[ -z "${domain_ip}" ]]; then
      warn "DNS not resolving yet — HTTPS will fail until A records point here"
    elif [[ -n "${server_ip}" && "${server_ip}" != "${domain_ip}" ]]; then
      warn "DNS (${domain_ip}) does not match this server (${server_ip})"
    fi
  fi
}

registry_login() {
  log "Logging in to ${IMAGE_REGISTRY%%/*}"
  local user="${REPO%%/*}"
  printf '%s' "${TOKEN}" | docker login "${IMAGE_REGISTRY%%/*}" --username "${user:-token}" --password-stdin
}

start_stack() {
  if [[ "${START_STACK}" -eq 0 ]]; then
    log "Skipping pull/start (--no-start)"
    return 0
  fi
  log "Pulling images and starting the stack"
  cd "${INSTALL_DIR}"
  docker compose pull
  docker compose up -d
  docker compose ps
}

print_next_steps() {
  local scheme="https"
  local vm_ip=""
  if [[ "${LOCAL_MODE}" -eq 1 ]]; then
    scheme="http"
    vm_ip="$(hostname -I 2>/dev/null | awk '{print $1}')"
  fi

  cat <<EOF

============================================================
Install finished.

Directory: ${INSTALL_DIR}
Images:    ${IMAGE_REGISTRY}/masged-*:${IMAGE_TAG}
Setup:     ${scheme}://admin.${DOMAIN}/setup
Public:    ${scheme}://${DOMAIN}
Mobile API:${scheme}://api.${DOMAIN}
EOF

  if [[ "${LOCAL_MODE}" -eq 1 ]]; then
    cat <<EOF

LOCAL / VMWARE — add these lines on your Windows PC
(Notepad as Administrator → C:\\Windows\\System32\\drivers\\etc\\hosts):

${vm_ip:-YOUR_VM_IP}  ${DOMAIN}
${vm_ip:-YOUR_VM_IP}  www.${DOMAIN}
${vm_ip:-YOUR_VM_IP}  admin.${DOMAIN}
${vm_ip:-YOUR_VM_IP}  api.${DOMAIN}

Then open ${scheme}://admin.${DOMAIN}/setup in the browser.
VMware network: use Bridged (or NAT + port forward) so the host can reach the VM on port 80.
EOF
  else
    cat <<EOF

Next:
  1. Wait for SQL to become healthy: cd ${INSTALL_DIR} && docker compose ps
  2. Open ${scheme}://admin.${DOMAIN}/setup and create the super admin
  3. Configure Admin -> Integrations (Wasender / Agora) if needed
EOF
  fi

  cat <<EOF

PUSH NOTIFICATIONS ARE OFF.
  Firebase credentials are never shipped inside the images. To enable:
    1. scp your firebase-service-account.json to ${INSTALL_DIR}/secrets/
    2. In ${INSTALL_DIR}/.env set FIREBASE_ENABLED=true and FIREBASE_PROJECT_ID=<your-project-id>
    3. cd ${INSTALL_DIR} && docker compose up -d
  Until then the APIs run normally and simply skip sending push messages.

Update to a newer release:
  cd ${INSTALL_DIR} && docker compose pull && docker compose up -d

Secrets live in ${INSTALL_DIR}/.env — back it up, keep it private.
============================================================
EOF
}

main() {
  parse_args "$@"
  export DEBIAN_FRONTEND=noninteractive
  apt-get update -y
  apt-get install -y ca-certificates curl openssl

  install_docker
  setup_firewall
  fetch_compose
  write_env
  check_dns
  registry_login
  start_stack
  print_next_steps
}

main "$@"
