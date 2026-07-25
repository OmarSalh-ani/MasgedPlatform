import type { LucideIcon } from 'lucide-react'
import type { ReactNode } from 'react'
import { cn } from '@/lib/utils'

interface PageHeaderProps {
  title: ReactNode
  description?: ReactNode
  icon?: LucideIcon
  actions?: ReactNode
  children?: ReactNode
  className?: string
  gradientClassName?: string
  titleClassName?: string
}

export function PageHeader({
  title,
  description,
  icon: Icon,
  actions,
  children,
  className,
  gradientClassName = 'bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a]',
  titleClassName,
}: PageHeaderProps) {
  const hasActions = Boolean(actions)

  return (
    <header
      className={cn(
        'relative mb-6 overflow-hidden rounded-2xl px-6 py-10 text-white shadow-md',
        gradientClassName,
        !hasActions && 'text-center',
        className,
      )}
    >
      <div className="pointer-events-none absolute -top-10 -left-10 size-40 rounded-full bg-white/10" />
      <div className="pointer-events-none absolute -right-8 -bottom-12 size-52 rounded-full bg-white/5" />

      <div
        className={cn(
          'relative',
          hasActions
            ? 'flex flex-wrap items-center justify-between gap-4'
            : 'mx-auto flex max-w-xl flex-col items-center gap-3',
        )}
      >
        <div className={cn(hasActions && 'text-right')}>
          {Icon ? (
            <div
              className={cn(
                'mb-3 flex size-16 items-center justify-center rounded-2xl border border-white/20 bg-white/10 backdrop-blur-sm',
                !hasActions && 'mx-auto',
              )}
            >
              <Icon className="size-8" strokeWidth={1.5} absoluteStrokeWidth />
            </div>
          ) : null}
          <h1 className={cn('text-3xl font-bold', titleClassName)}>{title}</h1>
          {description ? (
            <p
              className={cn(
                'text-sm leading-relaxed text-white/85',
                hasActions ? 'mt-1' : undefined,
              )}
            >
              {description}
            </p>
          ) : null}
          {children}
        </div>
        {actions}
      </div>
    </header>
  )
}
