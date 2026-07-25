namespace Masged.WhatsApp.Options;

public class WhatsAppProcessorOptions
{
    public const string SectionName = "WhatsAppProcessor";

    public int IntervalSeconds { get; set; } = 20;
    public int InitialDelaySeconds { get; set; } = 5;
    public int BatchSize { get; set; } = 10;
    public int DelayBetweenMessagesMs { get; set; } = 7000;
    public string ErrorLogFileName { get; set; } = "whatsapp_errors_log.txt";
}
