import { Eye, Hash, Pencil, Tag, Trash2 } from 'lucide-react'
import { Button } from '@/components/ui/button'
import type { WomanActivityListItem } from '@/types/womansActivity'

interface WomanActivityCardProps {
  item: WomanActivityListItem
  canModify: boolean
  feminineTheme: boolean
  onEdit: (item: WomanActivityListItem) => void
  onDelete: (id: number) => void
}

export function WomanActivityCard({
  item,
  canModify,
  feminineTheme,
  onEdit,
  onDelete,
}: WomanActivityCardProps) {
  const headerClass = feminineTheme
    ? 'bg-gradient-to-br from-pink-600 to-pink-800'
    : 'bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a]'
  const editBtnClass = feminineTheme
    ? 'bg-gradient-to-br from-pink-400 to-pink-600 hover:opacity-90'
    : 'bg-gradient-to-br from-cyan-600 to-cyan-700 hover:opacity-90'

  const statusLabel = item.isVisible ? 'مرئي' : 'مخفي'

  return (
    <article className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-md transition hover:-translate-y-1 hover:shadow-lg">
      <div className={`relative px-5 py-6 text-center text-white ${headerClass}`}>
        <span className="absolute top-4 right-4 rounded-full bg-white/20 px-3 py-1 text-xs font-semibold">
          {statusLabel}
        </span>
        <div className="mx-auto mb-3 flex size-20 items-center justify-center rounded-full border-4 border-white/30 bg-white/20">
          <Hash className="size-9" />
        </div>
        <h2 className="text-lg font-bold break-words">{item.name}</h2>
        <p className="mt-1 text-sm opacity-80">رقم النشاط: {item.id}</p>
      </div>

      <div className="space-y-3 p-5">
        <div className="flex items-center gap-2 rounded-lg bg-slate-50 p-3 text-sm">
          <Tag className="size-4 shrink-0 text-[var(--color-primary)]" />
          <span className="font-semibold text-slate-700">النشاط:</span>
          <span className="text-slate-600">{item.name}</span>
        </div>
        <div className="flex items-center gap-2 rounded-lg bg-slate-50 p-3 text-sm">
          <Eye className="size-4 shrink-0 text-[var(--color-primary)]" />
          <span className="font-semibold text-slate-700">الحالة:</span>
          <span className="text-slate-600">{statusLabel}</span>
        </div>

        {canModify && (
          <div className="flex flex-wrap gap-2 pt-2">
            <Button
              type="button"
              className={`min-w-[120px] flex-1 ${editBtnClass}`}
              onClick={() => onEdit(item)}
            >
              <Pencil className="size-4" />
              تعديل
            </Button>
            <Button
              type="button"
              className="min-w-[120px] flex-1 bg-red-600 hover:bg-red-700"
              onClick={() => onDelete(item.id)}
            >
              <Trash2 className="size-4" />
              حذف
            </Button>
          </div>
        )}
      </div>
    </article>
  )
}
