import { Calendar, CircleDot, Pencil, Trash2, User, Users } from 'lucide-react'
import { Link } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import type { CircleListItem } from '@/types/circle'
import { formatCircleCreatedAt } from '@/types/circle'

interface CircleCardProps {
  item: CircleListItem
  canModify: boolean
  selected?: boolean
  showSelection?: boolean
  onSelectChange?: (checked: boolean) => void
  onDelete: (id: number) => void
}

export function CircleCard({
  item,
  canModify,
  selected = false,
  showSelection = false,
  onSelectChange,
  onDelete,
}: CircleCardProps) {
  return (
    <article className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-md transition hover:-translate-y-1 hover:shadow-lg">
      <div className="relative bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a] px-5 py-6 text-center text-white">
        {showSelection && (
          <input
            type="checkbox"
            className="absolute top-4 left-4 size-4 accent-white"
            checked={selected}
            onChange={(e) => onSelectChange?.(e.target.checked)}
            aria-label={`تحديد ${item.name}`}
          />
        )}
        <span className="absolute top-4 right-4 rounded-full bg-white/20 px-3 py-1 text-xs font-semibold">
          {item.forGirls ? 'نساء' : 'رجال'}
        </span>
        <div className="mx-auto mb-3 flex size-20 items-center justify-center rounded-full border-4 border-white/30 bg-white/20">
          <CircleDot className="size-9" />
        </div>
        <h2 className="text-lg font-bold break-words">{item.name}</h2>
        <p className="mt-1 text-sm opacity-80">رقم الحلقة: {item.id}</p>
      </div>

      <div className="space-y-3 p-5">
        <div className="flex items-center gap-2 rounded-lg bg-slate-50 p-3 text-sm">
          <User className="size-4 shrink-0 text-[var(--color-primary)]" />
          <span className="font-semibold text-slate-700">المعلم:</span>
          <span className="text-slate-600">{item.teacherName}</span>
        </div>
        <div className="flex items-center gap-2 rounded-lg bg-slate-50 p-3 text-sm">
          <Users className="size-4 shrink-0 text-[var(--color-primary)]" />
          <span className="font-semibold text-slate-700">الطلاب:</span>
          <span className="text-slate-600">{item.studentsCount} طالب</span>
        </div>
        <div className="flex items-center gap-2 rounded-lg bg-slate-50 p-3 text-sm">
          <Calendar className="size-4 shrink-0 text-[var(--color-primary)]" />
          <span className="font-semibold text-slate-700">تاريخ الإنشاء:</span>
          <span className="text-slate-600">{formatCircleCreatedAt(item.createdAt)}</span>
        </div>
        <div className="flex items-center gap-2 rounded-lg bg-slate-50 p-3 text-sm">
          <User className="size-4 shrink-0 text-[var(--color-primary)]" />
          <span className="font-semibold text-slate-700">المنشئ:</span>
          <span className="text-slate-600">{item.createdBy}</span>
        </div>

        <div className="flex flex-wrap gap-2 pt-2">
          {canModify && (
            <Link
              to={`/circles/${item.id}/edit`}
              className="inline-flex min-w-[120px] flex-1 items-center justify-center gap-1 rounded-lg bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a] px-3 py-2 text-sm font-semibold text-white hover:opacity-90"
            >
              <Pencil className="size-4" />
              تعديل
            </Link>
          )}
          <a
            href={`/home?circle=${item.id}`}
            target="_blank"
            rel="noreferrer"
            className="inline-flex min-w-[120px] flex-1 items-center justify-center gap-1 rounded-lg bg-emerald-600 px-3 py-2 text-sm font-semibold text-white hover:bg-emerald-700"
          >
            <Users className="size-4" />
            الطلاب
          </a>
          {canModify && (
            <Button
              type="button"
              className="min-w-[120px] flex-1 bg-red-600 hover:bg-red-700"
              onClick={() => onDelete(item.id)}
            >
              <Trash2 className="size-4" />
              حذف
            </Button>
          )}
        </div>
      </div>
    </article>
  )
}
