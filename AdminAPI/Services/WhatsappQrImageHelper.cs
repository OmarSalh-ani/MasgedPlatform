using QRCoder;

namespace AdminAPI.Services;

public static class WhatsappQrImageHelper
{
    public static string? ToDataUrl(string? qrCodeString)
    {
        if (string.IsNullOrEmpty(qrCodeString))
            return null;

        try
        {
            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(qrCodeString, QRCodeGenerator.ECCLevel.Q);
            var png = new PngByteQRCode(data);
            var bytes = png.GetGraphic(10);
            return "data:image/png;base64," + Convert.ToBase64String(bytes);
        }
        catch
        {
            return null;
        }
    }
}
