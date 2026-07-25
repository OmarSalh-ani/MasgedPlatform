import { cn } from '@/lib/utils'
import { StatisticsIcon } from '@/pages/statistics/StatisticsIcon'
import { toneStyles } from '@/pages/statistics/statisticsConfig'
import type { ParentPanelStatCardConfig } from '@/pages/parent-panel-log-statistics/parentPanelLogStatisticsConfig'

interface ParentPanelLogStatCardProps {
  config: ParentPanelStatCardConfig
  value: string
}

export function ParentPanelLogStatCard({ config, value }: ParentPanelLogStatCardProps) {
  const tone = toneStyles[config.tone]

  return (
    <div
      className={cn(
        'flex flex-col rounded-2xl border border-slate-200 bg-white p-4 shadow-sm sm:p-5',
        'border-r-4',
        tone.accent,
      )}
    >
      <div className="mb-3 flex items-start justify-between gap-2 sm:mb-4 sm:gap-3">
        <div
          className={cn(
            'flex size-10 items-center justify-center rounded-xl ring-1 ring-inset sm:size-12',
            tone.iconWrap,
          )}
        >
          <StatisticsIcon icon={config.icon} size="lg" className={tone.icon} />
        </div>
      </div>

      <p className={cn('text-2xl font-bold tabular-nums sm:text-3xl', tone.value)}>{value}</p>
      <p className="mt-1 text-xs font-medium text-slate-600 sm:text-sm">{config.label}</p>
    </div>
  )
}
