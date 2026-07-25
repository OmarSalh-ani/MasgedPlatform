# Docker Guide — Deploy Masged Parent App on Ubuntu

Complete guide to build, configure, and run this project with Docker on an Ubuntu server for a new customer (white-label).

---

## 1. What you get

| Host | What runs |
|------|-----------|
| `https://customer.com` | Public website (PublicWebsiteUI) |
| `https://admin.customer.com` | Admin panel (AdminPanelUI) |
| `https://admin.customer.com/api` | Admin API (AdminAPI) |
| `https://admin.customer.com/uploads` | Uploaded files / logos |
| `https://api.customer.com` | Mobile API (MasgedParentMobileAPI) |

Also included:

- **SQL Server Express** (Docker)
- **Traefik** reverse proxy + **Let’s Encrypt** SSL (automatic HTTPS)
- First-time setup wizard (branding + super admin)
- Integrations page (WhatsApp / Agora)

Replace `customer.com` with the real domain everywhere.

---

## 2. Server requirements

- Ubuntu **22.04** or **24.04** (recommended)
- Public IP address
- Ports **80** and **443** open (firewall + cloud security group)
- At least **4 GB RAM** (8 GB better — SQL Server needs memory)
- Disk: **40 GB+** free
- Root or a user with `sudo`

---

## 3. Install a customer server (prebuilt images)

The server does **not** need the source code. It downloads ready-made images from GHCR, so a new customer takes about **2–5 minutes** instead of a 30-minute build.

One-time setup of the image registry is described in [`docker/GHCR_SETUP.md`](docker/GHCR_SETUP.md). Do that once, then repeat this section per customer.

### Before you run it

Point these **A records** to the server’s public IP (Let’s Encrypt needs this):

| Type | Name | Value |
|------|------|--------|
| A | `@` (or `customer.com`) | `YOUR_SERVER_IP` |
| A | `www` | `YOUR_SERVER_IP` |
| A | `admin` | `YOUR_SERVER_IP` |
| A | `api` | `YOUR_SERVER_IP` |

Check DNS:

```bash
dig +short customer.com
dig +short admin.customer.com
dig +short api.customer.com
```

### Install

```bash
# Put the PAT on the server once (chmod 600). Never commit this file.
sudo mkdir -p /opt/masged
echo 'ghp_your_token_here' | sudo tee /opt/masged/.ghcr-token >/dev/null
sudo chmod 600 /opt/masged/.ghcr-token

TOKEN=$(sudo cat /opt/masged/.ghcr-token)
curl -fsSL -H "Authorization: Bearer $TOKEN" \
  https://raw.githubusercontent.com/OmarSalh-ani/MasgedPlatform/main/docker/scripts/install.sh \
  -o install.sh

sudo bash install.sh \
  --domain customer.com \
  --email admin@customer.com \
  --repo OmarSalh-ani/MasgedPlatform
```

Token resolution: `--token` → `--token-file` → `/opt/masged/.ghcr-token` → `$GHCR_TOKEN` / `$GITHUB_TOKEN`.
Passing `--token` the first time also writes `/opt/masged/.ghcr-token` for later updates.

### What the script does

1. Installs **Docker Engine + Compose** (skips if already present)
2. Configures **UFW** (OpenSSH + ports 80/443)
3. Downloads **`docker-compose.yml`** into `/opt/masged` — nothing else is needed
4. Writes **`.env`** with strong random SQL/JWT secrets
5. Checks DNS and warns if it does not point here
6. Logs in to GHCR, **pulls** the images, and starts the stack

### Useful flags

| Flag | Meaning |
|------|---------|
| `--tag v1.0.0` | Deploy a pinned release instead of `latest` |
| `--local` | VMware/lab: HTTP only, no Let's Encrypt / public DNS |
| `--token` / `--token-file` | GitHub PAT (or use `/opt/masged/.ghcr-token`) |
| `--dir /opt/masged` | Install directory |
| `--branch main` | Branch to fetch the compose file from |
| `--registry ghcr.io/org` | Override the image registry path |
| `--force-env` | Overwrite `.env` and regenerate all secrets |
| `--skip-firewall` | Do not touch UFW |
| `--no-start` | Set up files only; do not pull/start |

Optional integrations (Wasender / Agora) can stay empty — set them later in Admin → Integrations, or edit `.env` before starting.

Then:

```bash
cd /opt/masged
docker compose ps
docker compose logs -f
```

Open `https://admin.customer.com/setup`.

### Updating a customer later

```bash
cd /opt/masged
docker compose pull
docker compose up -d
```

Volumes (database, uploads, certificates) are preserved.

---

## 4. Alternative: build from source on the server

Use this if you are not publishing images yet. It needs the full repo on the server and takes 10–30 minutes.

```bash
cd /opt/masged
chmod +x docker/scripts/bootstrap.sh
sudo ./docker/scripts/bootstrap.sh \
  --domain customer.com \
  --email admin@customer.com
# add --repo <git-url> to clone first
```

`bootstrap.sh` installs Docker, sets UFW, generates `.env`, then runs the build overlay:

```bash
docker compose -f docker-compose.yml -f docker-compose.build.yml up -d --build
```

---

## 5. Manual steps (fallback)

Use this only if you prefer not to run any script.

### 5.1 Install Docker on Ubuntu

```bash
sudo apt update
sudo apt install -y ca-certificates curl gnupg

sudo install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
sudo chmod a+r /etc/apt/keyrings/docker.gpg

echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
  $(. /etc/os-release && echo \"$VERSION_CODENAME\") stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

sudo apt update
sudo apt install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

sudo usermod -aG docker $USER
```

Log out and back in (or reboot), then verify:

```bash
docker --version
docker compose version
```

### 5.2 Get the project on the server

**Git clone:**

```bash
sudo mkdir -p /opt/masged
sudo chown $USER:$USER /opt/masged
cd /opt/masged
git clone <YOUR_REPO_URL> .
```

**Or upload** the project (zip/scp/rsync) to `/opt/masged`. You need at least:

- `docker-compose.yml`, `docker-compose.build.yml`, `.env.example`, `docker/`
- `AdminAPI/`, `MasgedParentMobileAPI/`, `AdminPanelUI/`, `PublicWebsiteUI/`, `Masged.WhatsApp/`

With prebuilt images you only need `docker-compose.yml` and `.env`.

### 5.3 Create `.env`

```bash
cd /opt/masged
./docker/scripts/generate-env.sh --domain customer.com --email admin@customer.com
# or: cp .env.example .env && nano .env
```

Required if editing by hand:

```env
DOMAIN=customer.com
ACME_EMAIL=admin@customer.com
IMAGE_REGISTRY=ghcr.io/your-github-org
IMAGE_TAG=latest
MSSQL_SA_PASSWORD=ChangeMe_StrongPass123!
JWT_KEY=AdminAPI_ChangeThisSecretKey_Min32Chars!
MOBILE_JWT_KEY=MobileAPI_ChangeThisSecretKey_Min32Chars!
TEACHER_JWT_KEY=TeacherAPI_ChangeThisSecretKey_Min32Chars!
STUDENT_QR_KEY=StudentQr_ChangeThisSecretKey_Min32Chars!
CHAT_INTERNAL_KEY=ChatInternal_ChangeThisKey_Min32Chars!
```

Notes:

- `DOMAIN` = apex only — **no** `https://`, **no** trailing slash
- Keep `.env` private (do not commit it)
- `IMAGE_REGISTRY` / `IMAGE_TAG` are ignored when building from source

### 5.4 Firewall

```bash
sudo ufw allow OpenSSH
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw enable
sudo ufw status
```

### 5.5 Start

Prebuilt images:

```bash
cd /opt/masged
docker login ghcr.io   # username = GitHub user, password = PAT with read:packages
docker compose pull
docker compose up -d
```

Or build from source:

```bash
docker compose -f docker-compose.yml -f docker-compose.build.yml up -d --build
```

Then watch:

```bash
docker compose ps
docker compose logs -f traefik
docker compose logs -f sql
docker compose logs -f admin-api
```

Wait until `sql` is healthy and `admin-api` / `mobile-api` are running.

---

## 6. Architecture (what Compose starts)

```
Internet :80/:443
        │
     Traefik (+ Let's Encrypt)
        │
        ├── customer.com      → public-ui (nginx)
        ├── admin.customer.com → admin-ui (nginx)
        │                      → admin-api (/api, /uploads, …)
        ├── api.customer.com   → mobile-api
        │
     sql (SQL Express)  ←── admin-api + mobile-api
```

Data that persists across restarts (Docker volumes):

- `sql-data` — database
- `admin-uploads` — logos / images
- `admin-files` — FilesManager
- `traefik-letsencrypt` — SSL certificates
- `mobile-uploads`

---

## 7. First-time setup (required)

Open:

```text
https://admin.YOUR_DOMAIN/setup
```

Fill in:

1. **Company / mosque name**
2. **Primary color** (global theme)
3. **Logo**
4. **Domain** (pre-filled from `.env`, read-only)
5. **Super admin**
   - Name
   - Email (= login username)
   - Password (min 6 characters) + confirm
6. Optional: App Store / Google Play URLs (parent + teacher)

Submit → you are redirected to `/login`.

Sign in with the **email + password** you just created.

Until setup is finished, the admin site forces `/setup`.

---

## 7b. Push notifications (Firebase) — manual step

Firebase service-account keys are **never** committed to Git or baked into the images, so a fresh install has push notifications **off**. The APIs run normally and simply skip sending push messages until you enable it.

To turn it on, per server:

```bash
# 1. Copy the key file to the server (run this from your PC)
scp firebase-service-account.json root@SERVER_IP:/opt/masged/secrets/

# 2. On the server, enable it in .env
cd /opt/masged
nano .env
#   FIREBASE_ENABLED=true
#   FIREBASE_PROJECT_ID=your-firebase-project-id

# 3. Apply
docker compose up -d
```

The `secrets/` folder is bind-mounted read-only into both APIs at `/app/secrets`. Nothing else is needed — the path is already set by Compose.

Verify:

```bash
docker compose exec admin-api ls -l /app/secrets
docker compose logs admin-api | grep -i firebase
```

If the file is missing or the project ID is wrong, sending is skipped and logged as `FirebaseNotInitialized` — the app does not crash.

---

## 8. After login — Integrations

Open **التكاملات** (`https://admin.YOUR_DOMAIN/integrations`):

| Field | Purpose |
|-------|---------|
| Wasender Api Token | WhatsApp API token |
| Wasender Session API Key | Session key (or connect via WhatsApp QR later) |
| Agora App Id | Video calls |
| Agora App Certificate | Video calls (server secret) |

You can also put these in `.env` before `docker compose up`. Values saved in Integrations are stored in the DB and override env when set.

**WhatsApp note:** Token alone is not enough. You usually still need a connected session (Admin → WhatsApp QR / Check Health).

---

## 9. Verify everything works

```bash
# Containers
docker compose ps

# HTTPS (should show valid cert after Let's Encrypt succeeds)
curl -I https://admin.YOUR_DOMAIN
curl -I https://YOUR_DOMAIN
curl -I https://api.YOUR_DOMAIN
```

In the browser:

1. Public site: `https://YOUR_DOMAIN`
2. Admin login: `https://admin.YOUR_DOMAIN/login`
3. Logo / color appear after setup
4. Mobile app should use `https://api.YOUR_DOMAIN` (see section 15)

If SSL fails, check Traefik logs and DNS:

```bash
docker compose logs traefik | tail -100
```

---

## 10. Common commands

```bash
cd /opt/masged

# Start / stop
docker compose up -d
docker compose down

# Update to the latest published images
docker compose pull && docker compose up -d

# Apply a .env change (including a new DOMAIN) — no rebuild needed
docker compose up -d

# Rebuild from source instead of pulling
docker compose -f docker-compose.yml -f docker-compose.build.yml up -d --build

# Restart one service
docker compose restart admin-api
docker compose restart mobile-api

# Logs
docker compose logs -f admin-api
docker compose logs --tail=200 traefik

# Enter SQL container (advanced)
docker exec -it masged-sql bash
```

---

## 11. Database notes (important)

On first start, AdminAPI runs **EnsureCreated** (empty schema + white-label tables).

That is **enough for setup + login**, but:

- **Quran reference data** (HolyQuran / Surah / Ayah) is **not** seeded automatically.
- Memorization / plans / Quran features need either:
  - a restored SQL backup from a known-good database, or
  - a separate seed/import of Quran tables.

### Recommended for a real customer

1. Start Compose once (or create SQL only).
2. Restore a prepared `.bak` / SQL dump into `NewMasgedTeacherAPIDB` (with Quran data, **without** another mosque’s branding if you want a clean start).
3. Or run white-label scripts on an existing DB:
   - `AdminAPI/Scripts/AddMasgedSettingsWhiteLabel.sql`
   - `AdminAPI/Scripts/CreateIntegrationSettings.sql`

If you restore a DB that already has a mosque name, `SetupCompleted` may already be `1` — setup wizard will be skipped. Create an admin teacher manually or use an existing admin account.

Default database name: **`NewMasgedTeacherAPIDB`**  
SQL login from other containers: **`sa`** / password from `MSSQL_SA_PASSWORD`  
SQL host inside Docker network: **`sql`**

---

## 12. Mobile app (Flutter) for this customer

Docker does **not** publish the Flutter app to stores. After the server is live:

1. Generate icons from the company logo (on a Windows/dev machine with Flutter):

```powershell
cd ParentApp
.\tool\generate_store_icons.ps1 -LogoPath C:\path\to\logo.png
```

2. See `google-play/templates/icon-assets.md` for Play / App Store sizes.

3. Copy `ParentApp/codemagic.yaml.example` and set:

- Bundle ID
- App Store Connect integration name
- Dart defines, for example:

```text
--dart-define=API_BASE_URL=https://api.customer.com
--dart-define=MEDIA_BASE_URL=https://admin.customer.com/
```

Without dart-defines, the app may still point at the old mosque hosts.

---

## 13. Changing domain later

1. Update DNS for the new domain.
2. Edit `.env` → `DOMAIN=newdomain.com`
3. Recreate the containers — **no rebuild**, the UIs read their URLs at startup:

```bash
docker compose up -d
```

4. Update store URLs / integrations if needed.
5. Rebuild Flutter with the new `API_BASE_URL`.

---

## 14. Backup

```bash
# List volumes
docker volume ls | grep masged

# Example: backup SQL volume (stop SQL first for consistency if possible)
docker compose stop sql
# then copy/back up the Docker volume or use sqlcmd / backup tools inside the container
docker compose start sql
```

Also back up:

- `.env`
- uploaded files volume (`admin-uploads`)
- Traefik certs volume (optional; certs can be reissued)

---

## 15. Troubleshooting

| Problem | What to check |
|---------|----------------|
| Can’t get SSL certificate | DNS A records, ports 80/443, Traefik logs, `ACME_EMAIL` |
| `admin` shows setup forever | Setup not completed; or DB missing / API down |
| Login fails after setup | Use **email** as username; check admin-api logs |
| Site loads but no API | `docker compose logs admin-api`; Traefik rules for `/api` |
| UI still calls old domain | `docker compose exec admin-ui cat /usr/share/nginx/html/config.js`; hard-refresh the browser |
| SQL won’t start | Weak `MSSQL_SA_PASSWORD`; check `docker compose logs sql` |
| Out of memory | Increase RAM; SQL Express is heavy |
| WhatsApp not sending | Token + Session key + connected QR session |
| Video calls fail | Agora Id + Certificate in Integrations or `.env` |
| Push notifications not arriving | `secrets/firebase-service-account.json` present? `FIREBASE_ENABLED=true`? See section 7b |
| `denied` when pulling images | `docker login ghcr.io` with a PAT that has `read:packages` |

Reset only apps (keeps DB volume):

```bash
docker compose down
docker compose up -d
```

**Dangerous — deletes database volume:**

```bash
docker compose down -v
```

Only use `-v` if you intentionally want a fresh empty SQL volume.

---

## 16. Checklist (new customer)

- [ ] Images published to GHCR (once — see `docker/GHCR_SETUP.md`)  
- [ ] DNS: `@`, `www`, `admin`, `api` → server IP  
- [ ] `sudo bash install.sh --domain … --email … --repo …` succeeded (token via `.ghcr-token`)  
- [ ] `https://admin.DOMAIN/setup` completed (branding + super admin)  
- [ ] Login works  
- [ ] Integrations: Wasender + Agora (if needed)  
- [ ] Public site shows name / logo / color  
- [ ] Quran data restored/seeded if needed for app features  
- [ ] Flutter built with customer `API_BASE_URL` / icons / Codemagic  

---

## 17. Related files

| File | Purpose |
|------|---------|
| `docker/scripts/install.sh` | Plug-and-play install from prebuilt images |
| `docker/GHCR_SETUP.md` | One-time GitHub / GHCR publishing setup |
| `.github/workflows/publish-images.yml` | Builds and pushes the four images |
| `docker/scripts/bootstrap.sh` | Install that builds from source on the server |
| `docker/scripts/generate-env.sh` | Create `.env` with random secrets only |
| `docker-compose.yml` | All services (prebuilt images) |
| `docker-compose.build.yml` | Overlay that builds images from source |
| `.env.example` | Template for secrets |
| `docker/nginx/spa.conf` | SPA nginx config |
| `docker/nginx/40-runtime-config.sh` | Writes `/config.js` per deployment |
| `docker/README.md` | Short summary |
| `AdminAPI/Dockerfile` | Admin API image |
| `MasgedParentMobileAPI/Dockerfile` | Mobile API image |
| `AdminPanelUI/Dockerfile` | Admin UI image |
| `PublicWebsiteUI/Dockerfile` | Public UI image |
| `ParentApp/codemagic.yaml.example` | iOS white-label CI template |
| `ParentApp/tool/generate_store_icons.ps1` | Launcher icons from logo |

---

## 18. Quick copy-paste (experienced operators)

```bash
# On Ubuntu server (DNS A records already pointed here)
sudo mkdir -p /opt/masged
echo 'ghp_your_token' | sudo tee /opt/masged/.ghcr-token >/dev/null && sudo chmod 600 /opt/masged/.ghcr-token
TOKEN=$(sudo cat /opt/masged/.ghcr-token)
curl -fsSL -H "Authorization: Bearer $TOKEN" \
  https://raw.githubusercontent.com/OmarSalh-ani/MasgedPlatform/main/docker/scripts/install.sh -o install.sh
sudo bash install.sh --domain customer.com --email admin@customer.com \
  --repo OmarSalh-ani/MasgedPlatform
# Wait for healthy SQL + APIs
# Browser: https://admin.customer.com/setup
# Then: https://admin.customer.com/login
# Then: https://admin.customer.com/integrations
```
