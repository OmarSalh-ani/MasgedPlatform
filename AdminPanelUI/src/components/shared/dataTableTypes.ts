import type { ReactNode } from 'react'

export type DataTableViewMode = 'list' | 'card'

export interface DataTableColumn<T> {
  id: string
  header: string
  accessor?: keyof T | ((row: T) => string | number | null | undefined)
  cell?: (row: T) => ReactNode
  className?: string
  headerClassName?: string
}

export interface DataTablePaginationConfig {
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
  pageSizeOptions?: readonly number[]
  itemLabel?: string
  onPageChange: (page: number) => void
  onPageSizeChange: (size: number) => void
}

export interface DataTableProps<T> {
  data: T[]
  columns: DataTableColumn<T>[]
  getRowKey: (row: T, index: number) => string
  emptyMessage?: string
  title?: string
  showExport?: boolean
  isExporting?: boolean
  onExport?: () => void
  toolbar?: ReactNode
  className?: string
  showViewSwitcher?: boolean
  defaultViewMode?: DataTableViewMode
  viewMode?: DataTableViewMode
  onViewModeChange?: (mode: DataTableViewMode) => void
  renderCard?: (row: T) => ReactNode
  pagination?: DataTablePaginationConfig
}
