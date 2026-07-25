namespace Masged.WhatsApp.Models;

public sealed record WhatsappQueueItem(
    int Id,
    string? Mobile,
    string? Message,
    string? Image);
