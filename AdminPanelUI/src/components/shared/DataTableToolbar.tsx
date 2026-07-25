import { FileSpreadsheet, Loader2 } from 'lucide-react'

import { DataTableViewSwitcher } from '@/components/shared/DataTableViewSwitcher'
import type { DataTableViewMode } from '@/components/shared/dataTableTypes'
import { cn } from '@/lib/utils'

interface DataTableToolbarProps {
  title?: string
  toolbar?: React.ReactNode
  showExport: boolean
  canExport: boolean
  isExporting: boolean
  onExport?: () => void
  showViewSwitcher?: boolean
  viewMode?: DataTableViewMode
  onViewModeChange?: (mode: DataTableViewMode) => void
}

export function DataTableToolbar({
  title,
  toolbar,
  showExport,
  canExport,
  isExporting,
  onExport,
  showViewSwitcher = false,
  viewMode = 'list',
  onViewModeChange,
}: DataTableToolbarProps) {
  return (
    <div className="flex flex-wrap items-center justify-between gap-4 px-5 py-4 sm:px-6">
      <div className="flex min-w-0 flex-1 flex-wrap items-center gap-3">
        {title && (
          <h3 className="text-base font-semibold tracking-tight text-slate-900">{title}</h3>
        )}
        {toolbar}
      </div>
      <div className="flex shrink-0 flex-wrap items-center gap-3">
        {showViewSwitcher && onViewModeChange && (
          <DataTableViewSwitcher viewMode={viewMode} onViewModeChange={onViewModeChange} />
        )}
        {showExport && (
        <button
          type="button"
          disabled={!canExport || isExporting}
          onClick={onExport}
          title={isExporting ? 'جاري التصدير...' : 'تصدير إلى إكسل'}
          aria-label="تصدير إلى إكسل"
          className={cn(
            'inline-flex shrink-0 items-center gap-2 rounded-xl px-3.5 py-2 text-sm font-medium transition-all',
            'border border-emerald-200/80 bg-emerald-50 text-emerald-800',
            'hover:border-emerald-300 hover:bg-emerald-100 hover:shadow-sm',
            'focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500/40',
            'disabled:cursor-not-allowed disabled:opacity-50 disabled:hover:shadow-none',
          )}
        >
          {isExporting ? (
            <Loader2 className="size-4 animate-spin" />
          ) : (
            <FileSpreadsheet className="size-4" />
          )}
          <span>{isExporting ? 'جاري التصدير...' : 'تصدير إلى إكسل'}</span>
        </button>
        )}
      </div>
    </div>
  )
}
