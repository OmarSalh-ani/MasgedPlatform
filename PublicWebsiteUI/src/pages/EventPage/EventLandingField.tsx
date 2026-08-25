import type { PublicEventPageFormField } from '@/types/eventPage'

interface EventLandingFieldProps {
  field: PublicEventPageFormField
  value: string | string[] | undefined
  error?: string
  onChange: (value: string | string[]) => void
}

function toggleValue(current: string[], option: string) {
  return current.includes(option)
    ? current.filter((item) => item !== option)
    : [...current, option]
}

export function EventLandingField({ field, value, error, onChange }: EventLandingFieldProps) {
  const requiredMark = field.isRequired ? ' *' : ''

  if (field.fieldType === 'SingleSelect') {
    return (
      <div className={`event-field ${error ? 'event-field--invalid' : ''}`}>
        <label>{field.label}{requiredMark}</label>
        <div className="event-field__chips">
          {field.options.map((option) => (
            <button
              key={option}
              type="button"
              className={`event-chip ${value === option ? 'is-active' : ''}`}
              onClick={() => onChange(option)}
            >
              {option}
            </button>
          ))}
        </div>
        {error && <span className="event-field__error">{error}</span>}
      </div>
    )
  }

  if (field.fieldType === 'MultiSelect') {
    const selected = Array.isArray(value) ? value : []
    return (
      <div className={`event-field ${error ? 'event-field--invalid' : ''}`}>
        <label>{field.label}{requiredMark}</label>
        <div className="event-field__chips">
          {field.options.map((option) => (
            <button
              key={option}
              type="button"
              className={`event-chip ${selected.includes(option) ? 'is-active' : ''}`}
              onClick={() => onChange(toggleValue(selected, option))}
            >
              {option}
            </button>
          ))}
        </div>
        {error && <span className="event-field__error">{error}</span>}
      </div>
    )
  }

  return (
    <div className={`event-field ${error ? 'event-field--invalid' : ''}`}>
      <label htmlFor={`event-field-${field.id}`}>{field.label}{requiredMark}</label>
      <input
        id={`event-field-${field.id}`}
        type={field.fieldType === 'Number' ? 'number' : 'text'}
        value={typeof value === 'string' ? value : ''}
        onChange={(event) => onChange(event.target.value)}
      />
      {error && <span className="event-field__error">{error}</span>}
    </div>
  )
}
