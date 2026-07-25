namespace AdminAPI.DTOs.WhatsappPending;

public class WhatsappPendingMessageDto
{
    public int Id { get; set; }
    public string? Mobile { get; set; }
    public string MessagePreview { get; set; } = string.Empty;
    public bool HasImage { get; set; }
}
