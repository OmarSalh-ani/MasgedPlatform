using System.Security.Cryptography;
using System.Text;
using AdminAPI.Configuration;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

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

    public string EncryptStudentId(int studentId)
    {
        var plainBytes = Encoding.UTF8.GetBytes(studentId.ToString());
        var nonce = RandomNumberGenerator.GetBytes(12);
        var cipherBytes = new byte[plainBytes.Length];
        var tag = new byte[16];

        using var aes = new AesGcm(_key, tagSizeInBytes: 16);
        aes.Encrypt(nonce, plainBytes, cipherBytes, tag);

        var payload = new byte[nonce.Length + tag.Length + cipherBytes.Length];
        Buffer.BlockCopy(nonce, 0, payload, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, payload, nonce.Length, tag.Length);
        Buffer.BlockCopy(cipherBytes, 0, payload, nonce.Length + tag.Length, cipherBytes.Length);

        return TokenPrefix + Convert.ToBase64String(payload);
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
