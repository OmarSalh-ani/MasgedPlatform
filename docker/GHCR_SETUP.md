# One-time setup: publishing images to GHCR

You do this **once**. After it is done, every new customer server is a single command.

GHCR (GitHub Container Registry) is a private storage for your Docker images, included free with GitHub. GitHub builds the images for you, stores them, and each customer server downloads them.

---

## Step 1 — Put the project on GitHub

If the project is not a Git repository yet, from the project root:

```powershell
git init
git add .
git commit -m "Initial commit"
```

Create an **empty private repository** on <https://github.com/new> (do not add a README), then:

```powershell
git remote add origin https://github.com/YOUR_ORG/YOUR_REPO.git
git branch -M main
git push -u origin main
```

A root `.gitignore` is already in place. It keeps these **out** of the repository:

| Excluded | Why |
|----------|-----|
| `.env` | Deployment secrets |
| `**/firebase-service-account.json` | Google private keys — supplied per server, see `DOCKER_GUIDE.md` §7b |
| `**/appsettings.Development.json` | Your real local DB / Wasender / Agora credentials |
| `AdminAPI/_buildcheck/` | 56 MB of build output |
| `AdminAPI/Uploads/`, `FilesManager/`, `Logs/` | Runtime data, lives in Docker volumes |
| `ParentApp/` | Flutter app has its own repo |
| `node_modules/`, `bin/`, `obj/`, `dist/` | Build artifacts |

The committed `appsettings.json` files contain `CHANGE_ME` placeholders. Real values come from Compose environment variables in production, and from `appsettings.Development.json` on your machine.

### Credential rotation (do this soon)

The database password and JWT keys that were previously hardcoded in `appsettings.json` are still live on your existing server. They were never pushed to GitHub, but they have been sitting in plain text in the working folder, so rotate them when convenient:

- SQL Server `sa` password
- `Jwt__Key`, `TeacherJwt__Key`, `StudentQr__EncryptionKey`, `Chat__InternalBroadcastKey`
- The Wasender API token and Agora certificate in `appsettings.Development.json`

New customer servers are unaffected — `install.sh` generates fresh random secrets for each one.

---

## Step 2 — Let the workflow build the images

The file `.github/workflows/publish-images.yml` is already in the repo. As soon as you push to `main`, GitHub Actions builds four images and pushes them to GHCR:

| Image | From |
|-------|------|
| `masged-admin-api` | `AdminAPI/Dockerfile` |
| `masged-mobile-api` | `MasgedParentMobileAPI/Dockerfile` |
| `masged-admin-ui` | `AdminPanelUI/Dockerfile` |
| `masged-public-ui` | `PublicWebsiteUI/Dockerfile` |

Watch it run: repo → **Actions** tab → **Publish Docker images**. The first run takes 10–20 minutes; later runs are much faster because of caching.

No secrets to configure — the workflow uses the built-in `GITHUB_TOKEN`.

When it finishes, the images appear at repo → **Packages** (right sidebar) as:

```text
ghcr.io/your-org/masged-admin-api:latest
ghcr.io/your-org/masged-mobile-api:latest
ghcr.io/your-org/masged-admin-ui:latest
ghcr.io/your-org/masged-public-ui:latest
```

The owner name is always lowercase in image paths.

---

## Step 3 — Create a token for customer servers

Each server needs a read-only token to pull the private images.

1. Go to <https://github.com/settings/tokens> → **Tokens (classic)** → **Generate new token (classic)**
2. Name it something like `masged-server-pull`
3. Expiration: **No expiration** (or set a reminder to rotate)
4. Tick these scopes:
   - `read:packages` — required to pull images
   - `repo` — required only because the repo is private, so the server can download `docker-compose.yml`
5. Generate, then **copy the token** (`ghp_…`). GitHub shows it only once.

Store it in your password manager. The same token can be reused for every customer server.

---

## Step 4 — Tag a release (optional but recommended)

Deploying `latest` means customers get whatever was last pushed to `main`. To pin customers to a tested version:

```powershell
git tag v1.0.0
git push origin v1.0.0
```

This publishes `…:v1.0.0` alongside `latest`. Then install customers with `--tag v1.0.0`.

---

## Step 5 — Install a customer server

On a fresh Ubuntu 22.04/24.04 server, with DNS already pointing at it:

```bash
TOKEN=ghp_your_token_here

curl -fsSL -H "Authorization: Bearer $TOKEN" \
  https://raw.githubusercontent.com/YOUR_ORG/YOUR_REPO/main/docker/scripts/install.sh \
  -o install.sh

sudo bash install.sh \
  --domain customer.com \
  --email admin@customer.com \
  --repo YOUR_ORG/YOUR_REPO \
  --token "$TOKEN"
```

Takes roughly 2–5 minutes, mostly image download. Then open `https://admin.customer.com/setup`.

---

## Shipping updates to customers

1. Push your changes to `main` (or tag a release) — GitHub rebuilds the images
2. On each customer server:

```bash
cd /opt/masged
docker compose pull
docker compose up -d
```

Data in the Docker volumes (database, uploads, certificates) is untouched.

---

## Why one image works for every customer

The SPAs no longer bake the domain into the JavaScript bundle. At container startup, `docker/nginx/40-runtime-config.sh` writes `/config.js` from the container's environment:

```js
window.__APP_CONFIG__ = { apiBaseUrl: "https://admin.customer.com/api", ... };
```

`src/lib/constants.ts` reads that first, then falls back to Vite env vars for local development. So changing `DOMAIN` in `.env` and running `docker compose up -d` is enough — no rebuild.

---

## Troubleshooting

| Problem | Cause / fix |
|---------|-------------|
| `denied` when pulling | Token missing `read:packages`, or expired |
| Install script downloads an HTML error page | Token missing `repo` scope on a private repo, or wrong `--repo`/`--branch` |
| Workflow fails with `permission_denied: write_package` | Repo → Settings → Actions → General → Workflow permissions → **Read and write permissions** |
| Images not visible under Packages | Check the Actions run finished successfully |
| UI calls the wrong domain | Check `docker compose exec admin-ui cat /usr/share/nginx/html/config.js` |
