import { DialogActions, DialogShell } from '@/pages/home/dialogs/HomeWhatsappDialog'
import type { PushNotificationAudience } from '@/types/pushNotification'

interface SendPushNotificationDialogProps {
  open: boolean
  audience: PushNotificationAudience
  targetAll: boolean
  selectedCount: number
  title: string
  body: string
  isPending: boolean
  canModify: boolean
  onOpenChange: (open: boolean) => void
  onConfirm: () => void
}

export function SendPushNotificationDialog({
  open,
  audience,
  targetAll,
  selectedCount,
  title,
  body,
  isPending,
  canModify,
  onOpenChange,
  onConfirm,
}: SendPushNotificationDialogProps) {
  if (!open) return null

  const audienceLabel = audience === 'teachers' ? 'المعلمين' : 'أولياء الأمور'
  const targetLabel = targetAll ? `جميع ${audienceLabel}` : `${selectedCount} ${audience === 'teachers' ? 'معلم' : 'طالب'}`

  return (
    <DialogShell title="تأكيد إرسال الإشعار" onClose={() => onOpenChange(false)}>
      <div className="space-y-3 text-sm text-slate-700">
        <p>
          <span className="font-semibold">الجمهور:</span> {audienceLabel}
        </p>
        <p>
          <span className="font-semibold">المستهدفون:</span> {targetLabel}
        </p>
        <p>
          <span className="font-semibold">العنوان:</span> {title}
        </p>
        <p>
          <span className="font-semibold">النص:</span> {body}
        </p>
      </div>

      <p className="mt-4 text-sm text-amber-700">
        سيتم إرسال الإشعار فوراً عبر تطبيق الجوال للأجهزة المسجلة.
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
