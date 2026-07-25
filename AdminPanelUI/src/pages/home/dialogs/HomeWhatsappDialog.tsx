import { useEffect, useRef, useState } from 'react'
import { createPortal } from 'react-dom'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'

interface HomeWhatsappDialogProps {
  open: boolean
  selectedCount: number
  isPending: boolean
  canModify: boolean
  onOpenChange: (open: boolean) => void
  onSubmit: (payload: { message: string; image?: File | null }) => void
}

const TEMPLATE_TOKENS = ['{أسم الطالب}', '{أسم الأب}', '{أسم الحلقة}', '{الرابط}'] as const

export function HomeWhatsappDialog({
  open,
  selectedCount,
  isPending,
  canModify,
  onOpenChange,
  onSubmit,
}: HomeWhatsappDialogProps) {
  const [message, setMessage] = useState('')
  const [image, setImage] = useState<File | null>(null)

  if (!open) return null

  return (
    <DialogShell title="إرسال رسالة واتساب" onClose={() => onOpenChange(false)}>
      <p className="mb-4 text-sm text-slate-600">الطلاب المحددين: {selectedCount}</p>
      <div className="mb-3 flex flex-wrap gap-2">
        {TEMPLATE_TOKENS.map((token) => (
          <Button key={token} type="button" variant="outline" className="h-8 px-3 text-sm" onClick={() => setMessage((prev) => prev + token)}>
            {token}
          </Button>
        ))}
      </div>
      <div className="mb-3 space-y-1">
        <Label htmlFor="homeWhatsappMessage">الرسالة</Label>
        <Textarea id="homeWhatsappMessage" rows={6} value={message} placeholder="اكتب رسالتك هنا..." onChange={(e) => setMessage(e.target.value)} />
      </div>
      <div className="mb-4 space-y-1">
        <Label htmlFor="homeWhatsappImage">الصورة (اختياري)</Label>
        <input id="homeWhatsappImage" type="file" accept="image/*" onChange={(e) => setImage(e.target.files?.[0] ?? null)} />
      </div>
      <DialogActions
        isPending={isPending}
        canSubmit={canModify}
        onCancel={() => onOpenChange(false)}
        onSubmit={() => onSubmit({ message, image })}
        submitLabel={isPending ? 'جاري الإرسال...' : 'إرسال'}
      />
    </DialogShell>
  )
}

function DialogShell({ title, children, onClose }: { title: string; children: React.ReactNode; onClose: () => void }) {
  const panelRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    document.body.style.overflow = 'hidden'
    panelRef.current?.scrollTo(0, 0)
    return () => { document.body.style.overflow = '' }
  }, [])

  return createPortal(
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <div ref={panelRef} className="flex h-full w-full flex-col overflow-y-auto bg-white sm:h-auto sm:max-w-lg sm:rounded-xl sm:shadow-lg">
        <div className="sticky top-0 z-10 flex items-center justify-between border-b bg-white p-4">
          <h3 className="text-lg font-semibold">{title}</h3>
          <Button type="button" variant="outline" className="h-8 px-3 text-sm" onClick={onClose}>إغلاق</Button>
        </div>
        <div className="flex-1 p-4 sm:p-6">{children}</div>
      </div>
    </div>,
    document.body,
  )
}

function DialogActions({
  onCancel,
  onSubmit,
  isPending,
  canSubmit,
  submitLabel,
}: {
  onCancel: () => void
  onSubmit: () => void
  isPending: boolean
  canSubmit: boolean
  submitLabel: string
}) {
  return (
    <div className="flex justify-end gap-2">
      <Button type="button" variant="outline" onClick={onCancel}>إلغاء</Button>
      <Button type="button" disabled={!canSubmit || isPending} onClick={onSubmit}>{submitLabel}</Button>
    </div>
  )
}

export { DialogShell, DialogActions }
