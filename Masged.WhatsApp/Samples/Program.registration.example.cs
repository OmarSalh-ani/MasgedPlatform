// Paste into your Program.cs (ASP.NET Core host).
using Masged.WhatsApp.Extensions;
using Masged.WhatsApp.Interfaces;
using YourApp.WhatsApp;

var builder = WebApplication.CreateBuilder(args);

// 1) Merge WhatsApp config (copy Samples/appsettings.whatsapp.json into appsettings.json)
// 2) Reference Masged.WhatsApp project

builder.Services.AddMasgedWhatsApp(builder.Configuration);

// Required adapters — replace with your implementations:
builder.Services.AddScoped<IWhatsappSessionStore, ConfigWhatsappSessionStore>();
builder.Services.AddScoped<IWhatsappQueueRepository, EfWhatsappQueueRepository>();

// Optional: direct send from controllers/services
// builder.Services.AddScoped<NotifyService>();

var app = builder.Build();
app.Run();

// --- Usage in a controller/service (Mode B — queued) ---
//
// public class MyService(YourAppDbContext db)
// {
//     public Task NotifyParentAsync(string phone, string text) =>
//         WhatsappEnqueueHelper.EnqueueAsync(db, phone, text);
// }
//
// --- Usage (Mode C — immediate) ---
//
// public class NotifyService(IWasenderApiClient wasender)
// {
//     public async Task SendNowAsync(string phone, string text)
//     {
//         var (ok, err) = await wasender.SendMessageAsync(
//             Masged.WhatsApp.PhoneNormalizer.ToWhatsappE164(phone),
//             text);
//         if (!ok) throw new InvalidOperationException(err);
//     }
// }
