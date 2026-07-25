import type { LucideIcon } from 'lucide-react'
import { cn } from '@/lib/utils'

const ICON_STROKE = 1.5

const sizeMap = {
  sm: 'size-4',
  md: 'size-5',
  lg: 'size-6',
  xl: 'size-8',
} as const

interface StatisticsIconProps {
  icon: LucideIcon
  className?: string
  size?: keyof typeof sizeMap
}

export function StatisticsIcon({ icon: Icon, className, size = 'md' }: StatisticsIconProps) {
  return (
    <Icon
      className={cn(sizeMap[size], className)}
      strokeWidth={ICON_STROKE}
      absoluteStrokeWidth
    />
  )
}
