import { ChevronLeft } from 'lucide-react'
import { Link } from 'react-router-dom'
import { cn } from '@/lib/utils'
import { StatisticsIcon } from '@/pages/statistics/StatisticsIcon'
import { toneStyles, type StatCardConfig } from '@/pages/statistics/statisticsConfig'

interface StatisticsCardProps {
  config: StatCardConfig
  value: number
}

export function StatisticsCard({ config, value }: StatisticsCardProps) {
  const tone = toneStyles[config.tone]

  return (
    <Link
      to={config.to}
      className={cn(
        'group flex flex-col rounded-2xl border border-slate-200 bg-white p-4 shadow-sm transition-all duration-200 sm:p-5',
        'hover:-translate-y-0.5 hover:shadow-md',
        'border-r-4',
        tone.accent,
        tone.hover,
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
        <ChevronLeft
          className="hidden size-4 shrink-0 text-slate-300 transition group-hover:text-slate-500 sm:block"
          strokeWidth={1.5}
          absoluteStrokeWidth
        />
      </div>

      <p className={cn('text-2xl font-bold tabular-nums sm:text-3xl', tone.value)}>
        {value.toLocaleString('ar-EG')}
      </p>
      <p className="mt-1 text-xs font-medium text-slate-600 sm:text-sm">{config.label}</p>
    </Link>
  )
}
