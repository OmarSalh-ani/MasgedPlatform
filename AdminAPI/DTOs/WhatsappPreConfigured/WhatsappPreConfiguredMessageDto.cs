namespace AdminAPI.DTOs.WhatsappPreConfigured;

public class WhatsappPreConfiguredMessageDto
{
    public int Id { get; set; }
    public string Event { get; set; } = string.Empty;
    public string EventDisplayName { get; set; } = string.Empty;
    public string EventDescription { get; set; } = string.Empty;
    public string WhatsappMessage { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string PreviewMessage { get; set; } = string.Empty;
}
