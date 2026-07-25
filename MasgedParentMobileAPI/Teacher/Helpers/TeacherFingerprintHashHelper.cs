using System.Security.Cryptography;
using System.Text;

namespace MasgedTeacherMobileAPI.Helpers;

/// <summary>
/// Hashes a client enrollment secret bound to the teacher id.
/// The OS biometric gate is separate; only the derived hash is stored server-side.
/// </summary>
public static class TeacherFingerprintHashHelper
{
    public static string ComputeHash(int teacherId, string enrollmentSecret)
    {
        var payload = $"{teacherId}:{enrollmentSecret}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static bool IsValidHashFormat(string? hash) =>
        !string.IsNullOrWhiteSpace(hash)
        && hash.Length == 64
        && hash.All(static c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f'));

    public static bool HashesMatch(string storedHash, string providedHash) =>
        CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(storedHash),
            Encoding.UTF8.GetBytes(providedHash));
}
