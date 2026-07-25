using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace AdminAPI.Services;

public static partial class GoogleMapsCoordinateExtractor
{
    public static (string? Lat, string? Lng) ExtractCoordinates(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return (null, null);

        var processedUrl = url;
        string? htmlBody = null;

        if (url.Contains("google.com/maps", StringComparison.OrdinalIgnoreCase)
            || url.Contains("goo.gl", StringComparison.OrdinalIgnoreCase)
            || url.Contains("maps.app.goo.gl", StringComparison.OrdinalIgnoreCase)
            || url.Contains("maps.google", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.AllowAutoRedirect = true;
                request.Timeout = 15000;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";
                using var response = (HttpWebResponse)request.GetResponse();
                processedUrl = response.ResponseUri.ToString();
                using var reader = new StreamReader(response.GetResponseStream()!);
                htmlBody = reader.ReadToEnd();
            }
            catch
            {
                // continue with original URL
            }
        }

        var coordsFromUrl = TryExtractFromUrl(processedUrl);
        if (coordsFromUrl.Lat is not null)
            return coordsFromUrl;

        if (processedUrl != url)
        {
            coordsFromUrl = TryExtractFromUrl(url);
            if (coordsFromUrl.Lat is not null)
                return coordsFromUrl;
        }

        if (string.IsNullOrEmpty(htmlBody))
            return (null, null);

        var match = Regex.Match(htmlBody, @"center=(-?\d+\.?\d*)%2C(-?\d+\.?\d*)");
        if (match.Success)
            return (match.Groups[1].Value, match.Groups[2].Value);

        match = Regex.Match(htmlBody, @"center=(-?\d+\.\d+),(-?\d+\.\d+)");
        if (match.Success)
            return (match.Groups[1].Value, match.Groups[2].Value);

        match = Regex.Match(htmlBody, @"\[0,(-?\d+\.\d+),(-?\d+\.\d+)\]");
        if (match.Success)
        {
            var val1 = match.Groups[1].Value;
            var val2 = match.Groups[2].Value;
            if (double.TryParse(val1, NumberStyles.Any, CultureInfo.InvariantCulture, out var d1)
                && double.TryParse(val2, NumberStyles.Any, CultureInfo.InvariantCulture, out var d2))
            {
                if (d2 is >= -90 and <= 90)
                    return (val2, val1);
                if (d1 is >= -90 and <= 90)
                    return (val1, val2);
            }
        }

        match = Regex.Match(htmlBody, @"!3d(-?\d+\.\d+)!4d(-?\d+\.\d+)");
        if (match.Success)
            return (match.Groups[1].Value, match.Groups[2].Value);

        return (null, null);
    }

    private static (string? Lat, string? Lng) TryExtractFromUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return (null, null);

        var match = Regex.Match(url, @"@(-?\d+\.\d+),(-?\d+\.\d+)");
        if (match.Success)
            return (match.Groups[1].Value, match.Groups[2].Value);

        match = Regex.Match(url, @"place\/(-?\d+\.\d+),(-?\d+\.\d+)");
        if (match.Success)
            return (match.Groups[1].Value, match.Groups[2].Value);

        match = Regex.Match(url, @"!3d(-?\d+\.\d+)!4d(-?\d+\.\d+)");
        if (match.Success)
            return (match.Groups[1].Value, match.Groups[2].Value);

        match = Regex.Match(url, @"[?&]ll=(-?\d+\.\d+),(-?\d+\.\d+)");
        if (match.Success)
            return (match.Groups[1].Value, match.Groups[2].Value);

        match = Regex.Match(url, @"[?&]q=(-?\d+\.\d+),(-?\d+\.\d+)");
        if (match.Success)
            return (match.Groups[1].Value, match.Groups[2].Value);

        match = Regex.Match(url, @"[?&]center=(-?\d+\.\d+),(-?\d+\.\d+)");
        if (match.Success)
            return (match.Groups[1].Value, match.Groups[2].Value);

        match = Regex.Match(url, @"[?&]daddr=(-?\d+\.\d+),(-?\d+\.\d+)");
        if (match.Success)
            return (match.Groups[1].Value, match.Groups[2].Value);

        return (null, null);
    }
}
