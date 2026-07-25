using System.Security.Cryptography;
using System.Text;
using MasgedParentMobileAPI.Configuration;
using Microsoft.Extensions.Options;

namespace MasgedParentMobileAPI.Services;

public class StudentQrTokenService
{
    public const string TokenPrefix = "MSQR:";

    private readonly byte[] _key;

    public StudentQrTokenService(IOptions<StudentQrOptions> options)
    {
        var keyText = options.Value.EncryptionKey;
        if (string.IsNullOrWhiteSpace(keyText) || keyText.Length < 32)
            throw new InvalidOperationException("StudentQr:EncryptionKey must be at least 32 characters.");

        _key = SHA256.HashData(Encoding.UTF8.GetBytes(keyText));
    }

    public bool TryDecryptStudentId(string token, out int studentId)
    {
        studentId = 0;

        if (string.IsNullOrWhiteSpace(token) || !token.StartsWith(TokenPrefix, StringComparison.Ordinal))
            return false;

        byte[] payload;
        try
        {
            payload = Convert.FromBase64String(token[TokenPrefix.Length..]);
        }
        catch (FormatException)
        {
            return false;
        }

        if (payload.Length < 12 + 16 + 1)
            return false;

        var nonce = payload[..12];
        var tag = payload[12..28];
        var cipherBytes = payload[28..];
        var plainBytes = new byte[cipherBytes.Length];

        try
        {
            using var aes = new AesGcm(_key, tagSizeInBytes: 16);
            aes.Decrypt(nonce, cipherBytes, tag, plainBytes);
        }
        catch (CryptographicException)
        {
            return false;
        }

        var plainText = Encoding.UTF8.GetString(plainBytes);
        return int.TryParse(plainText, out studentId) && studentId > 0;
    }
}
