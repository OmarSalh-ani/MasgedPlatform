import { useMemo, useState, type FormEvent } from 'react'
import type { PublicEventPageFormField } from '@/types/eventPage'
import { EventLandingField } from '@/pages/EventPage/EventLandingField'

interface EventLandingFormProps {
  fields: PublicEventPageFormField[]
  isSubmitting: boolean
  errorMessage: string | null
  onSubmit: (answers: Record<number, string | string[]>) => void
}

function validate(
  fields: PublicEventPageFormField[],
  answers: Record<number, string | string[]>,
): Record<number, string> {
  const errors: Record<number, string> = {}

  for (const field of fields) {
    const value = answers[field.id]
    const isEmpty = Array.isArray(value)
      ? value.length === 0
      : !String(value ?? '').trim()

    if (field.isRequired && isEmpty) {
      errors[field.id] = 'هذا الحقل مطلوب'
      continue
    }

    if (field.fieldType === 'Number' && !isEmpty) {
      const numeric = String(value).trim()
      if (Number.isNaN(Number(numeric))) errors[field.id] = 'يجب إدخال رقم صحيح'
    }
  }

  return errors
}

export function EventLandingForm({
  fields,
  isSubmitting,
  errorMessage,
  onSubmit,
}: EventLandingFormProps) {
  const sorted = useMemo(
    () => [...fields].sort((a, b) => a.sortOrder - b.sortOrder || a.id - b.id),
    [fields],
  )
  const [answers, setAnswers] = useState<Record<number, string | string[]>>({})
  const [errors, setErrors] = useState<Record<number, string>>({})

  const handleChange = (fieldId: number, value: string | string[]) => {
    setAnswers((current) => ({ ...current, [fieldId]: value }))
  }

  const handleSubmit = (event: FormEvent) => {
    event.preventDefault()
    const nextErrors = validate(sorted, answers)
    setErrors(nextErrors)
    if (Object.keys(nextErrors).length > 0) return
    onSubmit(answers)
  }

  return (
    <section className="event-form" id="event-register">
      <h2>التسجيل</h2>
      <form onSubmit={handleSubmit} noValidate>
        {sorted.map((field) => (
          <EventLandingField
            key={field.id}
            field={field}
            value={answers[field.id]}
            error={errors[field.id]}
            onChange={(value) => handleChange(field.id, value)}
          />
        ))}
        {errorMessage && <p className="event-form__error">{errorMessage}</p>}
        <button type="submit" className="btn btn-primary event-form__submit" disabled={isSubmitting}>
          {isSubmitting ? 'جاري الإرسال...' : 'إرسال التسجيل'}
        </button>
      </form>
    </section>
  )
}
