namespace AdminAPI.DTOs.WhatsappQr;

public class WhatsappQrStatusDto
{
    public string StatusText { get; set; } = string.Empty;
    public string? QrImageDataUrl { get; set; }
    public string? BodyHtml { get; set; }
    public bool ShowCreateSession { get; set; }
    public bool ShowDisconnect { get; set; }
    public bool ShowReconnect { get; set; }
    public bool IsConnected { get; set; }
}
