import { Button } from '@/components/ui/button'

interface SendNotesTablePaginationProps {
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
  onPageChange: (page: number) => void
}

export function SendNotesTablePagination({
  pageNumber,
  pageSize,
  totalCount,
  totalPages,
  onPageChange,
}: SendNotesTablePaginationProps) {
  const startRecord = (pageNumber - 1) * pageSize + 1
  const endRecord = Math.min(pageNumber * pageSize, totalCount)

  return (
    <div className="flex flex-wrap items-center justify-between gap-4 border-t border-slate-100 bg-slate-50/40 px-5 py-4">
      <p className="text-sm text-slate-600">
        عرض{' '}
        <span className="font-semibold text-[var(--color-primary)]">
          {startRecord} - {endRecord}
        </span>{' '}
        من أصل{' '}
        <span className="font-semibold text-[var(--color-primary)]">{totalCount}</span> ملاحظة
      </p>

      {totalPages > 1 && (
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
            صفحة {pageNumber} من {totalPages}
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
      )}
    </div>
  )
}
