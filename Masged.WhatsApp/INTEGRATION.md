# Masged.WhatsApp — Integration Guide

Standalone .NET 8 library for sending WhatsApp messages via [WasenderAPI](https://www.wasenderapi.com).

**Project path:** `Masged Parent App/Masged.WhatsApp/Masged.WhatsApp.csproj`

---

## Quick start (pick one mode)

| Mode | When to use | What you need |
|------|-------------|---------------|
| **A — Shared queue** | Same SQL Server DB as AdminAPI; AdminAPI is running | Insert into `whatsapp_temp_table` only |
| **B — Full standalone** | Your app sends on its own | Reference library + implement 2 interfaces |
| **C — Direct send** | Send immediately, no queue | Reference library + `IWasenderApiClient` |

Copy-ready sample files are in the [`Samples/`](./Samples/) folder.

---

## Mode A — Shared queue (simplest)

If AdminAPI is already running and shares your database, **do not** reference `Masged.WhatsApp` in the other app. Just enqueue rows.

### Database table

```sql
-- Table: whatsapp_temp_table
-- id      INT IDENTITY PK
-- message NVARCHAR(MAX)
-- image   NVARCHAR(MAX) NULL   -- base64, optional
-- mobile  VARCHAR(50)
-- IsGirl  INT NULL
```

### Enqueue (C# / EF)

```csharp
db.WhatsappTempTables.Add(new WhatsappTempTable
{
    Message = "Hello parent",
    Mobile = Masged.WhatsApp.PhoneNormalizer.ToWhatsappE164(fatherPhone), // +965XXXXXXXX
    Image = null,
    IsGirl = 0,
});
await db.SaveChangesAsync();
```

AdminAPI's `WhatsAppBackgroundService` picks up rows every ~20 seconds and sends them.

---

## Mode B — Full standalone app

Your app references the library, runs the background worker, and implements storage adapters.

### Step 1 — Add project reference

```xml
<!-- YourApp.csproj -->
<ItemGroup>
  <ProjectReference Include="..\Masged.WhatsApp\Masged.WhatsApp.csproj" />
</ItemGroup>
```

Or reference the built DLL:

```xml
<Reference Include="Masged.WhatsApp">
  <HintPath>..\Masged.WhatsApp\bin\Debug\net8.0\Masged.WhatsApp.dll</HintPath>
</Reference>
```

### Step 2 — appsettings.json

Copy from [`Samples/appsettings.whatsapp.json`](./Samples/appsettings.whatsapp.json):

```json
{
  "Wasender": {
    "BaseUrl": "https://www.wasenderapi.com/api",
    "ApiToken": "YOUR_WASENDER_ACCOUNT_TOKEN",
    "SessionApiKey": "YOUR_SESSION_API_KEY_AFTER_QR_CONNECT",
    "ErrorLogDirectory": "Logs"
  },
  "WhatsAppProcessor": {
    "IntervalSeconds": 20,
    "InitialDelaySeconds": 5,
    "BatchSize": 10,
    "DelayBetweenMessagesMs": 7000,
    "ErrorLogFileName": "whatsapp_errors_log.txt"
  }
}
```

| Setting | Description |
|---------|-------------|
| `Wasender:ApiToken` | Account token from Wasender dashboard. Used for media upload + session management. |
| `Wasender:SessionApiKey` | Per-session key returned after WhatsApp QR connect. Required to send messages. |
| `WhatsAppProcessor:DelayBetweenMessagesMs` | Pause between sends (default 7000 ms). |

Store secrets in User Secrets or environment variables — not in source control.

### Step 3 — Register services (Program.cs)

```csharp
using Masged.WhatsApp.Extensions;
using Masged.WhatsApp.Interfaces;

builder.Services.AddMasgedWhatsApp(builder.Configuration);
builder.Services.AddScoped<IWhatsappSessionStore, ConfigWhatsappSessionStore>(); // or your own
builder.Services.AddScoped<IWhatsappQueueRepository, EfWhatsappQueueRepository>(); // or your own
```

See [`Samples/Program.registration.example.cs`](./Samples/Program.registration.example.cs).

### Step 4 — Implement adapters

You must register **both** interfaces:

#### `IWhatsappSessionStore`

Stores Wasender session ID and session API key (used when sending).

| Method | Purpose |
|--------|---------|
| `SessionName` | Label when creating a Wasender session (e.g. `"Masged"`) |
| `GetSessionIdAsync` / `SetSessionIdAsync` | Wasender session ID |
| `GetSessionApiKeyAsync` / `SetSessionApiKeyAsync` | Key used in `Authorization: Bearer` for `send-message` |

Sample: [`Samples/ConfigWhatsappSessionStore.cs`](./Samples/ConfigWhatsappSessionStore.cs) (config-only, good for new apps).

AdminAPI uses `WhatsappQrSessionStore` → `AppSettings` table.

#### `IWhatsappQueueRepository`

Used by the background worker to drain pending messages.

| Method | Purpose |
|--------|---------|
| `DequeueBatchAsync(batchSize)` | Return oldest N pending items |
| `RemoveAsync(id)` | Delete item after send attempt (success or failure) |

Sample: [`Samples/EfWhatsappQueueRepository.cs`](./Samples/EfWhatsappQueueRepository.cs).

### Step 5 — Enqueue messages

```csharp
using Masged.WhatsApp;

await WhatsappEnqueueHelper.EnqueueAsync(db, new WhatsappTempTableEntity
{
    Message = "Your text",
    Mobile = PhoneNormalizer.ToWhatsappE164(phone),
    Image = null,
    IsGirl = 0,
});
```

Or use [`Samples/WhatsappEnqueueHelper.cs`](./Samples/WhatsappEnqueueHelper.cs) as a template.

---

## Mode C — Direct send (no queue)

Register the library (or only the HTTP client parts manually), ensure `SessionApiKey` is set, then inject `IWasenderApiClient`:

```csharp
using Masged.WhatsApp;
using Masged.WhatsApp.Interfaces;

public class NotifyService(IWasenderApiClient wasender)
{
    public async Task SendAsync(string phone, string text, string? imageBase64 = null)
    {
        var to = PhoneNormalizer.ToWhatsappE164(phone);
        var (success, error) = await wasender.SendMessageAsync(to, text, imageBase64);
        if (!success)
            throw new InvalidOperationException(error ?? "WhatsApp send failed");
    }
}
```

To skip the background worker, register clients manually instead of `AddMasgedWhatsApp()`:

```csharp
builder.Services.Configure<WasenderApiOptions>(builder.Configuration.GetSection("Wasender"));
builder.Services.AddHttpClient<IWasenderApiClient, WasenderApiClient>(/* ... same as extension */);
builder.Services.AddScoped<IWhatsappSessionStore, ConfigWhatsappSessionStore>();
builder.Services.AddScoped<WasenderSessionKeySyncService>();
// Do NOT call AddHostedService<WhatsAppBackgroundService>()
```

---

## Library API reference

### `IWasenderApiClient`

```csharp
Task<(bool Success, string? Error)> SendMessageAsync(
    string to,
    string text,
    string? imageBase64 = null,
    CancellationToken cancellationToken = default);
```

- `to` — phone in international format (`+96551234567`). Use `WasenderPhoneFormatter.FormatForWasender()` or `PhoneNormalizer.ToWhatsappE164()` for Kuwait numbers.
- `imageBase64` — raw base64 or `data:image/jpeg;base64,...`. Uploaded via Wasender `POST /upload`, then sent as `imageUrl`.

### `IWasenderSessionClient`

Session / QR management (optional — only if you build your own QR UI):

- `CreateSessionReplacingIfNeededAsync(name, phoneNumber)`
- `ConnectSessionAsync(sessionId)` → QR code
- `GetSessionDetailsAsync(sessionId)` → status + `api_key`
- `DisconnectSessionAsync(sessionId)`

### `PhoneNormalizer`

| Method | Example output |
|--------|----------------|
| `ToEnglishDigits("٠٥١٢٣٤٥٦")` | `05123456` |
| `ToCanonical("96551234567")` | `51234567` |
| `ToWhatsappE164("51234567")` | `+96551234567` |
| `ContainsArabicDigits(phone)` | `true` / `false` |

### `WasenderPhoneFormatter.FormatForWasender(phone)`

Formats any input for Wasender API (handles `+`, leading `0`, Kuwait 8-digit numbers).

---

## Wasender HTTP contract

### Send message

```
POST https://www.wasenderapi.com/api/send-message
Authorization: Bearer {SessionApiKey}
Content-Type: application/json

{ "to": "+96551234567", "text": "Hello", "imageUrl": "https://..." }
```

### Upload image

```
POST https://www.wasenderapi.com/api/upload
Authorization: Bearer {ApiToken}
Content-Type: application/json

{ "base64": "data:image/jpeg;base64,..." }
```

---

## Background worker behavior

`WhatsAppBackgroundService` (registered by `AddMasgedWhatsApp()`):

1. Waits `InitialDelaySeconds` (default 5s) after startup.
2. Every `IntervalSeconds` (default 20s), loads up to `BatchSize` (10) queue items.
3. Sends each via `IWasenderApiClient`.
4. Waits `DelayBetweenMessagesMs` (7000ms) between messages.
5. **Always removes** the row after attempt — failures are logged, not retried.

Log files (under app base directory):

| File | Content |
|------|---------|
| `Logs/WhatsAppSendLog.txt` | Every API send attempt |
| `Logs/whatsapp_errors_log.txt` | Failed queue items |

---

## WhatsApp session setup

Before sends work, a WhatsApp session must be connected:

1. Create session via Wasender API (or AdminAPI QR screen).
2. Scan QR code with WhatsApp on the phone.
3. Copy `api_key` from session details into `Wasender:SessionApiKey` (or persist via `IWhatsappSessionStore`).

AdminAPI admin panel: **WhatsApp QR → Check Health** syncs the key automatically.

---

## File map (library)

```
Masged.WhatsApp/
├── INTEGRATION.md                          ← this file
├── Masged.WhatsApp.csproj
├── Extensions/ServiceCollectionExtensions.cs
├── Interfaces/
│   ├── IWasenderApiClient.cs
│   ├── IWasenderSessionClient.cs
│   ├── IWhatsappSessionStore.cs
│   └── IWhatsappQueueRepository.cs
├── Models/WhatsappQueueItem.cs
├── Options/
│   ├── WasenderApiOptions.cs
│   └── WhatsAppProcessorOptions.cs
├── PhoneNormalizer.cs
├── WasenderPhoneFormatter.cs
├── Services/
│   ├── WasenderApiClient.cs
│   ├── WasenderSessionClient.cs
│   ├── WasenderSessionKeySyncService.cs
│   └── WhatsAppBackgroundService.cs
└── Samples/                                ← copy into your app
    ├── appsettings.whatsapp.json
    ├── ConfigWhatsappSessionStore.cs
    ├── EfWhatsappQueueRepository.cs
    ├── WhatsappTempTableEntity.cs
    ├── WhatsappEnqueueHelper.cs
    └── Program.registration.example.cs
```

---

## Troubleshooting

| Error | Fix |
|-------|-----|
| `WhatsApp session API key is not configured` | Set `Wasender:SessionApiKey` or connect QR session |
| Messages enqueued but never sent | Ensure app with `AddMasgedWhatsApp()` is running |
| `Upload failed` | Check `Wasender:ApiToken` |
| Wrong recipient | Use `PhoneNormalizer.ToWhatsappE164()` before enqueue/send |
| Arabic digit phones skipped | Convert with `ToEnglishDigits` or reject with `ContainsArabicDigits` |

---

## AdminAPI reference implementation

| Adapter | File |
|---------|------|
| Session store | `AdminAPI/Services/WhatsappQrSessionStore.cs` |
| Queue repository | `AdminAPI/Repositories/WhatsappQueueRepository.cs` |
| DI registration | `AdminAPI/Program.cs` → `AddMasgedWhatsApp()` |
| QR UI service | `AdminAPI/Services/WhatsappQrService.cs` |
