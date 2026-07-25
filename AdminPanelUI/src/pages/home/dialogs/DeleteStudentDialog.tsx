import { DialogActions, DialogShell } from '@/pages/home/dialogs/HomeWhatsappDialog'

interface DeleteStudentDialogProps {
  open: boolean
  studentId: number | null
  isPending: boolean
  onOpenChange: (open: boolean) => void
  onConfirm: () => void
}

export function DeleteStudentDialog({ open, studentId, isPending, onOpenChange, onConfirm }: DeleteStudentDialogProps) {
  if (!open || studentId == null) return null

  return (
    <DialogShell title="تأكيد الحذف" onClose={() => onOpenChange(false)}>
      <p className="mb-4 text-sm text-slate-700">هل أنت متأكد من حذف هذا الطالب؟</p>
      <DialogActions
        isPending={isPending}
        canSubmit={!isPending}
        onCancel={() => onOpenChange(false)}
        onSubmit={onConfirm}
        submitLabel={isPending ? 'جاري الحذف...' : 'حذف'}
      />
    </DialogShell>
  )
}
