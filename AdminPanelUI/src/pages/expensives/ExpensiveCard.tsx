import { Building2, Calendar, Eye, Pencil, Receipt, Trash2, User } from 'lucide-react'
import { Link } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import type { ExpensiveListItem } from '@/types/expensives'
import { formatAmount, formatDateGeorgian } from '@/lib/utils'

interface ExpensiveCardProps {
  item: ExpensiveListItem
  onDelete: (id: number) => void
}

export function ExpensiveCard({ item, onDelete }: ExpensiveCardProps) {
  return (
    <article className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-md transition hover:-translate-y-1 hover:shadow-lg">
      <div className="relative bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a] px-5 py-6 text-center text-white">
        <span className="absolute top-4 right-4 rounded-full bg-white/20 px-3 py-1 text-xs font-semibold">
          {item.forGirls ? 'نساء' : 'رجال'}
        </span>
        <div className="mx-auto mb-3 flex size-20 items-center justify-center rounded-full border-4 border-white/30 bg-white/20">
          <Receipt className="size-9" />
        </div>
        <p className="text-2xl font-bold">{formatAmount(item.totalAmount)} دينار كويتي</p>
        <p className="mt-1 text-sm opacity-80">رقم المصروف: {item.id}</p>
      </div>

      <div className="space-y-3 p-5">
        <InfoRow icon={Receipt} label="السبب" value={item.reason} />
        <InfoRow icon={Building2} label="المورد" value={item.supplier} />
        <InfoRow icon={Calendar} label="التاريخ" value={formatDateGeorgian(item.createdAt)} />
        <InfoRow icon={User} label="المسؤول" value={item.createdBy || '—'} />

        <div className="flex flex-wrap gap-2 pt-2">
          <Link
            to={`/expensives/${item.id}`}
            className="inline-flex min-w-[100px] flex-1 items-center justify-center gap-1 rounded-lg bg-[var(--color-primary)] px-3 py-2 text-sm font-semibold text-white hover:opacity-90"
          >
            <Eye className="size-4" />
            عرض
          </Link>
          <Link
            to={`/expensives/${item.id}/edit`}
            className="inline-flex min-w-[100px] flex-1 items-center justify-center gap-1 rounded-lg bg-cyan-600 px-3 py-2 text-sm font-semibold text-white hover:bg-cyan-700"
          >
            <Pencil className="size-4" />
            تعديل
          </Link>
          <Button
            type="button"
            className="min-w-[100px] flex-1 bg-red-600 hover:bg-red-700"
            onClick={() => onDelete(item.id)}
          >
            <Trash2 className="size-4" />
            حذف
          </Button>
        </div>
      </div>
    </article>
  )
}

function InfoRow({
  icon: Icon,
  label,
  value,
}: {
  icon: typeof Receipt
  label: string
  value: string
}) {
  return (
    <div className="flex items-start gap-2 rounded-lg bg-slate-50 p-3 text-sm">
      <Icon className="mt-0.5 size-4 shrink-0 text-[var(--color-primary)]" />
      <span className="shrink-0 font-semibold text-slate-700">{label}:</span>
      <span className="break-words text-slate-600">{value}</span>
    </div>
  )
}
