import {
  buildCountryData,
  defaultCountries,
  parseCountry,
  type CountryData,
} from 'react-international-phone'

const PREFERRED_COUNTRY_CODES = [
  'kw',
  'sa',
  'ae',
  'ps',
  'eg',
  'qa',
  'bh',
  'om',
  'jo',
  'iq',
  'sy',
  'lb',
] as const

let cachedCountries: CountryData[] | null = null

export function getRegistrationCountries(): CountryData[] {
  if (cachedCountries) return cachedCountries

  cachedCountries = defaultCountries
    .filter((entry) => parseCountry(entry).iso2 !== 'il')
    .map((entry) => {
      const parsed = parseCountry(entry)
      if (parsed.iso2 === 'ps') {
        return buildCountryData({ ...parsed, name: 'فلسطين' })
      }
      return entry
    })

  return cachedCountries
}

export function getPreferredCountryCodes() {
  return [...PREFERRED_COUNTRY_CODES]
}
