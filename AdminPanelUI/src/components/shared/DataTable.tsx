import { LayoutGrid } from 'lucide-react'
import { useState } from 'react'

import { DataTableCardView } from '@/components/shared/DataTableCardView'
import { DataTablePagination } from '@/components/shared/DataTablePagination'
import { renderDataTableCell } from '@/components/shared/dataTableRender'
import { DataTableToolbar } from '@/components/shared/DataTableToolbar'
import type { DataTableProps, DataTableViewMode } from '@/components/shared/dataTableTypes'
import { cn } from '@/lib/utils'

export function DataTable<T>({
  data,
  columns,
  getRowKey,
  emptyMessage = 'لا توجد بيانات',
  title,
  showExport = true,
  isExporting = false,
  onExport,
  toolbar,
  className,
  showViewSwitcher = true,
  defaultViewMode = 'list',
  viewMode: controlledViewMode,
  onViewModeChange,
  renderCard,
  pagination,
}: DataTableProps<T>) {
  const [internalViewMode, setInternalViewMode] = useState<DataTableViewMode>(defaultViewMode)
  const viewMode = controlledViewMode ?? internalViewMode
  const canExport = showExport && Boolean(onExport)
  const showToolbar = Boolean(title || toolbar || showExport || showViewSwitcher)

  const handleViewModeChange = (mode: DataTableViewMode) => {
    if (controlledViewMode === undefined) {
      setInternalViewMode(mode)
    }
    onViewModeChange?.(mode)
  }

  return (
    <div
      className={cn(
        'overflow-hidden rounded-2xl bg-white ring-1 ring-slate-200/70',
        'shadow-[0_1px_2px_rgba(15,23,42,0.04),0_8px_24px_rgba(15,23,42,0.06)]',
        className,
      )}
    >
      {showToolbar && (
        <div className="border-b border-slate-100 bg-gradient-to-b from-slate-50/80 to-white">
          <DataTableToolbar
            title={title}
            toolbar={toolbar}
            showExport={showExport}
            canExport={canExport}
            isExporting={isExporting}
            onExport={onExport}
            showViewSwitcher={showViewSwitcher}
            viewMode={viewMode}
            onViewModeChange={handleViewModeChange}
          />
        </div>
      )}

      {data.length === 0 ? (
        <DataTableEmpty message={emptyMessage} />
      ) : viewMode === 'card' ? (
        <DataTableCardView
          data={data}
          columns={columns}
          getRowKey={getRowKey}
          renderCard={renderCard}
        />
      ) : (
        <DataTableListView data={data} columns={columns} getRowKey={getRowKey} />
      )}

      {pagination && <DataTablePagination {...pagination} />}
    </div>
  )
}

function DataTableListView<T>({
  data,
  columns,
  getRowKey,
}: Pick<DataTableProps<T>, 'data' | 'columns' | 'getRowKey'>) {
  return (
    <div className="overflow-x-auto">
      <table className="min-w-full border-separate border-spacing-0 text-sm">
        <thead>
          <tr>
            {columns.map((column, index) => (
              <th
                key={column.id}
                className={cn(
                  'sticky top-0 z-10 border-b border-slate-200 bg-slate-50/95 px-5 py-3.5 text-right',
                  'text-xs font-semibold tracking-wide text-slate-600 backdrop-blur-sm',
                  index === 0 && 'rounded-ss-none',
                  column.headerClassName,
                )}
              >
                {column.header}
              </th>
            ))}
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100">
          {data.map((row, index) => (
            <tr
              key={getRowKey(row, index)}
              className="group transition-colors hover:bg-[var(--color-primary)]/[0.04]"
            >
              {columns.map((column) => (
                <td
                  key={column.id}
                  className={cn(
                    'px-5 py-3.5 text-right text-slate-700',
                    'group-hover:text-slate-900',
                    column.className,
                  )}
                >
                  {renderDataTableCell(row, column)}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

function DataTableEmpty({ message }: { message: string }) {
  return (
    <div className="flex flex-col items-center justify-center gap-3 px-6 py-16">
      <div className="flex size-14 items-center justify-center rounded-2xl bg-slate-100 ring-1 ring-slate-200/60">
        <LayoutGrid className="size-7 text-slate-400" strokeWidth={1.5} />
      </div>
      <p className="max-w-xs text-center text-sm leading-relaxed text-slate-500">{message}</p>
    </div>
  )
}
