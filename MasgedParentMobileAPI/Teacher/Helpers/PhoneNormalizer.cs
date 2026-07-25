namespace MasgedTeacherMobileAPI.Helpers;

public static class PhoneNormalizer
{
    public static IEnumerable<string> GetVariants(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            yield break;

        var digits = new string(phone.Where(char.IsDigit).ToArray());

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
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (digits.StartsWith("965") && digits.Length > 8)
            digits = digits[3..];
        if (digits.Length > 8)
            digits = digits[^8..];
        return digits;
    }
}
