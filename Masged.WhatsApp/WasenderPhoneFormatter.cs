namespace Masged.WhatsApp;

public static class WasenderPhoneFormatter
{
    public static string? FormatForWasender(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var normalized = PhoneNormalizer.ToEnglishDigits(input.Trim());
        if (string.IsNullOrEmpty(normalized))
            return null;

        var s = normalized;
        if (s.StartsWith('+'))
        {
            var afterPlus = new string(s.Skip(1).Where(char.IsDigit).ToArray());
            return string.IsNullOrEmpty(afterPlus) ? null : "+" + afterPlus;
        }

        var digits = new string(s.Where(char.IsDigit).ToArray());
        if (string.IsNullOrEmpty(digits))
            return null;

        if (digits.StartsWith('0') && digits.Length > 8)
            digits = digits.TrimStart('0');

        if (digits.StartsWith("965", StringComparison.Ordinal) && digits.Length == 11)
            return "+" + digits;

        if (digits.Length == 8)
            return "+965" + digits;

        return "+" + digits;
    }
}
