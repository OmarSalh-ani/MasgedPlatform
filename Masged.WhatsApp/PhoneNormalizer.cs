using System.Text;

namespace Masged.WhatsApp;

public static class PhoneNormalizer
{
    public static string ToEnglishDigits(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return string.Empty;

        var sb = new StringBuilder(phone.Length);
        foreach (var c in phone)
        {
            if (c is >= '0' and <= '9')
                sb.Append(c);
            else if (c is >= '\u0660' and <= '\u0669')
                sb.Append((char)('0' + (c - '\u0660')));
            else if (c is >= '\u06F0' and <= '\u06F9')
                sb.Append((char)('0' + (c - '\u06F0')));
        }

        return sb.ToString();
    }

    public static IEnumerable<string> GetVariants(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            yield break;

        var digits = ToEnglishDigits(phone);
        if (string.IsNullOrEmpty(digits))
            yield break;

        if (digits.StartsWith("965") && digits.Length > 8)
            digits = digits[3..];

        if (digits.Length > 8)
            digits = digits[^8..];

        yield return digits;
        yield return $"+965{digits}";
        yield return $"965{digits}";
        yield return phone.Trim();
    }

    public static string ToCanonical(string phone)
    {
        var digits = ToEnglishDigits(phone);
        if (string.IsNullOrEmpty(digits))
            return string.Empty;

        if (digits.StartsWith("965") && digits.Length > 8)
            digits = digits[3..];
        if (digits.Length > 8)
            digits = digits[^8..];
        return digits;
    }

    public static string ToWhatsappE164(string? phone)
    {
        var local = ToCanonical(phone ?? string.Empty);
        return string.IsNullOrEmpty(local) ? string.Empty : $"+965{local}";
    }

    public static bool ContainsArabicDigits(string? phone)
    {
        if (string.IsNullOrEmpty(phone))
            return false;

        foreach (var c in phone)
        {
            if (c is >= '\u0660' and <= '\u0669')
                return true;
            if (c is >= '\u06F0' and <= '\u06F9')
                return true;
        }

        return false;
    }
}
