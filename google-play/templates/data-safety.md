# Google Play — Data Safety declaration

Use this when filling **App content → Data safety** in Play Console.
Must match the shipped app + SDK behavior exactly.

## Data collected

| Play Console type | Collected | Shared | Purpose | Required |
|-------------------|-----------|--------|---------|----------|
| Name | Yes | No (your API only) | Account, profiles | Yes |
| Email address | Yes (teachers) | No | Teacher login | Yes |
| Phone number | Yes | No | Parent login (OTP) | Yes |
| User IDs | Yes | No | Auth sessions | Yes |
| Address | Yes | No | Parent profile | Optional |
| Other info (marital status, student records) | Yes | No | App functionality | Yes |
| Photos | Yes | No | Student avatars (server) | No |
| Messages | Yes | No | Parent–teacher chat | No |
| Precise location | Yes | No* | Prayer, Qibla, attendance geo | Optional |
| Approximate location | Yes | No* | Nearest mosques | Optional |
| Audio | Yes | Yes (Agora) | Video calls | Optional |
| Video | Yes | Yes (Agora) | Video calls | Optional |
| Device or other IDs | Yes | Yes (Google FCM) | Push notifications | Yes |
| App interactions | Yes | No | Device token registration | Yes |
| Crash logs | No | — | (No Crashlytics) | — |
| Diagnostics | Yes | Yes (FCM delivery) | Push infrastructure | Yes |

\*Processed on device for Qibla/prayer; sent to your API only for attendance features.

## Third parties

| SDK | Data shared | Purpose |
|-----|-------------|---------|
| Firebase Cloud Messaging | Device ID, FCM token | Push notifications |
| Agora RTC | Audio, video, network metadata | Live video lessons |
| Google Speech (Android STT) | Voice audio | Teacher voice commands only |
| Google ML Kit (mobile_scanner) | On-device camera frames | QR attendance (not uploaded) |

## Security practices

- Data encrypted in transit: **Yes** (HTTPS)
- Users can request deletion: **Yes** (in-app link → privacy policy)
- Data not used for advertising: **Yes**

## Target audience

- **18+** (parents and teachers)
- **Not** primarily child-directed
- **Not** enrolled in Designed for Families
