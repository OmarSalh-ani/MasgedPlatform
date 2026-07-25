import { useMemo, useState } from 'react'
import { Input } from '@/components/ui/input'
import type { StudentPlanCircleOption, StudentPlanStudentOption } from '@/types/studentPlan'

interface StudentPlanStudentPickerProps {
  circles: StudentPlanCircleOption[]
  students: StudentPlanStudentOption[]
  selectedIds: number[]
  onChange: (ids: number[]) => void
}

export function StudentPlanStudentPicker({
  circles,
  students,
  selectedIds,
  onChange,
}: StudentPlanStudentPickerProps) {
  const [circleId, setCircleId] = useState<number | ''>('')
  const [search, setSearch] = useState('')

  const filtered = useMemo(() => {
    const q = search.trim().toLowerCase()
    return students.filter((s) => {
      if (circleId !== '' && s.quranCircleId !== circleId) return false
      if (!q) return true
      return s.name.toLowerCase().includes(q)
    })
  }, [students, circleId, search])

  const toggle = (id: number, checked: boolean) => {
    onChange(checked ? [...selectedIds, id] : selectedIds.filter((x) => x !== id))
  }

  const selectAllVisible = () => onChange(filtered.map((s) => s.id))
  const deselectAllVisible = () => {
    const visibleIds = new Set(filtered.map((s) => s.id))
    onChange(selectedIds.filter((id) => !visibleIds.has(id)))
  }

  const selectedStudents = students.filter((s) => selectedIds.includes(s.id))

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap gap-4">
        <div>
          <label className="mb-1 block text-sm font-semibold">الحلقة</label>
          <select
            className="min-w-[200px] rounded-lg border px-3 py-2 text-sm"
            value={circleId}
            onChange={(e) => setCircleId(e.target.value ? Number(e.target.value) : '')}
          >
            <option value="">كل الطلاب</option>
            {circles.map((c) => (
              <option key={c.id} value={c.id}>
                {c.name}
              </option>
            ))}
          </select>
        </div>
        <div className="min-w-[240px] flex-1">
          <label className="mb-1 block text-sm font-semibold">بحث بالاسم</label>
          <Input value={search} onChange={(e) => setSearch(e.target.value)} placeholder="بحث بالاسم..." />
        </div>
      </div>

      <div className="flex gap-2">
        <button type="button" className="rounded border px-3 py-1 text-xs" onClick={selectAllVisible}>
          تحديد الكل
        </button>
        <button type="button" className="rounded border px-3 py-1 text-xs" onClick={deselectAllVisible}>
          إلغاء تحديد الكل
        </button>
      </div>

      <div className="max-h-48 overflow-y-auto rounded-lg border p-2">
        {filtered.map((s) => (
          <label key={s.id} className="flex cursor-pointer items-center gap-2 rounded px-2 py-1 hover:bg-slate-50">
            <input
              type="checkbox"
              checked={selectedIds.includes(s.id)}
              onChange={(e) => toggle(s.id, e.target.checked)}
            />
            <span>{s.name}</span>
          </label>
        ))}
      </div>

      <div>
        <span className="mb-1 block text-sm font-semibold">الطلاب المحددون:</span>
        <div className="flex min-h-8 flex-wrap gap-2 rounded-lg border bg-slate-50 p-2">
          {selectedStudents.length === 0 ? (
            <span className="text-sm text-slate-500">لم يتم اختيار طلاب</span>
          ) : (
            selectedStudents.map((s) => (
              <span
                key={s.id}
                className="inline-flex items-center gap-1 rounded-full bg-[var(--color-primary)] px-3 py-1 text-xs text-white"
              >
                {s.name}
                <button type="button" className="opacity-90" onClick={() => toggle(s.id, false)}>
                  ×
                </button>
              </span>
            ))
          )}
        </div>
      </div>
    </div>
  )
}
