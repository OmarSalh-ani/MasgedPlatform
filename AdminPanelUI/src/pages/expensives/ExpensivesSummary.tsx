import { Calendar, ChartLine, Coins, List } from 'lucide-react'
import type { ExpensiveSummary } from '@/types/expensives'
import { formatAmount, formatCount } from '@/lib/utils'

interface ExpensivesSummaryProps {
  summary: ExpensiveSummary
}

export function ExpensivesSummary({ summary }: ExpensivesSummaryProps) {
  return (
    <div className="mb-6 grid gap-5 sm:grid-cols-2 xl:grid-cols-4">
      <SummaryCard
        icon={List}
        label="إجمالي المصروفات"
        value={formatCount(summary.totalCount)}
        subtitle="مصروف"
        hint="جميع المصروفات المسجلة"
        tone="primary"
      />
      <SummaryCard
        icon={Coins}
        label="إجمالي المبلغ"
        value={formatAmount(summary.totalAmount)}
        subtitle="دينار كويتي"
        hint="مجموع جميع المصروفات"
        tone="primary"
      />
      <SummaryCard
        icon={Calendar}
        label="مصروفات هذا الشهر"
        value={formatCount(summary.thisMonthCount)}
        subtitle="مصروف"
        hint={`المبلغ: ${formatAmount(summary.thisMonthAmount)} دينار كويتي`}
        tone="success"
      />
      <SummaryCard
        icon={ChartLine}
        label="متوسط المصروف"
        value={formatAmount(summary.averageAmount)}
        subtitle="دينار كويتي"
        hint="متوسط قيمة المصروف الواحد"
        tone="secondary"
      />
    </div>
  )
}

function SummaryCard({
  icon: Icon,
  label,
  value,
  subtitle,
  hint,
  tone,
}: {
  icon: typeof List
  label: string
  value: string
  subtitle: string
  hint: string
  tone: 'primary' | 'success' | 'secondary'
}) {
  const iconClass =
    tone === 'success'
      ? 'bg-gradient-to-br from-emerald-600 to-teal-500'
      : tone === 'secondary'
        ? 'bg-gradient-to-br from-[#CBAC2D] to-[#d4a574]'
        : 'bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a]'

  const borderClass =
    tone === 'success'
      ? 'border-t-emerald-600'
      : tone === 'secondary'
        ? 'border-t-[#CBAC2D]'
        : 'border-t-[var(--color-primary)]'

  return (
    <article className={`rounded-xl border-t-4 bg-white p-5 shadow-md ${borderClass}`}>
      <div className={`mb-4 inline-flex size-12 items-center justify-center rounded-xl text-white ${iconClass}`}>
        <Icon className="size-5" />
      </div>
      <p className="text-sm font-semibold text-slate-600">{label}</p>
      <p className="mt-2 text-3xl font-bold text-slate-900">
        {value} <span className="text-base font-medium text-slate-500">{subtitle}</span>
      </p>
      <p className="mt-2 text-sm text-slate-500">{hint}</p>
    </article>
  )
}
