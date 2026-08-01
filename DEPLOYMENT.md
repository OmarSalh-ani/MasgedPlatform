# Deployment Guide — new customer (Windows Server / IIS)

How to put the platform on a customer's server.

The day-to-day parts of this guide (setup wizard, integrations, Quran restore, Firebase, mobile
builds, domain change, backups, troubleshooting, checklist) are also readable inside the admin panel
at **دليل التشغيل** — `https://admin.customer.com/guide`.

**Starting from a copy of an existing customer's project?** Run the scrubber first — it deletes the
previous customer's Firebase credentials, signing keys, uploads and stale build outputs, and resets
the UI `.env` files to template values:

```powershell
.\new-customer-reset.ps1          # preview with -DryRun first if unsure
```

---

## 1. What gets deployed


| Component             | Type       | Site / host                                                       |
| --------------------- | ---------- | ----------------------------------------------------------------- |
| AdminAPI              | .NET 8 API | `admin.customer.com` (serves `/api`, `/uploads`, `/FilesManager`) |
| AdminPanelUI          | Static SPA | `admin.customer.com`                                              |
| MasgedParentMobileAPI | .NET 8 API | `api.customer.com`                                                |
| PublicWebsiteUI       | Static SPA | `customer.com` + `www.customer.com`                               |
| SQL Server            | Database   | Local or separate DB server                                       |


---

## 2. Server requirements

- Windows Server 2019/2022 with **IIS**
- **ASP.NET Core Hosting Bundle 8.0** ([download](https://dotnet.microsoft.com/download/dotnet/8.0)) — installs the IIS module. Restart IIS after installing.
- **SQL Server** 2019+ (Express is fine)
- **URL Rewrite Module** for IIS — required so the SPAs handle client-side routes
- Public IP, ports **80** and **443** open
- SSL certificate (Let's Encrypt via win-acme, or a purchased cert)

---



## 3. DNS

Point these A records at the server IP:


| Type | Name               |
| ---- | ------------------ |
| A    | `@` (customer.com) |
| A    | `www`              |
| A    | `admin`            |
| A    | `api`              |


---



## 4. Build the packages

On your dev machine, from the project root:

```powershell
.\publish-all.ps1
```

This produces zips in `.\publish`:

- `AdminAPI.zip`
- `AdminPanelUI.zip`
- `MasgedParentMobileAPI.zip`
- `PublicWebsiteUI.zip`

### What the API packages deliberately exclude

Server-owned files are stripped from the API zips, so extracting an update over a live site can never overwrite a customer's configuration, credentials, or data:

| Excluded | Why |
| --- | --- |
| `appsettings.json` | Shipped as **`appsettings.example.json`** instead — a fresh install copies it to `appsettings.json` and fills it in |
| `appsettings.Development.json`, `appsettings.Production.json` | Your dev machine's settings, never a customer's |
| `firebase-service-account.json` | Real push credentials — must not travel to another customer |
| `Uploads`, `FilesManager`, `Logs` | Live customer data |

The rest of the app (`wwwroot`, `static/qcf-fonts`, `countires.json`, the DLLs) is packaged normally.



### Point the UIs at the customer domain first

Vite bakes API URLs into the bundle at build time, so set these **before** building.

`AdminPanelUI\.env`:

```env
VITE_API_BASE_URL=https://admin.customer.com/api
VITE_UPLOADS_BASE_URL=https://admin.customer.com
VITE_PUBLIC_SITE_URL=https://customer.com
```

`PublicWebsiteUI\.env`:

```env
VITE_API_BASE_URL=https://admin.customer.com/api
VITE_UPLOADS_BASE_URL=https://admin.customer.com
```

Changing the domain later means rebuilding both UIs.

---



## 5. Create the database

Create an empty database (default name `NewMasgedTeacherAPIDB`) and a SQL login the APIs will use.

AdminAPI creates the schema itself on first start when `Deployment:EnsureDatabase` is `true` (see below).

### Quran reference data is not seeded automatically

Memorization, revision and plan features depend on reference tables (surahs, ayahs, pages, plan levels) that `EnsureCreated` does **not** populate. An empty database gives you a working admin panel with empty Quran screens.

The practical path is to restore a known-good backup instead of creating an empty database:

```sql
RESTORE DATABASE NewMasgedTeacherAPIDB
FROM DISK = 'C:\Backups\seed.bak'
WITH MOVE 'NewMasgedTeacherAPIDB' TO 'C:\SQLData\NewMasgedTeacherAPIDB.mdf',
     MOVE 'NewMasgedTeacherAPIDB_log' TO 'C:\SQLData\NewMasgedTeacherAPIDB_log.ldf',
     REPLACE;
```

Then clear the previous customer's operational data (students, teachers, circles, attendance, plans, messages) while keeping the Quran reference tables, and reset the branding row so the new customer gets the wizard:

```sql
UPDATE MasgedSettings SET SetupCompleted = 0;
```

If `MasgedSettings` has no row at all, AdminAPI inserts a default one on startup.

---



## 6. Deploy AdminAPI

1. Create an IIS site or app for `admin.customer.com`, pointing at the extracted `AdminAPI.zip`
2. Application pool: **No Managed Code**, and set it to start automatically
3. Grant the app pool identity **modify** permission on `Uploads`, `FilesManager`, and `Logs`
4. Copy `appsettings.example.json` to `appsettings.json` in the deployed folder, then edit it:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=NewMasgedTeacherAPIDB;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "a-unique-random-string-of-at-least-32-characters"
  },
  "Cors": {
    "Origins": [ "https://customer.com", "https://www.customer.com", "https://admin.customer.com" ]
  },
  "PublicSite": {
    "BaseUrl": "https://customer.com"
  },
  "Deployment": {
    "Domain": "customer.com",
    "EnsureDatabase": true
  },
  "StudentQr": {
    "EncryptionKey": "another-unique-random-string-min-32-chars"
  }
}
```

`Deployment:Domain` prefills the domain field in the setup wizard; the operator can still change it there or later in **الإعدادات**. `Deployment:EnsureDatabase` must be `true` on a fresh install so the schema and white-label tables are created.

**Generate a unique secret per customer.** Never reuse keys between customers:

```powershell
[Convert]::ToHexString((1..24 | ForEach-Object { Get-Random -Max 256 }))
```

---



## 7. Deploy MasgedParentMobileAPI

Same process, on `api.customer.com`: copy `appsettings.example.json` to `appsettings.json`, then set the same connection string plus its own unique secrets:

- `ApiSettings:MediaBaseUrl` → `https://admin.customer.com/`
- `ApiSettings:Jwt:Key`
- `TeacherJwt:Key`
- `Chat:InternalBroadcastKey`
- `StudentQr:EncryptionKey`

---



## 8. Deploy the two SPAs

Extract `AdminPanelUI.zip` and `PublicWebsiteUI.zip` into their IIS sites.

Each needs a `web.config` so client-side routing works and unknown paths fall back to `index.html`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <rewrite>
      <rules>
        <rule name="SPA fallback" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
            <add input="{REQUEST_URI}" pattern="^/(api|uploads|Uploads|FilesManager)" negate="true" />
          </conditions>
          <action type="Rewrite" url="/index.html" />
        </rule>
      </rules>
    </rewrite>
  </system.webServer>
</configuration>
```

On `admin.customer.com` the SPA and AdminAPI share a hostname. Host the API as an IIS **application** under `/api` (or use ARR to proxy), so `/api`, `/uploads` and `/FilesManager` reach the API while everything else serves the SPA.

---



## 9. SSL

Issue certificates for all four hostnames and bind them in IIS. [win-acme](https://www.win-acme.com/) automates Let's Encrypt on Windows and renews on a schedule.

---



## 10. First-time setup (the setup page)

Open:

```text
https://admin.customer.com/setup
```

`SetupGuard` forces every new install to this page until setup is complete.

Fill in:

1. **Company / mosque name**
2. **Primary color** (global theme)
3. **Domain** — pre-filled from `Deployment:Domain`, editable
4. **Logo**
5. **Super admin** — name, email (this is the login username), password (min 6 characters) + confirm
6. Optional: App Store / Google Play links for parent and teacher apps

Submit → you are redirected to `/login`. Sign in with the email and password you just created.

The super admin is created as a teacher record with `UsersManage` enabled. Create the rest of the staff accounts from **المعلمين**, granting user management only to those who need it.

If you restore a database that already has a mosque name, `SetupCompleted` may already be `1` and the wizard is skipped — use an existing admin account instead.

---



## 11. After login — Integrations

Open **التكاملات** (`https://admin.customer.com/integrations`):


| Field                    | Purpose                                        |
| ------------------------ | ---------------------------------------------- |
| Wasender Api Token       | WhatsApp API token                             |
| Wasender Session API Key | Session key (or connect via WhatsApp QR later) |
| Agora App Id             | Video calls                                    |
| Agora App Certificate    | Video calls (server secret)                    |


Values saved here are stored in the database and override `appsettings.json`.

**WhatsApp note:** the token alone is not enough — you usually also need a connected session (Admin → WhatsApp QR / Check Health).

---



## 12. Push notifications (Firebase)

Every customer needs their own Firebase project. `firebase-service-account.json` is **not** in source control and is **stripped from the deployment packages**, so it must be placed on each server by hand.

1. In the Firebase console create the project, then **Project settings → Service accounts → Generate new private key**
2. Copy the JSON as `firebase-service-account.json` next to the API executable (AdminAPI, and MasgedParentMobileAPI if it sends push)
3. In `appsettings.json`:

```json
"Firebase": {
  "Enabled": true,
  "ProjectId": "customer-firebase-project-id",
  "ServiceAccountJsonPath": "firebase-service-account.json"
}
```

4. Recycle the app pool

The mobile app needs its own files from the same project: `google-services.json` (Android) and `GoogleService-Info.plist` (iOS). For iOS, upload the APNs key in Firebase or notifications will never arrive.

If the service-account file is missing, the APIs run normally and simply skip sending push messages.

---



## 13. Mobile app (Flutter / Codemagic)

API hosts are compiled into the app, so each customer needs their own build.

```powershell
flutter build appbundle --release `
  --dart-define=API_BASE_URL=https://api.customer.com `
  --dart-define=MEDIA_BASE_URL=https://admin.customer.com/ `
  --dart-define=PRIVACY_POLICY_URL=https://customer.com/privacy-policy
```

Icons from the customer logo:

```powershell
cd ParentApp
.\tool\generate_store_icons.ps1 -LogoPath C:\path\to\logo.png -BackgroundColor "#071B3A"
```

See `google-play/templates/icon-assets.md` for store image sizes.

### Codemagic

Copy `ParentApp/codemagic.yaml.example` to `ParentApp/codemagic.yaml` and replace, per customer:

| Placeholder | Replace with |
| --- | --- |
| `CUSTOMER_NAME` | Workflow label shown in Codemagic |
| `CUSTOMER_ASC_INTEGRATION_NAME` | Name of the App Store Connect integration created in Codemagic |
| `com.customer.app` | The customer's bundle id / application id |
| `API_BASE_URL`, `MEDIA_BASE_URL` | The customer's API hosts |

Signing secrets (Apple certificates, Google Play service account, Android keystore) belong in the Codemagic UI, never in the committed YAML.

Also change `namespace` and `applicationId` in `ParentApp/android/app/build.gradle.kts` (currently `com.mubarakmasged.com`) and the bundle id in Xcode to match, and drop in the customer's `google-services.json` / `GoogleService-Info.plist` before building.

Once both apps are published, put the store links into **الإعدادات** so the public website shows them.

---



## 14. Changing the domain

The domain field in **الإعدادات** only changes what the platform displays. A real domain move needs all of these, in order:

1. DNS: A records for the new `@`, `www`, `admin`, `api` → server IP
2. Issue SSL certificates for the four new hostnames and bind them in IIS
3. AdminAPI `appsettings.json`: `Cors:Origins`, `PublicSite:BaseUrl`, `Deployment:Domain`
4. MasgedParentMobileAPI `appsettings.json`: `ApiSettings:MediaBaseUrl`
5. Update `AdminPanelUI\.env` and `PublicWebsiteUI\.env`, then **rebuild and redeploy both SPAs** — Vite bakes these URLs into the bundle
6. Update the domain in **الإعدادات**
7. Rebuild the mobile app with the new `API_BASE_URL` / `MEDIA_BASE_URL` and ship a store update

Keep the old domain resolving (redirect) for a transition period — installed mobile apps keep calling the old host until users update.

---

## 15. Backups

Three things must be backed up:

| What | Notes |
| --- | --- |
| Database | Full daily backup; add log backups if the recovery model is Full |
| File folders | `Uploads` and `FilesManager` under the AdminAPI deployment folder |
| Config | `appsettings.json` and `firebase-service-account.json` — they hold secrets that cannot be regenerated |

```sql
BACKUP DATABASE NewMasgedTeacherAPIDB
TO DISK = 'D:\Backups\masged.bak'
WITH INIT, COMPRESSION, CHECKSUM;
```

- Schedule with SQL Server Agent (or Task Scheduler + `sqlcmd` on Express, which has no Agent) and copy backups off the server
- Restore-test on a spare server periodically; an untested backup is not a backup
- Take a full backup of the database and file folders before every application update

---

## 16. Updating an existing customer

1. Build the SPAs with **that customer's** `.env` values, then run `.\publish-all.ps1`
2. Take a backup (database + `Uploads` + `FilesManager` + `appsettings.json`)
3. Stop the IIS site / app pool
4. Extract the new files over the deployed folder
5. Start the site

The API zips contain no `appsettings.json`, no Firebase key and no `Uploads`/`FilesManager`/`Logs`, so extracting over the live folder leaves those untouched. The existing config keeps working as-is.

Two things the package cannot protect you from:

- **The SPA bundles** have `VITE_*` URLs compiled in. Building `AdminPanelUI.zip` with template values and deploying it will point the panel at the wrong domain. Always build with the customer's `.env`.
- **New config keys** added by an update are not in the live `appsettings.json`. Compare it against the shipped `appsettings.example.json` after a big update and add anything missing.

Existing installs will **not** be sent through the setup wizard: on startup the bootstrap sets `SetupCompleted = 1` for any `MasgedSettings` row that already has a name, and `SetupGuard` only forces `/setup` when the API explicitly reports that setup is incomplete.

---



## 17. Checklist (new customer)

- [ ] If starting from a copied project: `.\new-customer-reset.ps1` executed
- [ ] DNS: `@`, `www`, `admin`, `api` → server IP
- [ ] ASP.NET Core Hosting Bundle + URL Rewrite installed
- [ ] Database created, Quran reference data restored
- [ ] UI `.env` files set to the customer domain **before** building
- [ ] `.\publish-all.ps1` succeeded
- [ ] Both APIs deployed with unique secrets and correct `Deployment:Domain`
- [ ] Both SPAs deployed with SPA-fallback `web.config`
- [ ] SSL bound for all four hostnames
- [ ] `https://admin.customer.com/setup` completed and super admin created
- [ ] Login works
- [ ] Integrations configured (Wasender / Agora) and a WhatsApp session connected
- [ ] Firebase JSON in place and a test notification received
- [ ] Flutter app built with the customer `API_BASE_URL`, published, store links saved in **الإعدادات**
- [ ] Backups scheduled and one restore tested

---



## 18. Troubleshooting

| Problem | What to check |
| --- | --- |
| Setup page never appears | Setup is already complete (`SetupCompleted = 1`) — sign in with an existing admin |
| Setup page domain field is empty | `Deployment:Domain` missing in `appsettings.json` — you can still type the domain |
| Tables missing on a fresh install | `Deployment:EnsureDatabase` must be `true` |
| Quran / memorization screens are empty | Reference tables were never restored — see section 5 |
| SPA routes 404 on refresh | URL Rewrite module + SPA fallback `web.config` |
| UI calls the wrong domain | Vite `.env` was wrong at build time — rebuild the UI |
| 500.19 / 500.30 in IIS | Hosting Bundle not installed, or app pool is not "No Managed Code" |
| Upload fails | App pool identity needs modify rights on `Uploads` and `FilesManager` |
| Logo does not render | Check `VITE_UPLOADS_BASE_URL` and that `/uploads` reaches the API |
| Push notifications never arrive | `Firebase:Enabled`, `ProjectId`, service-account file; APNs key for iOS |
| WhatsApp messages not sent | Token is set but no session is connected — open **ربط الواتساب** |
| Video calls fail | Agora App Id / App Certificate in **التكاملات** |
| Login fails after setup | Use the **email** as the username; check the API logs |


