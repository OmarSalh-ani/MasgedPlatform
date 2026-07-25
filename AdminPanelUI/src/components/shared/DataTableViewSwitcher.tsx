import { LayoutGrid, List } from 'lucide-react'

import { Button } from '@/components/ui/button'
import type { DataTableViewMode } from '@/components/shared/dataTableTypes'

interface DataTableViewSwitcherProps {
  viewMode: DataTableViewMode
  onViewModeChange: (mode: DataTableViewMode) => void
}

export function DataTableViewSwitcher({
  viewMode,
  onViewModeChange,
}: DataTableViewSwitcherProps) {
  return (
    <div className="inline-flex rounded-lg border border-slate-200 bg-white p-1">
      <Button
        type="button"
        size="sm"
        variant={viewMode === 'list' ? 'default' : 'outline'}
        onClick={() => onViewModeChange('list')}
        aria-pressed={viewMode === 'list'}
        className="gap-1.5"
      >
        <List className="size-4" />
        قائمة
      </Button>
      <Button
        type="button"
        size="sm"
        variant={viewMode === 'card' ? 'default' : 'outline'}
        onClick={() => onViewModeChange('card')}
        aria-pressed={viewMode === 'card'}
        className="gap-1.5"
      >
        <LayoutGrid className="size-4" />
        بطاقات
      </Button>
    </div>
  )
}
