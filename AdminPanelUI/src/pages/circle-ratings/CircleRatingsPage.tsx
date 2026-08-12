import { useState } from 'react'
import { Link } from 'react-router-dom'
import { ClipboardCheck, ClipboardPlus } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useCircleVisitRatings } from '@/hooks/useCircleVisitRatings'
import { CircleRatingsTable } from '@/pages/circle-ratings/CircleRatingsTable'

export function CircleRatingsPage() {
  const [pageNumber, setPageNumber] = useState(1)
  const { query, exportMutation } = useCircleVisitRatings(pageNumber)
  const result = query.data
  const items = result?.items ?? []

  return (
    <div className="mx-auto max-w-7xl space-y-8">
      <PageHeader
        icon={ClipboardCheck}
        title="التقييمات"
        description="سجل تقييمات زيارات حلقات القرآن الكريم"
        actions={
          <Link
            to="/circle-ratings/new"
            className="inline-flex items-center gap-2 rounded-full bg-white/20 px-5 py-2.5 text-sm font-semibold text-white transition hover:bg-white/30"
          >
            <ClipboardPlus className="size-4" strokeWidth={1.5} absoluteStrokeWidth />
            تقييم جديد
          </Link>
        }
      />

      {query.isLoading && <Skeleton className="mt-8 h-64 w-full rounded-2xl" />}

      {query.isError && (
        <Alert variant="destructive" className="mt-8">
          تعذر تحميل التقييمات. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      {result && (
        <div className="mt-8">
          <CircleRatingsTable
            items={items}
            pageNumber={pageNumber}
            pageSize={result.pageSize}
            totalCount={result.totalCount}
            totalPages={result.totalPages}
            exportingId={exportMutation.isPending ? (exportMutation.variables ?? null) : null}
            onPageChange={setPageNumber}
            onExportPdf={(id) => exportMutation.mutate(id)}
          />
        </div>
      )}
    </div>
  )
}
