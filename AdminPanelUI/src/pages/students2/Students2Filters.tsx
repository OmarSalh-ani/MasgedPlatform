import { X } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'

interface Students2FiltersProps {
  search: string
  onSearchChange: (value: string) => void
  onClear: () => void
}

export function Students2Filters({ search, onSearchChange, onClear }: Students2FiltersProps) {
  return (
    <section className="mb-8">
      <div className="grid gap-3 md:grid-cols-[2fr_1fr]">
        <Input
          value={search}
          placeholder="ابحث عن طالب بالاسم..."
          className="h-12 rounded-full border-2 px-5 text-base"
          onChange={(event) => onSearchChange(event.target.value)}
        />
        <Button type="button" variant="outline" className="h-12 rounded-full" onClick={onClear}>
          <X className="size-4" />
          مسح البحث
        </Button>
      </div>
    </section>
  )
}
