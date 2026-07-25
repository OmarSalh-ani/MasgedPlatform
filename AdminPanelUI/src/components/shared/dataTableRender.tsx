import type { ReactNode } from 'react'

import type { DataTableColumn } from '@/components/shared/dataTableTypes'

export function renderDataTableCell<T>(row: T, column: DataTableColumn<T>) {
  if (column.cell) return column.cell(row)
  return formatDataTableCellValue(getColumnDisplayValue(row, column))
}

function getColumnDisplayValue<T>(
  row: T,
  column: DataTableColumn<T>,
): string | number | null | undefined {
  if (!column.accessor) return ''
  if (typeof column.accessor === 'function') return column.accessor(row)
  return row[column.accessor] as string | number | null | undefined
}

function formatDataTableCellValue(value: string | number | null | undefined): ReactNode {
  if (value === null || value === undefined || value === '') {
    return <span className="text-slate-300">—</span>
  }
  return value
}
