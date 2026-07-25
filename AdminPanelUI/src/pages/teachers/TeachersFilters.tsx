import { FileSpreadsheet, Plus } from 'lucide-react'
import { Link } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'

interface TeachersFiltersProps {
  search: string
  onSearchChange: (value: string) => void
  canModify: boolean
  onExport: () => void
  isExporting: boolean
}

export function TeachersFilters({
  search,
  onSearchChange,
  canModify,
  onExport,
  isExporting,
}: TeachersFiltersProps) {
  return (
    <div className="mb-6 rounded-xl bg-white p-5 shadow-md">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div className="flex min-w-[240px] flex-1 flex-wrap items-center gap-3">
          <Input
            value={search}
            onChange={(event) => onSearchChange(event.target.value)}
            placeholder="البحث في المعلمين..."
            className="max-w-md rounded-full"
          />
          {canModify && (
            <Link
              to="/teachers/new"
              className="inline-flex items-center gap-2 rounded-full bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a] px-5 py-2.5 font-semibold text-white hover:opacity-90"
            >
              <Plus className="size-4" />
              إضافة معلم جديد
            </Link>
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
