import { useEffect, useState } from 'react'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { DialogActions, DialogShell } from '@/pages/home/dialogs/HomeWhatsappDialog'
import type { HomeLookup } from '@/types/home'

interface CreateCircleDialogProps {
  open: boolean
  selectedCount: number
  teachers: HomeLookup[]
  isPending: boolean
  canModify: boolean
  onOpenChange: (open: boolean) => void
  onSubmit: (payload: { circleName: string; teacherId: number }) => void
}

export function CreateCircleDialog({
  open,
  selectedCount,
  teachers,
  isPending,
  canModify,
  onOpenChange,
  onSubmit,
}: CreateCircleDialogProps) {
  const [circleName, setCircleName] = useState('')
  const [teacherId, setTeacherId] = useState('')

  useEffect(() => {
    if (!open) {
      setCircleName('')
      setTeacherId('')
    }
  }, [open])

  if (!open) return null

  return (
    <DialogShell title="إنشاء حلقة جديدة" onClose={() => onOpenChange(false)}>
      <p className="mb-4 text-sm text-slate-600">عدد الطلاب المحددين: {selectedCount}</p>
      <div className="mb-3 space-y-1">
        <Label htmlFor="circleName">اسم الحلقة</Label>
        <Input id="circleName" value={circleName} placeholder="أدخل اسم الحلقة" onChange={(e) => setCircleName(e.target.value)} />
      </div>
      <div className="mb-4 space-y-1">
        <Label htmlFor="circleTeacher">اسم المعلم</Label>
        <select id="circleTeacher" className="h-10 w-full rounded-md border px-3 text-sm" value={teacherId} onChange={(e) => setTeacherId(e.target.value)}>
          <option value="">اختر المعلم</option>
          {teachers.map((teacher) => (
            <option key={teacher.id} value={teacher.id}>{teacher.name}</option>
          ))}
        </select>
      </div>
      <DialogActions
        isPending={isPending}
        canSubmit={canModify && circleName.trim() !== '' && teacherId !== ''}
        onCancel={() => onOpenChange(false)}
        onSubmit={() => onSubmit({ circleName: circleName.trim(), teacherId: Number(teacherId) })}
        submitLabel={isPending ? 'جاري الإنشاء...' : 'إنشاء الحلقة'}
      />
    </DialogShell>
  )
}
