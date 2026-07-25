import { useEffect, useState } from 'react'
import { Label } from '@/components/ui/label'
import { DialogActions, DialogShell } from '@/pages/home/dialogs/HomeWhatsappDialog'
import type { HomeLookup, SelectedHomeStudent } from '@/types/home'

interface TransferStudentsDialogProps {
  open: boolean
  selectedStudents: SelectedHomeStudent[]
  circles: HomeLookup[]
  isPending: boolean
  canModify: boolean
  onOpenChange: (open: boolean) => void
  onSubmit: (circleId: number) => void
}

export function TransferStudentsDialog({
  open,
  selectedStudents,
  circles,
  isPending,
  canModify,
  onOpenChange,
  onSubmit,
}: TransferStudentsDialogProps) {
  const [circleId, setCircleId] = useState('')

  useEffect(() => {
    if (!open) setCircleId('')
  }, [open])

  if (!open) return null

  return (
    <DialogShell title="نقل الطلاب إلى حلقة جديدة" onClose={() => onOpenChange(false)}>
      <div className="mb-4 rounded-lg border bg-slate-50 p-3">
        <p className="mb-2 font-semibold">الطلاب المحددين: {selectedStudents.length}</p>
        <div className="max-h-40 overflow-y-auto text-sm">
          {selectedStudents.map((student) => (
            <div key={student.id} className="border-b py-1 last:border-b-0">{student.studentName}</div>
          ))}
        </div>
      </div>
      <div className="mb-4 space-y-1">
        <Label htmlFor="transferCircle">اختر الحلقة الجديدة</Label>
        <select id="transferCircle" className="h-10 w-full rounded-md border px-3 text-sm" value={circleId} onChange={(e) => setCircleId(e.target.value)}>
          <option value="">-- اختر الحلقة --</option>
          {circles.map((circle) => (
            <option key={circle.id} value={circle.id}>{circle.name}</option>
          ))}
        </select>
      </div>
      <p className="mb-4 rounded-lg border border-amber-200 bg-amber-50 p-3 text-sm text-amber-800">
        سيتم نقل الطلاب المحددين إلى الحلقة المختارة. هل أنت متأكد من هذا الإجراء؟
      </p>
      <DialogActions
        isPending={isPending}
        canSubmit={canModify && circleId !== ''}
        onCancel={() => onOpenChange(false)}
        onSubmit={() => onSubmit(Number(circleId))}
        submitLabel={isPending ? 'جاري النقل...' : 'نقل الطلاب'}
      />
    </DialogShell>
  )
}
