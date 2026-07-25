import { Crown, Key, List, Mail, Pencil, Phone, Printer, Trash2, Users } from 'lucide-react'
import { Link } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { resolveImageUrl } from '@/lib/resolveImageUrl'
import type { TeacherListItem } from '@/types/teacher'

interface TeacherCardProps {
  item: TeacherListItem
  canModify: boolean
  onDelete: (id: number, name: string) => void
}

export function TeacherCard({ item, canModify, onDelete }: TeacherCardProps) {
  return (
    <article className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-md transition hover:-translate-y-1 hover:shadow-lg">
      <div className="bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a] px-5 py-6 text-center text-white">
        {item.imageUrl ? (
          <img
            src={resolveImageUrl(item.imageUrl)}
            alt={item.name}
            className="mx-auto mb-3 size-20 rounded-full border-4 border-white/30 object-cover"
          />
        ) : (
          <div className="mx-auto mb-3 flex size-20 items-center justify-center rounded-full border-4 border-white/30 bg-white/20">
            <Users className="size-9" />
          </div>
        )}
        <h2 className="teacher-name text-lg font-bold break-words">{item.name}</h2>
        <p className="teacher-id mt-1 text-sm opacity-80">رقم المعلم: {item.id}</p>
      </div>

      <div className="space-y-3 p-5">
        <div className="flex items-center gap-2 rounded-lg bg-slate-50 p-3 text-sm">
          <Phone className="size-4 shrink-0 text-[var(--color-primary)]" />
          <span className="font-semibold text-slate-700">الموبايل:</span>
          <span className="teacher-mobile text-slate-600">{item.mobile ?? '—'}</span>
        </div>
        <div className="flex items-center gap-2 rounded-lg bg-slate-50 p-3 text-sm">
          <Mail className="size-4 shrink-0 text-[var(--color-primary)]" />
          <span className="font-semibold text-slate-700">البريد الإلكتروني:</span>
          <span className="teacher-email break-all text-slate-600">{item.email}</span>
        </div>
        <div className="flex items-center gap-2 rounded-lg bg-slate-50 p-3 text-sm">
          <Key className="size-4 shrink-0 text-[var(--color-primary)]" />
          <span className="font-semibold text-slate-700">كلمة المرور:</span>
          <span className="text-slate-600">{item.password}</span>
        </div>
        <div className="flex items-center gap-2 rounded-lg bg-slate-50 p-3 text-sm">
          <Users className="size-4 shrink-0 text-[var(--color-primary)]" />
          <span className="font-semibold text-slate-700">الحلقات:</span>
          <span className="text-slate-600">{item.circleCount} حلقة</span>
        </div>
        {item.usersManage && (
          <div className="flex items-center gap-2 rounded-lg bg-slate-50 p-3 text-sm">
            <Crown className="size-4 shrink-0 text-[var(--color-primary)]" />
            <span className="font-semibold text-slate-700">الصلاحية:</span>
            <span className="rounded-full bg-emerald-600 px-3 py-0.5 text-xs font-semibold text-white">
              أدمن عام
            </span>
          </div>
        )}

        <div className="flex flex-wrap gap-2 pt-2">
          {canModify && (
            <Link
              to={`/teachers/${item.id}/edit`}
              className="inline-flex min-w-[120px] flex-1 items-center justify-center gap-1 rounded-lg bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a] px-3 py-2 text-sm font-semibold text-white hover:opacity-90"
            >
              <Pencil className="size-4" />
              تعديل
            </Link>
          )}
          <Link
            to={`/circles?teacher=${item.id}`}
            className="inline-flex min-w-[120px] flex-1 items-center justify-center gap-1 rounded-lg bg-amber-600 px-3 py-2 text-sm font-semibold text-white hover:bg-amber-700"
          >
            <List className="size-4" />
            الحلقات
          </Link>
          <Link
            to={`/teachers/${item.id}/card-print`}
            target="_blank"
            rel="noreferrer"
            className="inline-flex min-w-[120px] flex-1 items-center justify-center gap-1 rounded-lg bg-amber-600 px-3 py-2 text-sm font-semibold text-white hover:bg-amber-700"
          >
            <Printer className="size-4" />
            طباعة الكرت
          </Link>
          {canModify && (
            <Button
              type="button"
              className="min-w-[120px] flex-1 bg-red-600 hover:bg-red-700"
              onClick={() => onDelete(item.id, item.name)}
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
