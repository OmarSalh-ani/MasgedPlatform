type Option = { label: string; value: string }

type Props = {
  name: string
  value: string
  options: readonly Option[]
  onChange: (value: string) => void
}

export function ParentsFollowupRadioGroup({ name, value, options, onChange }: Props) {
  return (
    <div className="radio-group">
      {options.map((option) => (
        <label key={option.value} className="radio-item">
          <input
            type="radio"
            name={name}
            value={option.value}
            checked={value === option.value}
            onChange={() => onChange(option.value)}
          />
          {option.label}
        </label>
      ))}
    </div>
  )
}
