import { useEffect, useState } from 'react'
import { useMutation } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { PhoneCountryInput } from '@/components/PhoneCountryInput'
import { submitRegistration } from '@/services/publicIndexService'
import type {
  PublicRegistrationConfig,
  RegistrationMode,
  SubmitRegistrationPayload,
} from '@/types/publicIndex'

interface RegistrationFormProps {
  mode: RegistrationMode
  config: PublicRegistrationConfig
}

function digitsOnly(value: string) {
  return value.replace(/\D/g, '')
}

function calculateAge(birthdate: string) {
  const birthDate = new Date(birthdate)
  const today = new Date()
  let age = today.getFullYear() - birthDate.getFullYear()
  const monthDiff = today.getMonth() - birthDate.getMonth()
  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) age--
  return age
}

function isValidLocalPhone(countryIso: string, phone: string) {
  const phoneDigits = digitsOnly(phone)
  if (!phoneDigits) return false
  if (countryIso === 'KW') return phoneDigits.length === 8
  return phoneDigits.length >= 7 && phoneDigits.length <= 15
}

function phoneErrorMessage(countryIso: string) {
  return countryIso === 'KW'
    ? 'يجب أن يكون رقم الهاتف 8 أرقام (رقم كويتي صحيح)'
    : 'أدخل رقم الجوال بدون رمز الدولة (7–15 رقماً)'
}

function buildBirthdateIso(day: string, month: string, year: string): string | null {
  const d = parseInt(day, 10)
  const m = parseInt(month, 10)
  const y = parseInt(year, 10)

  if (!day || !month || !year || year.length !== 4) return null
  if (m < 1 || m > 12 || d < 1 || d > 31) return null

  const date = new Date(y, m - 1, d)
  if (date.getFullYear() !== y || date.getMonth() !== m - 1 || date.getDate() !== d) return null
  if (date > new Date()) return null

  return `${y}-${String(m).padStart(2, '0')}-${String(d).padStart(2, '0')}`
}

function sanitizeDatePartInput(value: string, maxLength: number, maxValue: number): string {
  const digits = digitsOnly(value).slice(0, maxLength)
  if (!digits) return ''

  if (digits.length === 1 && digits === '0') return '0'

  if (digits.length === 2 && digits.startsWith('0')) {
    const parsed = parseInt(digits, 10)
    if (parsed >= 1 && parsed <= maxValue) return digits
    return digits.slice(-1)
  }

  const parsed = parseInt(digits, 10)
  if (Number.isNaN(parsed)) return ''
  if (parsed > maxValue) return String(maxValue).padStart(2, '0')
  return digits
}

function padDatePartOnBlur(value: string, maxValue: number): string {
  const digits = digitsOnly(value)
  if (!digits) return ''

  const parsed = parseInt(digits, 10)
  if (Number.isNaN(parsed) || parsed < 1 || parsed > maxValue) return value

  return String(parsed).padStart(2, '0')
}

export function RegistrationForm({ mode, config }: RegistrationFormProps) {
  const navigate = useNavigate()
  const [fullName, setFullName] = useState('')
  const [birthDay, setBirthDay] = useState('')
  const [birthMonth, setBirthMonth] = useState('')
  const [birthYear, setBirthYear] = useState('')
  const [ageValue, setAgeValue] = useState('')
  const [computedAge, setComputedAge] = useState('')
  const [countryIso, setCountryIso] = useState('KW')
  const [parentPhone1, setParentPhone1] = useState('')
  const [countryIso2, setCountryIso2] = useState('KW')
  const [parentPhone2, setParentPhone2] = useState('')
  const [learnCertificate, setLearnCertificate] = useState('')
  const [activityId, setActivityId] = useState('')
  const [errors, setErrors] = useState<Record<string, string>>({})
  const [submitting, setSubmitting] = useState(false)

  const labels = config.labels

  useEffect(() => {
    const iso = buildBirthdateIso(birthDay, birthMonth, birthYear)
    if (iso) setComputedAge(String(Math.max(calculateAge(iso), 0)))
    else setComputedAge('')
  }, [birthDay, birthMonth, birthYear])

  const mutation = useMutation({
    mutationFn: submitRegistration,
    onSuccess: () => navigate('/register-success'),
  })

  const validate = () => {
    const nextErrors: Record<string, string> = {}
    let valid = true

    if (!fullName.trim()) {
      nextErrors.fullName = 'required'
      valid = false
    }

    let age: number | undefined
    let birthdateIso: string | null = null
    if (labels.showAgeDiv) {
      const parsed = parseInt(ageValue, 10)
      if (!ageValue || parsed < 5) {
        nextErrors.age = 'required'
        valid = false
      } else age = parsed
    } else {
      birthdateIso = buildBirthdateIso(birthDay, birthMonth, birthYear)
      if (!birthdateIso) {
        nextErrors.birthdate = 'يرجى إدخال تاريخ ميلاد صحيح'
        valid = false
      } else {
        age = calculateAge(birthdateIso)
      }
    }

    if (!countryIso || countryIso.length < 2) {
      nextErrors.country = 'required'
      valid = false
    } else if (!parentPhone1.trim()) {
      nextErrors.parentPhone1 = 'required'
      valid = false
    } else {
      const phoneDigits = digitsOnly(parentPhone1)
      if (!isValidLocalPhone(countryIso, phoneDigits)) {
        nextErrors.parentPhone1 = phoneErrorMessage(countryIso)
        valid = false
      }
    }

    if (typeof age === 'number' && age < 5) {
      window.alert('السلام عليكم ورحمة الله وبركاته\n\nعذراً، يجب أن لا يقل العمر عن 5 سنوات\n\nجزاكم الله خيراً')
      return false
    }

    if (!activityId) {
      nextErrors.activity = 'يرجى اختيار نوع النشاط'
      valid = false
    }

    if (parentPhone2.trim()) {
      if (!isValidLocalPhone(countryIso2, parentPhone2)) {
        nextErrors.parentPhone2 = phoneErrorMessage(countryIso2)
        valid = false
      }
    }

    setErrors(nextErrors)
    return valid
  }

  const onSubmit = async () => {
    if (!validate()) return
    setSubmitting(true)
    try {
      const payload: SubmitRegistrationPayload = {
        mode,
        fullName: fullName.trim(),
        parentPhoneCountryIso: countryIso,
        parentPhone1,
        parentPhone2: parentPhone2 || undefined,
        parentPhone2CountryIso: parentPhone2 ? countryIso2 : undefined,
        learnCertificate: learnCertificate || undefined,
        womanActivityTypeId: Number(activityId),
      }
      if (labels.showAgeDiv) payload.age = parseInt(ageValue, 10)
      else {
        const iso = buildBirthdateIso(birthDay, birthMonth, birthYear)
        if (iso) payload.birthdate = iso
      }

      await mutation.mutateAsync(payload)
    } catch {
      window.alert('حدث خطأ أثناء الإرسال. يرجى المحاولة مرة أخرى.')
    } finally {
      setSubmitting(false)
    }
  }

  const showPersonalExtras =
    labels.showBirthdateDiv || labels.showLearnDiv || labels.showAgeDiv

  return (
    <div className="reg-form-card">
      <form className="reg-form" noValidate onSubmit={(e) => e.preventDefault()}>
        <fieldset className="reg-form__section">
          <legend className="reg-form__section-title">
            <span className="reg-form__section-icon" aria-hidden="true">
              <i className="fas fa-user-graduate" />
            </span>
            البيانات الشخصية
          </legend>

          <div className="reg-form__field">
            <label htmlFor="fullName" className="reg-form__label" id="FullNameLbl">
              {labels.fullNameLabel}
            </label>
            <div className={`reg-form__input-wrap${errors.fullName ? ' reg-form__input-wrap--invalid' : ''}`}>
              <i className="fas fa-user reg-form__input-icon" aria-hidden="true" />
              <input
                type="text"
                className="reg-form__input"
                id="fullName"
                value={fullName}
                onChange={(e) => setFullName(e.target.value)}
                placeholder="الاسم الكامل"
                required
              />
            </div>
          </div>

          {showPersonalExtras && (
            <div className="reg-form__grid reg-form__grid--2">
              {labels.showBirthdateDiv && (
                <div className="reg-form__field reg-form__field--full" id="BirthdateDiv">
                  <span className="reg-form__label">تاريخ الميلاد *</span>
                  <div className={`reg-form__date-grid${errors.birthdate ? ' reg-form__date-grid--invalid' : ''}`}>
                    <div className="reg-form__date-part">
                      <label htmlFor="birthDay" className="reg-form__sublabel">اليوم</label>
                      <input
                        type="text"
                        className="reg-form__date-input"
                        id="birthDay"
                        inputMode="numeric"
                        autoComplete="bday-day"
                        placeholder="01"
                        maxLength={2}
                        value={birthDay}
                        onChange={(e) => setBirthDay(sanitizeDatePartInput(e.target.value, 2, 31))}
                        onBlur={() => setBirthDay((current) => padDatePartOnBlur(current, 31))}
                      />
                    </div>
                    <div className="reg-form__date-part">
                      <label htmlFor="birthMonth" className="reg-form__sublabel">الشهر</label>
                      <input
                        type="text"
                        className="reg-form__date-input"
                        id="birthMonth"
                        inputMode="numeric"
                        autoComplete="bday-month"
                        placeholder="01"
                        maxLength={2}
                        value={birthMonth}
                        onChange={(e) => setBirthMonth(sanitizeDatePartInput(e.target.value, 2, 12))}
                        onBlur={() => setBirthMonth((current) => padDatePartOnBlur(current, 12))}
                      />
                    </div>
                    <div className="reg-form__date-part">
                      <label htmlFor="birthYear" className="reg-form__sublabel">السنة</label>
                      <input
                        type="text"
                        className="reg-form__date-input"
                        id="birthYear"
                        inputMode="numeric"
                        autoComplete="bday-year"
                        placeholder="2010"
                        maxLength={4}
                        value={birthYear}
                        onChange={(e) => setBirthYear(digitsOnly(e.target.value).slice(0, 4))}
                      />
                    </div>
                  </div>
                  <input
                    type="hidden"
                    id="birthDate"
                    value={buildBirthdateIso(birthDay, birthMonth, birthYear) ?? ''}
                    readOnly
                    aria-hidden="true"
                  />
                  {errors.birthdate && <span className="reg-form__error">{errors.birthdate}</span>}
                </div>
              )}

              {labels.showAgeDiv && (
                <div className="reg-form__field" id="AgeDiv">
                  <label htmlFor="ageInput" className="reg-form__label">العمر *</label>
                  <div className={`reg-form__input-wrap${errors.age ? ' reg-form__input-wrap--invalid' : ''}`}>
                    <i className="fas fa-hashtag reg-form__input-icon" aria-hidden="true" />
                    <input
                      type="number"
                      className="reg-form__input"
                      id="ageInput"
                      min={5}
                      max={100}
                      value={ageValue}
                      onChange={(e) => setAgeValue(e.target.value)}
                      placeholder="5"
                    />
                  </div>
                </div>
              )}

              {labels.showLearnDiv && (
                <div className="reg-form__field" id="LearnDiv">
                  <label htmlFor="LearnCertificate" className="reg-form__label" id="LearnCertificateLabel">
                    {labels.learnCertificateLabel}
                  </label>
                  <div className="reg-form__input-wrap">
                    <i className="fas fa-certificate reg-form__input-icon" aria-hidden="true" />
                    <input
                      type="text"
                      className="reg-form__input"
                      id="LearnCertificate"
                      value={learnCertificate}
                      onChange={(e) => setLearnCertificate(e.target.value)}
                    />
                  </div>
                </div>
              )}

            </div>
          )}
        </fieldset>

        <input type="hidden" id="age" value={computedAge} readOnly aria-hidden="true" />

        <fieldset className="reg-form__section">
          <legend className="reg-form__section-title">
            <span className="reg-form__section-icon" aria-hidden="true">
              <i className="fas fa-mobile-alt" />
            </span>
            التواصل
          </legend>

          <div className="reg-form__field">
            <label htmlFor="parentPhone1" className="reg-form__label" id="ParentPhone1Lbl">
              {labels.parentPhone1Label}
            </label>
            <PhoneCountryInput
              id="parentPhone1"
              countryIso={countryIso}
              phone={parentPhone1}
              onCountryChange={setCountryIso}
              onPhoneChange={setParentPhone1}
              invalid={Boolean(errors.country || (errors.parentPhone1 && errors.parentPhone1 !== 'required'))}
              placeholder={countryIso === 'KW' ? '51234567' : 'رقم الجوال'}
            />
            {errors.parentPhone1 && errors.parentPhone1 !== 'required' && (
              <span className="reg-form__error">{errors.parentPhone1}</span>
            )}
          </div>

          {labels.showPhone2Div && (
            <div className="reg-form__field" id="Phone2Div">
              <label htmlFor="parentPhone2" className="reg-form__label">
                رقم هاتف ولي الأمر 2 (اختياري)
              </label>
              <PhoneCountryInput
                id="parentPhone2"
                countryIso={countryIso2}
                phone={parentPhone2}
                onCountryChange={setCountryIso2}
                onPhoneChange={setParentPhone2}
                invalid={Boolean(errors.parentPhone2)}
                placeholder={countryIso2 === 'KW' ? '51234567' : 'رقم الجوال'}
              />
              {errors.parentPhone2 && (
                <span className="reg-form__error">{errors.parentPhone2}</span>
              )}
            </div>
          )}
        </fieldset>

        <fieldset className="reg-form__section">
          <legend className="reg-form__section-title">
            <span className="reg-form__section-icon" aria-hidden="true">
              <i className="fas fa-list-check" />
            </span>
            النشاط
          </legend>

          <div className="reg-form__field">
            <span className="reg-form__label" id="WomanActivityTypeLabel">نوع النشاط *</span>
            <div
              className={`reg-form__chips${errors.activity ? ' reg-form__chips--invalid' : ''}`}
              role="radiogroup"
              aria-labelledby="WomanActivityTypeLabel"
            >
              {config.womanActivities.map((activity) => {
                const selected = activityId === String(activity.id)
                return (
                  <button
                    key={activity.id}
                    type="button"
                    role="radio"
                    aria-checked={selected}
                    id={selected ? 'WomanActivityType' : undefined}
                    className={`reg-form__chip${selected ? ' reg-form__chip--selected' : ''}`}
                    onClick={() => setActivityId(String(activity.id))}
                  >
                    {activity.name}
                  </button>
                )
              })}
            </div>
            {errors.activity && <span className="reg-form__error">{errors.activity}</span>}
          </div>
        </fieldset>

        <div className="reg-form__actions">
          <button
            type="button"
            className="btn btn-primary btn-lg reg-form__submit"
            id="RegisterClientBtn"
            disabled={submitting}
            onClick={onSubmit}
          >
            {submitting ? (
              <>
                <span className="form-spinner" aria-hidden="true" />
                جاري الإرسال...
              </>
            ) : (
              <>
                <i className="fas fa-paper-plane" aria-hidden="true" />
                إرسال طلب التسجيل
              </>
            )}
          </button>
        </div>
      </form>
    </div>
  )
}
