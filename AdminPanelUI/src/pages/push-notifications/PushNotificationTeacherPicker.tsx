import { Button } from '@/components/ui/button'
import type { PushNotificationTeacherOption } from '@/types/pushNotification'

interface PushNotificationTeacherPickerProps {
  teachers: PushNotificationTeacherOption[]
  selectedIds: number[]
  onChange: (ids: number[]) => void
}

export function PushNotificationTeacherPicker({
  teachers,
  selectedIds,
  onChange,
}: PushNotificationTeacherPickerProps) {
  const toggleTeacher = (teacherId: number, checked: boolean) => {
    onChange(checked ? [...selectedIds, teacherId] : selectedIds.filter((id) => id !== teacherId))
  }

  const selectAll = (checked: boolean) => {
    onChange(checked ? teachers.map((t) => t.id) : [])
  }

  if (teachers.length === 0) {
    return <p className="text-sm text-slate-600">لا يوجد معلمون متاحون</p>
  }

  return (
    <div>
      <div className="max-h-[300px] overflow-y-auto rounded-lg border bg-slate-50 p-3">
        {teachers.map((teacher) => (
          <label key={teacher.id} className="mb-2 flex cursor-pointer items-center gap-2">
            <input
              type="checkbox"
              checked={selectedIds.includes(teacher.id)}
              onChange={(e) => toggleTeacher(teacher.id, e.target.checked)}
            />
            <span>{teacher.name}</span>
          </label>
        ))}
      </div>
      <div className="mt-2 flex gap-2">
        <Button type="button" variant="outline" className="px-3 py-1 text-sm" onClick={() => selectAll(true)}>
          تحديد الكل
        </Button>
        <Button type="button" variant="outline" className="px-3 py-1 text-sm" onClick={() => selectAll(false)}>
          إلغاء التحديد
        </Button>
      </div>
      <p className="mt-2 text-sm text-slate-600">المحدد: {selectedIds.length} معلم</p>
    </div>
  )
}
