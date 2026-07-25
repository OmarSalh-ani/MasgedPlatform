import type { ReactNode } from 'react'
import type { LucideIcon } from 'lucide-react'
import { cn } from '@/lib/utils'
import { StatisticsIcon } from '@/pages/statistics/StatisticsIcon'

interface StatisticsSectionProps {
  title: string
  description: string
  icon: LucideIcon
  columns?: 3 | 4
  children: ReactNode
}

const columnClass = {
  3: 'grid-cols-2 xl:grid-cols-3',
  4: 'grid-cols-2 xl:grid-cols-4',
} as const

export function StatisticsSection({
  title,
  description,
  icon,
  columns = 4,
  children,
}: StatisticsSectionProps) {
  return (
    <section className="space-y-4">
      <div className="flex items-start gap-3">
        <div className="flex size-10 shrink-0 items-center justify-center rounded-xl bg-[var(--color-primary-muted)] ring-1 ring-blue-100">
          <StatisticsIcon icon={icon} className="text-[var(--color-primary)]" />
        </div>
        <div>
          <h2 className="text-lg font-bold text-slate-800">{title}</h2>
          <p className="mt-0.5 text-sm text-slate-500">{description}</p>
        </div>
      </div>
      <div className={cn('grid gap-4', columnClass[columns])}>{children}</div>
    </section>
  )
}
