# Google Play — Sensitive permissions justification (Arabic + English)

Use in **Policy → App content → Sensitive app permissions** and Privacy Policy.

## INTERNET
**Why:** API calls, push notifications (FCM), video calls (Agora), on-demand Quran font download.

## POST_NOTIFICATIONS
**Why:** Chat messages and video call invitations from teachers.

## ACCESS_FINE_LOCATION / ACCESS_COARSE_LOCATION
**Why:** Prayer times, Qibla direction, nearest mosques map, teacher attendance geo-verification at mosque.
**Not used for:** Background tracking or advertising.

## CAMERA
**Why:** Video calls with teacher; QR code scan for teacher attendance.
**Requested:** At runtime when user starts call or opens scanner.

## RECORD_AUDIO
**Why:** Video call audio; teacher voice commands for attendance.
**Requested:** At runtime.

## READ_MEDIA_IMAGES / Storage (legacy)
**Why:** Save Quran page images to device gallery when user taps share/save.

## BLUETOOTH_CONNECT
**Why:** Route video call audio to Bluetooth headsets (optional).

## FOREGROUND_SERVICE (+ location, mediaProjection)
**Why:** Geolocator foreground updates during attendance; Agora screen share in video calls.

## USE_BIOMETRIC
**Why:** Optional teacher login / attendance confirmation via fingerprint/Face ID.

## Permissions NOT used (removed from manifest)
- READ_PHONE_STATE — removed (was merged by Agora SDK, not needed)

## Privacy policy must mention
All permissions above + third parties: Google (FCM, Speech, ML Kit), Agora.
