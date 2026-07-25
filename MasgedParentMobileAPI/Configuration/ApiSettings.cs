namespace MasgedParentMobileAPI.Configuration;

public class ApiSettings
{
    public string MediaBaseUrl { get; set; } = string.Empty;
    public JwtSettings Jwt { get; set; } = new();
}

public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpireMinutes { get; set; } = 10080;
}
