namespace AdminAPI.DTOs.WhatsappSender;

public class SendWhatsappSenderRequestDto
{
    public List<int> StudentIds { get; set; } = [];
    public string Message { get; set; } = string.Empty;
    public int? FormId { get; set; }
}
