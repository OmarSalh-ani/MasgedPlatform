import { FileSpreadsheet, Plus } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'

interface WomansActivitiesFiltersProps {
  search: string
  onSearchChange: (value: string) => void
  canModify: boolean
  feminineTheme: boolean
  onAdd: () => void
  onExport: () => void
  isExporting: boolean
}

export function WomansActivitiesFilters({
  search,
  onSearchChange,
  canModify,
  feminineTheme,
  onAdd,
  onExport,
  isExporting,
}: WomansActivitiesFiltersProps) {
  const addBtnClass = feminineTheme
    ? 'bg-gradient-to-br from-pink-600 to-pink-800 hover:opacity-90'
    : 'bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a] hover:opacity-90'

  return (
    <div className="mb-6 rounded-xl bg-white p-5 shadow-md">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div className="flex min-w-[240px] flex-1 flex-wrap items-center gap-3">
          <Input
            value={search}
            onChange={(event) => onSearchChange(event.target.value)}
            placeholder="البحث في النشاطات..."
            className="max-w-md rounded-full"
          />
          {canModify && (
            <Button type="button" className={`rounded-full ${addBtnClass}`} onClick={onAdd}>
              <Plus className="size-4" />
              إضافة نشاط جديد
            </Button>
          )}
        </div>
        <Button
          type="button"
          variant="outline"
          className="rounded-full"
          disabled={isExporting}
          onClick={onExport}
        >
          <FileSpreadsheet className="size-4" />
          {isExporting ? 'جاري التصدير...' : 'تصدير إلى Excel'}
        </Button>
      </div>
    </div>
  )
}
