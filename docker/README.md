# Docker deployment (Ubuntu)

White-label stack with SQL Server Express, both .NET APIs, Admin + Public SPAs, and Traefik + Let's Encrypt.

## Hostnames

Set `DOMAIN=customer.com` in `.env`. DNS A records must point to the server:

| Host | Service |
|------|---------|
| `customer.com` / `www.customer.com` | Public website |
| `admin.customer.com` | Admin UI + AdminAPI (`/api`, `/uploads`, …) |
| `api.customer.com` | MasgedParentMobileAPI |

## Prerequisites

- Ubuntu 22.04+ with Docker Engine + Docker Compose plugin
- Ports **80** and **443** open
- DNS already pointing at the server (required for Let's Encrypt)

## Quick start (prebuilt images)

Publish images once ([`GHCR_SETUP.md`](GHCR_SETUP.md)), then per customer on a fresh Ubuntu server with DNS already pointing at it:

```bash
TOKEN=ghp_your_token_here
curl -fsSL -H "Authorization: Bearer $TOKEN" \
  https://raw.githubusercontent.com/YOUR_ORG/YOUR_REPO/main/docker/scripts/install.sh -o install.sh
sudo bash install.sh --domain customer.com --email admin@customer.com \
  --repo YOUR_ORG/YOUR_REPO --token "$TOKEN"
```

Installs Docker, opens UFW (22/80/443), writes `.env` with generated secrets, pulls the images, and starts everything. The server needs only `docker-compose.yml` + `.env` — no source code.

Update later:

```bash
cd /opt/masged && docker compose pull && docker compose up -d
```

## Build from source instead

```bash
sudo ./docker/scripts/bootstrap.sh --domain customer.com --email admin@customer.com
# equivalent to:
docker compose -f docker-compose.yml -f docker-compose.build.yml up -d --build
```

`.env` only:

```bash
./docker/scripts/generate-env.sh --domain customer.com --email admin@customer.com
```

See [`DOCKER_GUIDE.md`](../DOCKER_GUIDE.md) for flags and the manual fallback.

Watch logs until healthy:

```bash
docker compose ps
docker compose logs -f admin-api
```

Open first-time setup:

`https://admin.YOUR_DOMAIN/setup`

Enter:

- Company / mosque name
- Global primary color
- Logo
- Domain (pre-filled from `.env`, read-only)
- Optional App Store / Google Play URLs (parent + teacher)
- Super admin name, email, and password (creates the first login account)

After save you are redirected to login. Use the email/password you entered to sign in.

Then open **التكاملات** (`/integrations`) to set or update Wasender + Agora keys (or put them in `.env` before compose up).

## Environment variables

See [`.env.example`](../.env.example). Important:

- `DOMAIN` — apex domain only (no `https://`); changing it needs only `docker compose up -d`
- `IMAGE_REGISTRY` / `IMAGE_TAG` — which prebuilt images to run
- `ACME_EMAIL` — Let's Encrypt account email
- `MSSQL_SA_PASSWORD` — strong SQL SA password
- JWT / QR / Chat keys — change every secret before production
- `WASENDER_API_TOKEN`, `WASENDER_SESSION_API_KEY` — WhatsApp (Wasender)
- `AGORA_APP_ID`, `AGORA_APP_CERTIFICATE` — video calls

DB overrides from Admin → Integrations take precedence over env when set. MobileAPI refreshes Agora from DB every ~30s.

## Architecture

- **traefik** — reverse proxy + automatic TLS
- **sql** — `MSSQL_PID=Express`
- **admin-api** — creates DB schema on first start (`Deployment__EnsureDatabase=true`)
- **mobile-api** — shared SQL database
- **admin-ui** / **public-ui** — nginx SPA images; domain-specific URLs are written to `/config.js` at container startup, so the same image works for every customer

## Mobile / store publish

1. Download the customer logo from Admin uploads (or export from setup).
2. Generate Flutter icons:

```powershell
cd ParentApp
.\tool\generate_store_icons.ps1 -LogoPath C:\path\to\logo.png
```

3. Follow [`google-play/templates/icon-assets.md`](../google-play/templates/icon-assets.md) for Play/App Store listing sizes.
4. Copy [`ParentApp/codemagic.yaml.example`](../ParentApp/codemagic.yaml.example) → configure `BUNDLE_ID`, ASC integration, and `API_BASE_URL=https://api.YOUR_DOMAIN` in Codemagic (do not commit customer secrets).

## Useful commands

```bash
docker compose pull
docker compose up -d --build
docker compose restart admin-api
docker compose down
```

Data persists in Docker volumes (`sql-data`, `admin-uploads`, `traefik-letsencrypt`, …).

## Existing non-Docker databases

If you run AdminAPI outside Docker against an existing SQL database, apply:

[`AdminAPI/Scripts/AddMasgedSettingsWhiteLabel.sql`](../AdminAPI/Scripts/AddMasgedSettingsWhiteLabel.sql)

Existing rows with a mosque name are marked `SetupCompleted = 1` so the setup wizard is skipped.

Also ensure `IntegrationSettings` table exists (created automatically when `Deployment__EnsureDatabase=true`, or by AdminAPI startup bootstrap).

## Out of scope

- Changing Traefik hostnames from the setup wizard (edit `.env` + `docker compose up -d`)
