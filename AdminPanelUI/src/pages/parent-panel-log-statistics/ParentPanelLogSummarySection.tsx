import { BarChart3 } from 'lucide-react'
import { ParentPanelLogStatCard } from '@/pages/parent-panel-log-statistics/ParentPanelLogStatCard'
import {
  getParentPanelSummaryValue,
  parentPanelSummaryCards,
} from '@/pages/parent-panel-log-statistics/parentPanelLogStatisticsConfig'
import { StatisticsSection } from '@/pages/statistics/StatisticsSection'
import type { ParentPanelLogStatisticsSummary } from '@/types/parentPanelLogStatistics'

interface ParentPanelLogSummarySectionProps {
  summary: ParentPanelLogStatisticsSummary
}

export function ParentPanelLogSummarySection({ summary }: ParentPanelLogSummarySectionProps) {
  return (
    <StatisticsSection
      title="الإحصائيات"
      description="ملخص دخول أولياء الأمور إلى لوحة المتابعة خلال الفترة المحددة"
      icon={BarChart3}
    >
      {parentPanelSummaryCards.map((card) => (
        <ParentPanelLogStatCard
          key={card.key}
          config={card}
          value={getParentPanelSummaryValue(summary, card.key)}
        />
      ))}
    </StatisticsSection>
  )
}
