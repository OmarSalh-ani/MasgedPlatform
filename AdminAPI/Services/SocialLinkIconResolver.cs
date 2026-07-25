namespace AdminAPI.Services;

public static class SocialLinkIconResolver
{
    public static string Resolve(string? iconClass, string? platformName)
    {
        if (!string.IsNullOrWhiteSpace(iconClass))
        {
            var resolved = iconClass.Trim();
            if (resolved.Contains("x-twitter", StringComparison.OrdinalIgnoreCase)
                || resolved.Contains("twitter-f", StringComparison.OrdinalIgnoreCase))
                return "fab fa-twitter";
            return resolved;
        }

        if (string.IsNullOrWhiteSpace(platformName))
            return "fas fa-link";

        var p = platformName.ToLowerInvariant();
        if (p.Contains("facebook") || p.Contains("فيسبوك")) return "fab fa-facebook-f";
        if (p.Contains("twitter") || p.Contains("تويتر") || p.Contains("x")) return "fab fa-twitter";
        if (p.Contains("whatsapp") || p.Contains("واتساب")) return "fab fa-whatsapp";
        if (p.Contains("instagram") || p.Contains("انستغرام") || p.Contains("انستقرام")) return "fab fa-instagram";
        if (p.Contains("youtube") || p.Contains("يوتيوب")) return "fab fa-youtube";
        if (p.Contains("tiktok") || p.Contains("تيك توك")) return "fab fa-tiktok";
        if (p.Contains("telegram") || p.Contains("تيليجرام")) return "fab fa-telegram-plane";
        return "fas fa-link";
    }
}
