import { cn } from '@/lib/utils'

interface AlertProps extends React.HTMLAttributes<HTMLDivElement> {
  variant?: 'default' | 'destructive'
}

export function Alert({ className, variant = 'default', ...props }: AlertProps) {
  return (
    <div
      role="alert"
      className={cn(
        'rounded-lg border p-4 text-sm',
        variant === 'destructive' && 'border-red-200 bg-red-50 text-red-800',
        variant === 'default' && 'border-slate-200 bg-slate-50',
        className
      )}
      {...props}
    />
  )
}
