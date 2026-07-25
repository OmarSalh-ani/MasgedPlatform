import { ClipboardList, Percent, UserCheck, UserX } from 'lucide-react'
import type { LucideIcon } from 'lucide-react'
import type { StatTone } from '@/pages/statistics/statisticsConfig'
import type { ParentPanelLogStatisticsSummary } from '@/types/parentPanelLogStatistics'

export interface ParentPanelStatCardConfig {
  key: keyof ParentPanelLogStatisticsSummary
  label: string
  icon: LucideIcon
  tone: StatTone
}

export const parentPanelSummaryCards: ParentPanelStatCardConfig[] = [
  {
    key: 'parentsOpened',
    label: 'عدد أولياء الأمور الذين فتحوا اللوحة',
    icon: UserCheck,
    tone: 'emerald',
  },
  {
    key: 'parentsNotOpened',
    label: 'عدد أولياء الأمور الذين لم يفتحوا اللوحة',
    icon: UserX,
    tone: 'rose',
  },
  {
    key: 'totalLogEntries',
    label: 'إجمالي سجلات الدخول',
    icon: ClipboardList,
    tone: 'sky',
  },
  {
    key: 'percentage',
    label: 'نسبة أولياء الأمور الذين فتحوا اللوحة',
    icon: Percent,
    tone: 'amber',
  },
]

export function getParentPanelSummaryValue(
  summary: ParentPanelLogStatisticsSummary,
  key: keyof ParentPanelLogStatisticsSummary,
): string {
  const value = summary[key]
  if (key === 'percentage') {
    return summary.percentage
  }
  return Number(value).toLocaleString('ar-EG')
}
