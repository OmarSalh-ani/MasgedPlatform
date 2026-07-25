import { Plus } from 'lucide-react'
import { Link } from 'react-router-dom'
import { Input } from '@/components/ui/input'

interface CirclesFiltersProps {
  search: string
  onSearchChange: (value: string) => void
  canModify: boolean
}

export function CirclesFilters({ search, onSearchChange, canModify }: CirclesFiltersProps) {
  return (
    <div className="mb-6 rounded-xl bg-white p-5 shadow-md">
      <div className="flex flex-wrap items-center gap-3">
        <Input
          value={search}
          onChange={(event) => onSearchChange(event.target.value)}
          placeholder="البحث في الحلقات..."
          className="max-w-md rounded-full"
        />
        {canModify && (
          <Link
            to="/circles/new"
            className="inline-flex items-center gap-2 rounded-full bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a] px-5 py-2.5 font-semibold text-white hover:opacity-90"
          >
            <Plus className="size-4" />
            إضافة حلقة جديدة
          </Link>
        )}
      </div>
    </div>
  )
}
