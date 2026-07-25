import { SOCIAL_ICON_OPTIONS } from '@/pages/social-links/socialIconOptions'

interface SocialIconPickerProps {
  value: string
  onChange: (iconClass: string) => void
}

export function SocialIconPicker({ value, onChange }: SocialIconPickerProps) {
  const selected = value.trim()

  return (
    <div className="flex flex-wrap gap-2.5">
      {SOCIAL_ICON_OPTIONS.map((option) => {
        const isSelected = selected === option.iconClass
        return (
          <button
            key={option.iconClass}
            type="button"
            title={option.title}
            className={`flex size-[52px] items-center justify-center rounded-xl border-2 text-2xl transition-all ${
              isSelected
                ? 'border-[#7C8738] bg-[rgba(124,135,56,0.2)] text-[#7C8738] shadow-[0_0_0_2px_rgba(124,135,56,0.3)]'
                : 'border-slate-200 bg-slate-50 text-slate-600 hover:border-[#7C8738] hover:bg-[rgba(124,135,56,0.1)] hover:text-[#7C8738]'
            }`}
            onClick={() => onChange(option.iconClass)}
          >
            <i className={option.iconClass} aria-hidden />
          </button>
        )
      })}
    </div>
  )
}
