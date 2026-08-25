import { useState } from 'react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useEventPageLookups, useEventPageResponses } from '@/hooks/useEventPageResponses'
import { EventPageResponsesFilters } from '@/pages/event-page-responses/EventPageResponsesFilters'
import { EventPageResponsesTable } from '@/pages/event-page-responses/EventPageResponsesTable'
import {
  EVENT_PAGE_RESPONSES_PAGE_SIZE,
  EVENT_PAGE_RESPONSES_PAGE_SIZE_OPTIONS,
} from '@/types/eventPageResponse'

export function EventPageResponsesPage() {
  const [activityName, setActivityName] = useState('')
  const [pageNumber, setPageNumber] = useState(1)
  const [pageSize, setPageSize] = useState(EVENT_PAGE_RESPONSES_PAGE_SIZE)
  const lookupsQuery = useEventPageLookups()
  const { listQuery, exportMutation } = useEventPageResponses({
    activityName: activityName || undefined,
    pageNumber,
    pageSize,
  })

  if (lookupsQuery.isLoading || listQuery.isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  if (lookupsQuery.isError || listQuery.isError) {
    return <Alert variant="destructive">تعذر تحميل ردود التسجيل.</Alert>
  }

  const page = listQuery.data

  return (
    <div className="space-y-4">
      <PageHeader
        title="ردود صفحات التسجيل"
        description="عرض وتصدير استجابات نماذج الدورات"
      />
      {exportMutation.isError && (
        <Alert variant="destructive">تعذر تصدير الملف. يرجى المحاولة مرة أخرى.</Alert>
      )}
      <EventPageResponsesFilters
        activityName={activityName}
        lookups={lookupsQuery.data ?? []}
        onActivityNameChange={(value) => {
          setActivityName(value)
          setPageNumber(1)
        }}
      />
      <EventPageResponsesTable
        items={page?.items ?? []}
        fieldLabels={page?.fieldLabels ?? []}
        activityName={activityName}
        emptyMessage="لا توجد ردود"
        isExporting={exportMutation.isPending}
        onExport={() => exportMutation.mutate()}
        pagination={{
          pageNumber: page?.pageNumber ?? pageNumber,
          pageSize: page?.pageSize ?? pageSize,
          totalCount: page?.totalCount ?? 0,
          totalPages: page?.totalPages ?? 1,
          pageSizeOptions: EVENT_PAGE_RESPONSES_PAGE_SIZE_OPTIONS,
          itemLabel: 'رد',
          onPageChange: setPageNumber,
          onPageSizeChange: (size) => {
            setPageSize(size)
            setPageNumber(1)
          },
        }}
      />
    </div>
  )
}
