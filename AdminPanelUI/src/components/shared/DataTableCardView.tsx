import type { ReactNode } from 'react'

import { renderDataTableCell } from '@/components/shared/dataTableRender'
import type { DataTableColumn } from '@/components/shared/dataTableTypes'
import { cn } from '@/lib/utils'

interface DataTableCardViewProps<T> {
  data: T[]
  columns: DataTableColumn<T>[]
  getRowKey: (row: T, index: number) => string
  renderCard?: (row: T) => ReactNode
}

export function DataTableCardView<T>({
  data,
  columns,
  getRowKey,
  renderCard,
}: DataTableCardViewProps<T>) {
  return (
    <div className="grid gap-4 p-5 sm:grid-cols-2 xl:grid-cols-3">
      {data.map((row, index) =>
        renderCard ? (
          <div key={getRowKey(row, index)}>{renderCard(row)}</div>
        ) : (
          <article
            key={getRowKey(row, index)}
            className={cn(
              'overflow-hidden rounded-xl border border-slate-200/80 bg-white',
              'shadow-sm transition-shadow hover:shadow-md',
            )}
          >
            <DefaultDataTableCard row={row} columns={columns} />
          </article>
        ),
      )}
    </div>
  )
}

function DefaultDataTableCard<T>({
  row,
  columns,
}: {
  row: T
  columns: DataTableColumn<T>[]
}) {
  const [primaryColumn, ...detailColumns] = columns

  return (
    <div className="divide-y divide-slate-100">
      {primaryColumn && (
        <div className="bg-gradient-to-b from-slate-50 to-white px-4 py-3.5">
          <p className="text-xs font-medium text-slate-500">{primaryColumn.header}</p>
          <div className="mt-1 text-base font-semibold text-slate-900">
            {renderDataTableCell(row, primaryColumn)}
          </div>
        </div>
      )}

      <dl className="space-y-2.5 px-4 py-3.5 text-sm">
        {(primaryColumn ? detailColumns : columns).map((column) => (
          <div key={column.id} className="flex items-start justify-between gap-3">
            <dt className="shrink-0 text-slate-500">{column.header}</dt>
            <dd className={cn('text-left text-slate-800', column.className)}>
              {renderDataTableCell(row, column)}
            </dd>
          </div>
        ))}
      </dl>
    </div>
  )
}
