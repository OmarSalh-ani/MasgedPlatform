import { useState } from 'react'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import { DialogActions, DialogShell } from '@/pages/home/dialogs/HomeWhatsappDialog'
import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'
import {
  buildWhatsappRegisterLinkTemplate,
  WHATSAPP_REMINDER_TEMPLATE,
} from '@/pages/whatsapp-sender/whatsappSenderTemplates'
import type { WhatsappSenderFormOption } from '@/types/whatsappSender'

const TEMPLATE_TOKENS = [
  '{أسم الطالب}',
  '{أسم الأب}',
  '{أسم الحلقة}',
  '{الرابط}',
  '{اسم النموذج}',
  '{رابط النموذج}',
] as const

interface WhatsappSenderDialogProps {
  open: boolean
  selectedCount: number
  formOptions: WhatsappSenderFormOption[]
  isPending: boolean
  canModify: boolean
  onOpenChange: (open: boolean) => void
  onSubmit: (payload: { message: string; formId?: number | null; image?: File | null }) => void
}

export function WhatsappSenderDialog({
  open,
  selectedCount,
  formOptions,
  isPending,
  canModify,
  onOpenChange,
  onSubmit,
}: WhatsappSenderDialogProps) {
  const { masgedName } = useMasgedBranding()
  const [message, setMessage] = useState('')
  const [formId, setFormId] = useState('')
  const [image, setImage] = useState<File | null>(null)

  if (!open) return null

  const appendToMessage = (text: string) => setMessage((prev) => prev + text)

  const addReminderText = () => appendToMessage(WHATSAPP_REMINDER_TEMPLATE)

  const addRegisterLinkText = () => appendToMessage(buildWhatsappRegisterLinkTemplate(masgedName))

  return (
    <DialogShell title="إرسال رسالة واتساب" onClose={() => onOpenChange(false)}>
      <p className="mb-4 text-sm text-slate-600">الطلاب المحددين: {selectedCount}</p>

      <div className="mb-3 space-y-1">
        <Label htmlFor="whatsappFormLink">رابط النموذج (اختياري)</Label>
        <select
          id="whatsappFormLink"
          className="w-full rounded-md border px-3 py-2"
          value={formId}
          onChange={(e) => setFormId(e.target.value)}
        >
          <option value="">— لا نموذج —</option>
          {formOptions.map((form) => (
            <option key={form.id} value={form.id}>{form.title}</option>
          ))}
        </select>
        <p className="text-xs text-slate-500">
          اختر نموذجاً ثم استخدم {'{اسم النموذج}'} و {'{رابط النموذج}'} لرابط النموذج مع رقم الطالب تلقائياً.
        </p>
      </div>

      <div className="mb-3 flex flex-wrap gap-2">
        {TEMPLATE_TOKENS.map((token) => (
          <Button key={token} type="button" variant="outline" className="h-8 px-3 text-sm" onClick={() => appendToMessage(token)}>
            {token}
          </Button>
        ))}
      </div>

      <div className="mb-3 space-y-1">
        <Label htmlFor="whatsappSenderMessage">الرسالة</Label>
        <Textarea
          id="whatsappSenderMessage"
          rows={6}
          value={message}
          placeholder="اكتب رسالتك هنا... استخدم {رابط النموذج} لرابط النموذج مع رقم الطالب"
          onChange={(e) => setMessage(e.target.value)}
        />
      </div>

      <div className="mb-3 space-y-1">
        <Label htmlFor="whatsappSenderImage">الصورة (اختياري)</Label>
        <input id="whatsappSenderImage" type="file" accept="image/*" onChange={(e) => setImage(e.target.files?.[0] ?? null)} />
      </div>

      <div className="mb-4 flex flex-wrap gap-2">
        <Button type="button" variant="outline" onClick={addReminderText}>تذكير بحلقة صلاة المغرب</Button>
        <Button type="button" variant="outline" onClick={addRegisterLinkText}>رابط إكمال البيانات</Button>
      </div>

      <DialogActions
        isPending={isPending}
        canSubmit={canModify}
        onCancel={() => onOpenChange(false)}
        onSubmit={() =>
          onSubmit({
            message,
            formId: formId ? Number(formId) : null,
            image,
          })
        }
        submitLabel={isPending ? 'جاري الإرسال...' : 'إرسال'}
      />
    </DialogShell>
  )
}
