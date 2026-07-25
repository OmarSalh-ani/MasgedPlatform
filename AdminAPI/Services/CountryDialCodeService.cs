using System.Text.Json;
using System.Text.RegularExpressions;
using AdminAPI.DTOs.CountryDialCodes;
using AdminAPI.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public sealed class CountryDialCodeService : ICountryDialCodeService
{
    private const string DefaultDialCode = "+965";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly object _sync = new();
    private readonly string _jsonPath;
    private List<CountryDialEntryDto>? _cache;

    public CountryDialCodeService(IHostEnvironment environment, IOptions<CountryDialCodeOptions> options)
    {
        var fileName = options.Value.JsonFileName;
        _jsonPath = Path.Combine(environment.ContentRootPath, fileName);
    }

    public IReadOnlyList<CountryDialEntryDto> GetCountries()
    {
        if (string.IsNullOrEmpty(_jsonPath) || !File.Exists(_jsonPath))
            return Array.Empty<CountryDialEntryDto>();

        lock (_sync)
        {
            if (_cache != null)
                return _cache;

            var json = File.ReadAllText(_jsonPath);
            var list = JsonSerializer.Deserialize<List<CountryDialEntryDto>>(json, JsonOptions)
                ?? new List<CountryDialEntryDto>();

            _cache = list
                .Where(x => !string.IsNullOrWhiteSpace(x.IsoCode))
                .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return _cache;
        }
    }

    public void ClearCache()
    {
        lock (_sync)
        {
            _cache = null;
        }
    }

    public string GetDialCodeForIso(string isoCode)
    {
        if (string.IsNullOrWhiteSpace(isoCode))
            return DefaultDialCode;

        var list = GetCountries();
        var entry = list.FirstOrDefault(x =>
            string.Equals(x.IsoCode, isoCode.Trim(), StringComparison.OrdinalIgnoreCase));

        var dial = entry?.DialCode?.Trim();
        return string.IsNullOrEmpty(dial) ? DefaultDialCode : dial;
    }

    public string CombineIsoAndLocal(string isoCode, string localRaw)
    {
        var code = GetDialCodeForIso(isoCode);
        if (!code.StartsWith("+", StringComparison.Ordinal))
            code = "+" + code.TrimStart('+');

        var digits = Regex.Replace(localRaw ?? string.Empty, @"\D", string.Empty);
        return string.IsNullOrEmpty(digits) ? string.Empty : code + digits;
    }
}
