import { BarChart3, LayoutGrid } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useStatistics } from '@/hooks/useStatistics'
import { StatisticsCard } from '@/pages/statistics/StatisticsCard'
import { StatisticsSection } from '@/pages/statistics/StatisticsSection'
import {
  additionalStatCards,
  circleStatCards,
  getAdditionalStatValue,
  getCircleStatValue,
} from '@/pages/statistics/statisticsConfig'

export function StatisticsPage() {
  const statisticsQuery = useStatistics()
  const circleStats = statisticsQuery.data?.circleStatistics
  const additionalStats = statisticsQuery.data?.additionalStatistics

  return (
    <div className="mx-auto max-w-7xl space-y-8">
      <PageHeader
        icon={BarChart3}
        title="إحصائيات النظام"
        description="نظرة شاملة على حضور الطلاب وبيانات النظام الأساسية"
        className="mb-0"
      />

      {statisticsQuery.isLoading && <StatisticsPageSkeleton />}

      {statisticsQuery.isError && (
        <Alert variant="destructive">تعذر تحميل الإحصائيات. يرجى المحاولة مرة أخرى.</Alert>
      )}

      {circleStats && additionalStats && (
        <div className="space-y-10">
          <StatisticsSection
            title="احصائيات الحلقات"
            description="متابعة الحضور والغياب وإجمالي الطلاب لليوم"
            icon={BarChart3}
          >
            {circleStatCards.map((card) => (
              <StatisticsCard
                key={card.key}
                config={card}
                value={getCircleStatValue(circleStats, card.key)}
              />
            ))}
          </StatisticsSection>

          <StatisticsSection
            title="إحصائيات إضافية"
            description="أعداد المعلمين والحلقات والطلاب المميزين"
            icon={LayoutGrid}
            columns={3}
          >
            {additionalStatCards.map((card) => (
              <StatisticsCard
                key={card.key}
                config={card}
                value={getAdditionalStatValue(additionalStats, card.key)}
              />
            ))}
          </StatisticsSection>
        </div>
      )}
    </div>
  )
}

function StatisticsPageSkeleton() {
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
        <div className="grid grid-cols-2 gap-4 xl:grid-cols-3">
          {Array.from({ length: 3 }).map((_, index) => (
            <Skeleton key={index} className="h-36 rounded-2xl" />
          ))}
        </div>
      </div>
    </div>
  )
}
