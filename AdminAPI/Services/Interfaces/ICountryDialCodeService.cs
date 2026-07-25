using AdminAPI.DTOs.CountryDialCodes;

namespace AdminAPI.Services.Interfaces;

public interface ICountryDialCodeService
{
    IReadOnlyList<CountryDialEntryDto> GetCountries();

    string GetDialCodeForIso(string isoCode);

    string CombineIsoAndLocal(string isoCode, string localRaw);

    void ClearCache();
}
