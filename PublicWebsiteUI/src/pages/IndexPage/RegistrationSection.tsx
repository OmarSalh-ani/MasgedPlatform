import { useEffect, useMemo, useState } from 'react'
import { useMutation, useQuery } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { SectionHeader } from '@/components/SectionHeader'
import {
  getCountryDialCodes,
  submitRegistration,
} from '@/services/publicIndexService'
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

export function RegistrationForm({ mode, config }: RegistrationFormProps) {
  const navigate = useNavigate()
  const countriesQuery = useQuery({ queryKey: ['country-dial-codes'], queryFn: getCountryDialCodes })
  const [fullName, setFullName] = useState('')
  const [birthdate, setBirthdate] = useState('')
  const [ageValue, setAgeValue] = useState('')
  const [computedAge, setComputedAge] = useState('')
  const [countryIso, setCountryIso] = useState('KW')
  const [parentPhone1, setParentPhone1] = useState('')
  const [parentPhone2, setParentPhone2] = useState('')
  const [learnCertificate, setLearnCertificate] = useState('')
  const [activityId, setActivityId] = useState('')
  const [errors, setErrors] = useState<Record<string, string>>({})
  const [submitting, setSubmitting] = useState(false)

  const phoneLocked = !countryIso || countryIso.length < 2
  const phoneMax = countryIso === 'KW' ? 8 : 15
  const labels = config.labels

  useEffect(() => {
    if (birthdate) setComputedAge(String(Math.max(calculateAge(birthdate), 0)))
  }, [birthdate])

  const mutation = useMutation({
    mutationFn: submitRegistration,
    onSuccess: () => navigate('/register-success'),
  })

  const countryOptions = useMemo(() => countriesQuery.data ?? [], [countriesQuery.data])

  const validate = () => {
    const nextErrors: Record<string, string> = {}
    let valid = true

    if (!fullName.trim()) {
      nextErrors.fullName = 'required'
      valid = false
    }

    let age: number | undefined
    if (labels.showAgeDiv) {
      const parsed = parseInt(ageValue, 10)
      if (!ageValue || parsed < 5) {
        nextErrors.age = 'required'
        valid = false
      } else age = parsed
    } else if (!birthdate) {
      nextErrors.birthdate = 'يرجى إدخال تاريخ الميلاد'
      valid = false
    } else {
      age = calculateAge(birthdate)
    }

    if (!countryIso || countryIso.length < 2) {
      nextErrors.country = 'required'
      valid = false
    } else if (!parentPhone1.trim()) {
      nextErrors.parentPhone1 = 'required'
      valid = false
    } else {
      const phoneDigits = digitsOnly(parentPhone1)
      const phoneOk = countryIso === 'KW'
        ? phoneDigits.length === 8
        : phoneDigits.length >= 7 && phoneDigits.length <= 15
      if (!phoneOk) {
        nextErrors.parentPhone1 = countryIso === 'KW'
          ? 'يجب أن يكون رقم الهاتف 8 أرقام (رقم كويتي صحيح)'
          : 'أدخل رقم الجوال بدون رمز الدولة (7–15 رقماً)'
        valid = false
      }
    }

    if (typeof age === 'number' && age < 5) {
      window.alert('السلام عليكم ورحمة الله وبركاته\n\nعذراً، يجب أن لا يقل العمر عن 5 سنوات\n\nجزاكم الله خيراً')
      return false
    }

    if (!activityId) {
      window.alert('يرجى أختيار نوع النشاط')
      return false
    }

    if (parentPhone2.trim()) {
      const phone2Digits = digitsOnly(parentPhone2)
      if (phone2Digits.length !== 8) {
        nextErrors.parentPhone2 = 'يجب أن يكون رقم الهاتف 8 أرقام (رقم كويتي صحيح)'
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
        learnCertificate: learnCertificate || undefined,
        womanActivityTypeId: Number(activityId),
      }
      if (labels.showAgeDiv) payload.age = parseInt(ageValue, 10)
      else payload.birthdate = birthdate

      await mutation.mutateAsync(payload)
    } catch {
      window.alert('حدث خطأ أثناء الإرسال. يرجى المحاولة مرة أخرى.')
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="registration-card">
      <form noValidate onSubmit={(e) => e.preventDefault()}>
        <div className="form-grid">
          <div className="form-field form-field--full">
            <label htmlFor="fullName" className="form-label" id="FullNameLbl">{labels.fullNameLabel}</label>
            <input
              type="text"
              className={`form-input${errors.fullName ? ' is-invalid' : ''}`}
              id="fullName"
              value={fullName}
              onChange={(e) => setFullName(e.target.value)}
              required
            />
          </div>

          <div className={`form-grid form-grid--2 form-field--full${labels.showBirthdateDiv || labels.showLearnDiv || labels.showAgeDiv || labels.showPhone2Div ? '' : ''}`}>
            {labels.showBirthdateDiv && (
              <div className="form-field" id="BirthdateDiv">
                <label htmlFor="birthDate" className="form-label">تاريخ الميلاد *</label>
                <input
                  type="date"
                  className={`form-input${errors.birthdate ? ' is-invalid' : ''}`}
                  id="birthDate"
                  value={birthdate}
                  onChange={(e) => setBirthdate(e.target.value)}
                />
                {errors.birthdate && <span className="form-error">{errors.birthdate}</span>}
              </div>
            )}

            {labels.showAgeDiv && (
              <div className="form-field" id="AgeDiv">
                <label htmlFor="ageInput" className="form-label">العمر *</label>
                <input
                  type="number"
                  className={`form-input${errors.age ? ' is-invalid' : ''}`}
                  id="ageInput"
                  min={5}
                  max={100}
                  value={ageValue}
                  onChange={(e) => setAgeValue(e.target.value)}
                />
              </div>
            )}

            {labels.showLearnDiv && (
              <div className="form-field" id="LearnDiv">
                <label htmlFor="LearnCertificate" className="form-label" id="LearnCertificateLabel">
                  {labels.learnCertificateLabel}
                </label>
                <input
                  type="text"
                  className="form-input"
                  id="LearnCertificate"
                  value={learnCertificate}
                  onChange={(e) => setLearnCertificate(e.target.value)}
                />
              </div>
            )}

            {labels.showPhone2Div && (
              <div className="form-field" id="Phone2Div">
                <label htmlFor="parentPhone2" className="form-label">رقم هاتف ولي الأمر 2 (اختياري)</label>
                <input
                  type="tel"
                  className={`form-input${errors.parentPhone2 ? ' is-invalid' : ''}`}
                  id="parentPhone2"
                  value={parentPhone2}
                  onChange={(e) => setParentPhone2(digitsOnly(e.target.value).slice(0, 8))}
                />
                {errors.parentPhone2 && <span className="form-error">{errors.parentPhone2}</span>}
              </div>
            )}
          </div>

          <input type="hidden" id="age" value={computedAge} readOnly aria-hidden="true" />

          <div className="form-field form-field--full">
            <label htmlFor="parentPhone1" className="form-label" id="ParentPhone1Lbl">{labels.parentPhone1Label}</label>
            <div className="phone-group">
              <select
                className={`form-select${errors.country ? ' is-invalid' : ''}`}
                id="parentPhone1Country"
                value={countryIso}
                onChange={(e) => setCountryIso(e.target.value)}
                aria-label="رمز الدولة"
              >
                <option value="">— اختر رمز / بلد —</option>
                {countryOptions.map((country) => (
                  <option key={country.code} value={country.code}>
                    {country.dial_code ? `${country.name} (${country.dial_code})` : country.name}
                  </option>
                ))}
              </select>
              <input
                type="tel"
                className={`form-input${errors.parentPhone1 ? ' is-invalid' : ''}${phoneLocked ? ' phone-locked' : ''}`}
                id="parentPhone1"
                value={parentPhone1}
                readOnly={phoneLocked}
                placeholder={phoneLocked ? 'اختر رمز الدولة أولاً' : 'مثال: 51234567'}
                onChange={(e) => setParentPhone1(digitsOnly(e.target.value).slice(0, phoneMax))}
              />
            </div>
            {errors.parentPhone1 && errors.parentPhone1 !== 'required' && (
              <span className="form-error">{errors.parentPhone1}</span>
            )}
            <p className="form-hint">
              اختر رمز الدولة أولاً. الافتراضي: الكويت. يُخزَّن الرقم كاملاً مع رمز الدولة (مثل +96551234567).
            </p>
          </div>

          <div className="form-field form-field--full">
            <label htmlFor="WomanActivityType" className="form-label">نوع النشاط *</label>
            <select
              className="form-select"
              id="WomanActivityType"
              value={activityId}
              onChange={(e) => setActivityId(e.target.value)}
            >
              <option value="">النشاط</option>
              {config.womanActivities.map((activity) => (
                <option key={activity.id} value={activity.id}>{activity.name}</option>
              ))}
            </select>
          </div>

          <div className="form-field form-field--full">
            <button
              type="button"
              className="btn btn-primary btn-lg btn-block"
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
        </div>
      </form>
    </div>
  )
}

export function RegistrationSection({
  mode,
  config,
}: {
  mode: RegistrationMode
  config: PublicRegistrationConfig
}) {
  if (!config.registrationEnabled) {
    return (
      <section className="section registration-section" id="registrationClosedSection">
        <div className="container">
          <div className="registration-closed">
            <div className="registration-closed-icon">
              <i className="fas fa-info-circle" aria-hidden="true" />
            </div>
            <h3>التسجيل مغلق حالياً</h3>
            <p>نعتذر، التسجيل في الأنشطة مغلق حالياً. يرجى المحاولة لاحقاً أو التواصل معنا للاستفسار.</p>
          </div>
        </div>
      </section>
    )
  }

  return (
    <section id="registration" className="section registration-section">
      <div className="container">
        <SectionHeader
          badge="التسجيل"
          title="تسجيل الطلاب"
          subtitle="سجل الآن في الأنشطة المتاحة"
        />
        <RegistrationForm mode={mode} config={config} />
      </div>
    </section>
  )
}
