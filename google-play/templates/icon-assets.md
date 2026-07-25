# Store icon / listing assets (white-label)

Use the company logo from Admin first-time setup (or `/settings`) as the source art.

## Sizes checklist

| Platform | Asset | Size |
|----------|--------|------|
| Android / Play | High-res icon | **512 × 512** PNG, 32-bit |
| Android | Feature graphic | **1024 × 500** |
| iOS / App Store | App icon | **1024 × 1024** PNG, no alpha |
| Flutter in-app | `assets/images/app_icon.png` | preferably 1024 × 1024 |

## Generate Flutter launcher icons

From `ParentApp`:

```powershell
.\tool\generate_store_icons.ps1 -LogoPath C:\path\to\customer-logo.png -BackgroundColor "#071B3A"
```

This copies the logo into `assets/images/app_icon.png` + `app_icon_foreground.png`, then runs `flutter_launcher_icons` (Android + iOS).

## After icons

1. Copy `codemagic.yaml.example` → `codemagic.yaml` (or configure Codemagic UI) with the customer `BUNDLE_ID` and ASC integration.
2. Set `--dart-define=API_BASE_URL=https://api.CUSTOMER_DOMAIN` on the IPA build.
3. Upload 512 / 1024 icons in Play Console / App Store Connect separately if required.
