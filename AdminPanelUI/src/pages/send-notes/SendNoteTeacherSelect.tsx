import { Button } from '@/components/ui/button'
import type { TeacherOption } from '@/types/sendNote'

interface SendNoteTeacherSelectProps {
  teachers: TeacherOption[]
  selectedTeacherIds: number[]
  onToggle: (teacherId: number, checked: boolean) => void
  onSelectAll: (checked: boolean) => void
}

export function SendNoteTeacherSelect({
  teachers,
  selectedTeacherIds,
  onToggle,
  onSelectAll,
}: SendNoteTeacherSelectProps) {
  const allSelected = teachers.length > 0 && selectedTeacherIds.length === teachers.length

  return (
    <div className="space-y-3">
      <div className="max-h-[300px] overflow-y-auto rounded-xl border border-slate-200 bg-slate-50/80 p-4">
        {teachers.map((teacher) => (
          <label
            key={teacher.id}
            className="mb-2 flex cursor-pointer items-center gap-2 rounded-lg px-2 py-1.5 transition hover:bg-white"
          >
            <input
              type="checkbox"
              className="size-4 accent-[var(--color-primary)]"
              checked={selectedTeacherIds.includes(teacher.id)}
              onChange={(e) => onToggle(teacher.id, e.target.checked)}
            />
            <span className="text-sm text-slate-800">{teacher.name}</span>
          </label>
        ))}
      </div>
      <div className="flex flex-wrap gap-2">
        <Button type="button" variant="outline" size="sm" onClick={() => onSelectAll(true)}>
          تحديد الكل
        </Button>
        <Button type="button" variant="outline" size="sm" onClick={() => onSelectAll(false)}>
          إلغاء التحديد
        </Button>
        {allSelected && (
          <span className="self-center text-xs text-slate-500">تم تحديد جميع المعلمين</span>
        )}
      </div>
    </div>
  )
}
