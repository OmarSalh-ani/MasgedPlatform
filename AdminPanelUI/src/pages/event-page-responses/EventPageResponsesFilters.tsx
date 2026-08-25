import { Filter } from 'lucide-react'
import { SearchableDropdown } from '@/components/shared/SearchableDropdown'
import type { EventPageLookup } from '@/types/eventPage'

interface EventPageResponsesFiltersProps {
  activityName: string
  lookups: EventPageLookup[]
  onActivityNameChange: (value: string) => void
}

export function EventPageResponsesFilters({
  activityName,
  lookups,
  onActivityNameChange,
}: EventPageResponsesFiltersProps) {
  const options = [
    { value: '', label: 'جميع الأنشطة' },
    ...lookups.map((item) => ({ value: item.activityName, label: item.activityName })),
  ]

  return (
    <section className="rounded-xl border bg-white p-5 shadow-sm">
      <h2 className="mb-4 flex items-center gap-2 text-lg font-semibold text-[var(--color-primary)]">
        <Filter className="size-5" />
        فلاتر البحث
      </h2>
      <div className="max-w-md">
        <label className="mb-2 block text-sm font-medium text-slate-700">اسم النشاط</label>
        <SearchableDropdown
          value={activityName}
          onChange={onActivityNameChange}
          options={options}
          placeholder="جميع الأنشطة"
        />
      </div>
    </section>
  )
}
