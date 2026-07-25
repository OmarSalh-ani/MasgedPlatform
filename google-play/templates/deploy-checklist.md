# Google Play — Deploy checklist

Generated artifacts are in `publish/google-play/` after running `publish-all.ps1`.

## Before upload

- [ ] **AAB** — `masged-parent-app-v*.aab` (signed with upload keystore)
- [ ] **Upload certificate** — `upload_certificate.pem` (register on first Play Console upload)
- [ ] **Obfuscation symbols** — keep `ParentApp/build/app/outputs/symbols/` per release
- [ ] **QCF fonts live** — verify `https://admin-api.mosque-mbark-j.com/static/qcf-fonts/p1.woff` returns 200
- [ ] **Privacy policy live** — URL opens in browser
- [ ] **Demo accounts** — fill `release-notes.ar.md` reviewer section

## Play Console steps

1. **Create app** → default language Arabic
2. **App signing** → upload `upload_certificate.pem` (or let Google manage)
3. **Production → Create release** → upload AAB
4. **Store listing** → copy from `store-listing.ar.md`
5. **App content:**
   - Privacy policy URL
   - Data safety → `data-safety.md`
   - Ads → No ads
   - Content rating questionnaire
   - Target audience → 18+, not for children
   - Sensitive permissions → `permissions-justification.md`
6. **Release notes** → `release-notes.ar.md`
7. **Screenshots** — phone (required), 7-inch/10-inch if supporting tablets

## Build commands (standalone)

```powershell
# Full publish (all projects + fonts + AAB)
.\publish-all.ps1

# Android only
cd ParentApp
.\tool\play_deploy.ps1
```

## Version bump

Edit `ParentApp/pubspec.yaml`:
```yaml
version: 1.0.1+2   # name+code — increment code every Play upload
```

## QCF fonts on server

`publish-all.ps1` copies fonts into AdminAPI publish output at:
`static/qcf-fonts/p1.woff` … `p604.woff`

Ensure your IIS/nginx site serves `/static/qcf-fonts/` from that folder with `font/woff` MIME type.
