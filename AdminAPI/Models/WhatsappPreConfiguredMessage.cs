namespace AdminAPI.Models;

public class WhatsappPreConfiguredMessage
{
    public int Id { get; set; }
    public string WhatsappMessage { get; set; } = string.Empty;
    public string Event { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}
