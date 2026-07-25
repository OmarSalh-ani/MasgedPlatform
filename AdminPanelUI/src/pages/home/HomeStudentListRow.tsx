import { ChevronDown, GraduationCap } from 'lucide-react'
import { useState } from 'react'
import { Button } from '@/components/ui/button'
import { resolveImageUrl } from '@/lib/resolveImageUrl'
import {
  HomeStudentActionButtons,
  HomeStudentSelectCheckbox,
  InfoRow,
  type HomeStudentItemProps,
} from '@/pages/home/HomeStudentCardParts'

export function HomeStudentListRow(props: HomeStudentItemProps) {
  const { item, selected, canModify, onToggle, onDelete, onShowTests, onShowReviews } = props
  const [expanded, setExpanded] = useState(false)

  return (
    <article className="overflow-hidden rounded-xl border bg-white shadow-sm transition hover:shadow-md">
      <div className="flex flex-col gap-3 p-4 md:flex-row md:items-center">
        <div className="flex min-w-0 flex-1 items-center gap-3">
          <input
            type="checkbox"
            checked={selected}
            onChange={() =>
              onToggle({
                id: item.id,
                studentName: item.studentName,
                fatherName: item.fatherName,
                fatherPhone: item.fatherPhone,
                circleName: item.circleName,
              })
            }
            className="shrink-0"
          />

          <div className="flex size-12 shrink-0 items-center justify-center overflow-hidden rounded-full border-2 border-[var(--color-primary)]/20 bg-slate-100">
            {item.studentImage ? (
              <img src={resolveImageUrl(item.studentImage)} alt={item.studentName} className="size-full object-cover" />
            ) : (
              <GraduationCap className="size-5 text-[var(--color-primary)]" />
            )}
          </div>

          <div className="min-w-0 flex-1">
            <h3 className="truncate font-bold text-slate-900">{item.studentName}</h3>
            <p className="text-xs text-slate-500">#{item.id}</p>
          </div>
        </div>

        <div className="grid flex-1 grid-cols-2 gap-2 text-sm md:grid-cols-4">
          <ListField label="الحلقة" value={item.circleName || '—'} />
          <ListField label="المستوى" value={item.planLevelName} highlight />
          <ListField label="هاتف ولي الأمر" value={item.fatherPhone} className="hidden sm:block" />
          <ListField label="الاستمارة" value={item.completeFollowup} className="hidden lg:block" />
        </div>

        <Button
          type="button"
          variant="outline"
          size="sm"
          className="shrink-0 md:hidden"
          onClick={() => setExpanded((value) => !value)}
        >
          {expanded ? 'إغلاق' : 'المزيد'}
          <ChevronDown className={`size-4 transition-transform ${expanded ? 'rotate-180' : ''}`} />
        </Button>
      </div>

      {expanded ? (
        <div className="space-y-2 border-t px-4 pt-4 pb-4 md:hidden">
          <div className="grid gap-2">
            <InfoRow label="تاريخ الميلاد" value={item.birthdate ?? '—'} />
            <InfoRow label="العمر" value={`${item.age} سنة`} />
            <InfoRow label="هاتف ولي الأمر" value={item.fatherPhone} />
            <InfoRow label="طالب مميز" value={item.isSpecial} />
            <InfoRow label="طالب نخبة" value={item.isElite} />
            <InfoRow label="تاريخ التسجيل" value={item.createdAt ?? '—'} />
            <InfoRow label="الاستمارة مكتملة" value={item.completeFollowup} />
            {item.womanActivityType ? <InfoRow label="نوع التسجيل" value={item.womanActivityType} /> : null}
          </div>
          <HomeStudentSelectCheckbox item={item} selected={selected} onToggle={onToggle} />
          <HomeStudentActionButtons
            item={item}
            canModify={canModify}
            onDelete={onDelete}
            onShowTests={onShowTests}
            onShowReviews={onShowReviews}
            compact
          />
        </div>
      ) : null}

      <div className="hidden border-t px-4 pt-3 pb-4 md:block">
        <HomeStudentActionButtons
          item={item}
          canModify={canModify}
          onDelete={onDelete}
          onShowTests={onShowTests}
          onShowReviews={onShowReviews}
          compact
        />
      </div>
    </article>
  )
}

function ListField({
  label,
  value,
  highlight = false,
  className = '',
}: {
  label: string
  value: string
  highlight?: boolean
  className?: string
}) {
  return (
    <div className={`min-w-0 rounded-lg bg-slate-50 px-3 py-2 ${className}`}>
      <p className="text-xs font-semibold text-slate-500">{label}</p>
      {highlight ? (
        <span className="mt-0.5 inline-block rounded-full bg-[#CBAC2D] px-2 py-0.5 text-xs font-bold text-white">{value}</span>
      ) : (
        <p className="truncate text-sm text-slate-700">{value}</p>
      )}
    </div>
  )
}
