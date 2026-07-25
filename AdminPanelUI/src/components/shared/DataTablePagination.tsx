import { Button } from '@/components/ui/button'
import type { DataTablePaginationConfig } from '@/components/shared/dataTableTypes'

const DEFAULT_PAGE_SIZE_OPTIONS = [10, 20, 50, 100, 200, 500, 1000] as const

export function DataTablePagination({
  pageNumber,
  pageSize,
  totalCount,
  totalPages,
  pageSizeOptions = DEFAULT_PAGE_SIZE_OPTIONS,
  itemLabel = 'عنصر',
  onPageChange,
  onPageSizeChange,
}: DataTablePaginationConfig) {
  if (totalCount === 0) return null

  const startRecord = (pageNumber - 1) * pageSize + 1
  const endRecord = Math.min(pageNumber * pageSize, totalCount)

  return (
    <div className="flex flex-wrap items-center justify-between gap-4 border-t border-slate-100 bg-slate-50/40 px-5 py-4 sm:px-6">
      <p className="text-sm text-slate-600">
        عرض{' '}
        <span className="font-semibold text-[var(--color-primary)]">
          {startRecord} - {endRecord}
        </span>{' '}
        من أصل{' '}
        <span className="font-semibold text-[var(--color-primary)]">{totalCount}</span> {itemLabel}
      </p>

      <div className="flex items-center gap-2 text-sm text-slate-600">
        <span>عدد العناصر:</span>
        <select
          className="rounded-lg border border-slate-200 bg-white px-2.5 py-1.5 text-sm shadow-sm"
          value={pageSize}
          onChange={(event) => onPageSizeChange(Number(event.target.value))}
        >
          {pageSizeOptions.map((size) => (
            <option key={size} value={size}>
              {size}
            </option>
          ))}
        </select>
      </div>

      <div className="flex items-center gap-2">
        <Button
          type="button"
          variant="outline"
          size="sm"
          disabled={pageNumber <= 1}
          onClick={() => onPageChange(pageNumber - 1)}
        >
          السابق
        </Button>
        <span className="text-sm text-slate-600">
          صفحة {pageNumber} من {totalPages || 1}
        </span>
        <Button
          type="button"
          variant="outline"
          size="sm"
          disabled={pageNumber >= totalPages}
          onClick={() => onPageChange(pageNumber + 1)}
        >
          التالي
        </Button>
      </div>
    </div>
  )
}
