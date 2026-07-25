import { useEffect, useState } from 'react'
import { Users } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useParentPanelLogStatistics } from '@/hooks/useParentPanelLogStatistics'
import { ParentPanelLogStatisticsFilters } from '@/pages/parent-panel-log-statistics/ParentPanelLogStatisticsFilters'
import { ParentPanelLogStatisticsTable } from '@/pages/parent-panel-log-statistics/ParentPanelLogStatisticsTable'
import { ParentPanelLogSummarySection } from '@/pages/parent-panel-log-statistics/ParentPanelLogSummarySection'
import {
  getDefaultDateRange,
  type ParentPanelLogStatisticsFilters as Filters,
} from '@/types/parentPanelLogStatistics'

export function ParentPanelLogStatisticsPage() {
  const defaults = getDefaultDateRange()
  const [fromDate, setFromDate] = useState(defaults.fromDate)
  const [toDate, setToDate] = useState(defaults.toDate)
  const [appliedFilters, setAppliedFilters] = useState<Filters | null>(defaults)

  const statisticsQuery = useParentPanelLogStatistics(appliedFilters)

  useEffect(() => {
    setAppliedFilters(defaults)
  }, [])

  const report = statisticsQuery.data
  const summary = report?.summary

  const applyFilters = () => {
    setAppliedFilters({ fromDate, toDate })
  }

  return (
    <div className="mx-auto max-w-7xl space-y-8">
      <PageHeader
        icon={Users}
        title="إحصائيات دخول أولياء الأمور"
        description="تتبع ومراقبة دخول أولياء الأمور إلى لوحة المتابعة"
        gradientClassName="bg-gradient-to-br from-[#7C8738] to-[#1a5f8a]"
        className="mb-0"
      />

      <ParentPanelLogStatisticsFilters
        fromDate={fromDate}
        toDate={toDate}
        isLoading={statisticsQuery.isFetching}
        onFromDateChange={setFromDate}
        onToDateChange={setToDate}
        onApply={applyFilters}
      />

      {statisticsQuery.isLoading && <ParentPanelLogStatisticsPageSkeleton />}

      {statisticsQuery.isError && (
        <Alert variant="destructive">تعذر تحميل الإحصائيات. يرجى المحاولة مرة أخرى.</Alert>
      )}

      {summary && (
        <div className="space-y-10">
          <ParentPanelLogSummarySection summary={summary} />
          <ParentPanelLogStatisticsTable entries={report?.entries ?? []} />
        </div>
      )}
    </div>
  )
}

function ParentPanelLogStatisticsPageSkeleton() {
  return (
    <div className="space-y-10">
      <div className="space-y-4">
        <Skeleton className="h-12 w-64" />
        <div className="grid grid-cols-2 gap-4 xl:grid-cols-4">
          {Array.from({ length: 4 }).map((_, index) => (
            <Skeleton key={index} className="h-36 rounded-2xl" />
          ))}
        </div>
      </div>
      <div className="space-y-4">
        <Skeleton className="h-12 w-56" />
        <Skeleton className="h-64 rounded-2xl" />
      </div>
    </div>
  )
}
