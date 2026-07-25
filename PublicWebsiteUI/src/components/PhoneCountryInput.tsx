import { useMemo } from 'react'
import { PhoneInput, type CountryIso2, type ParsedCountry } from 'react-international-phone'
import 'react-international-phone/style.css'
import { getPreferredCountryCodes, getRegistrationCountries } from '@/lib/phoneCountries'

interface PhoneCountryInputProps {
  id?: string
  countryIso: string
  phone: string
  onCountryChange: (iso: string) => void
  onPhoneChange: (phone: string) => void
  invalid?: boolean
  placeholder?: string
  disabled?: boolean
}

function digitsOnly(value: string) {
  return value.replace(/\D/g, '')
}

function phoneMaxForCountry(iso2: string) {
  return iso2 === 'kw' ? 8 : 15
}

export function PhoneCountryInput({
  id,
  countryIso,
  phone,
  onCountryChange,
  onPhoneChange,
  invalid = false,
  placeholder = '51234567',
  disabled = false,
}: PhoneCountryInputProps) {
  const countries = useMemo(() => getRegistrationCountries(), [])
  const preferredCountries = useMemo(() => getPreferredCountryCodes(), [])

  const defaultCountry = (countryIso?.length >= 2 ? countryIso.toLowerCase() : 'kw') as CountryIso2

  const handleChange = (_value: string, meta: { country: ParsedCountry; inputValue: string }) => {
    onCountryChange(meta.country.iso2.toUpperCase())
    const max = phoneMaxForCountry(meta.country.iso2)
    onPhoneChange(digitsOnly(meta.inputValue).slice(0, max))
  }

  return (
    <PhoneInput
      defaultCountry={defaultCountry}
      countries={countries}
      preferredCountries={preferredCountries}
      value={phone}
      onChange={handleChange}
      disableDialCodeAndPrefix
      disableFormatting
      disabled={disabled}
      placeholder={placeholder}
      className={`phone-country-input${invalid ? ' phone-country-input--invalid' : ''}`}
      inputProps={{
        id,
        'aria-invalid': invalid,
        inputMode: 'tel',
        autoComplete: 'tel-national',
      }}
    />
  )
}
