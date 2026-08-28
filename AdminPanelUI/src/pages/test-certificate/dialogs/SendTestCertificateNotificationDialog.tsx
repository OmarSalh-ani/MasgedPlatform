import { DialogActions, DialogShell } from '@/pages/home/dialogs/HomeWhatsappDialog'

interface SendTestCertificateNotificationDialogProps {
  open: boolean
  studentName: string
  title: string
  body: string
  isPending: boolean
  canModify: boolean
  onOpenChange: (open: boolean) => void
  onConfirm: () => void
}

export function SendTestCertificateNotificationDialog({
  open,
  studentName,
  title,
  body,
  isPending,
  canModify,
  onOpenChange,
  onConfirm,
}: SendTestCertificateNotificationDialogProps) {
  if (!open) return null

  return (
    <DialogShell title="تأكيد إرسال الإشعار" onClose={() => onOpenChange(false)}>
      <div className="space-y-3 text-sm text-slate-700">
        <p>
          <span className="font-semibold">ولي الأمر لـ:</span> {studentName}
        </p>
        <p>
          <span className="font-semibold">العنوان:</span> {title}
        </p>
        <p>
          <span className="font-semibold">النص:</span> {body}
        </p>
      </div>

      <p className="mt-4 text-sm text-amber-700">
        سيتم إرسال الإشعار فوراً إلى تطبيق ولي الأمر للأجهزة المسجلة.
      </p>

      <DialogActions
        isPending={isPending}
        canSubmit={canModify}
        onCancel={() => onOpenChange(false)}
        onSubmit={onConfirm}
        submitLabel={isPending ? 'جاري الإرسال...' : 'إرسال الإشعار'}
      />
    </DialogShell>
  )
}
