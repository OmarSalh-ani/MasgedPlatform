import { useState } from 'react'
import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'

interface AttendanceWhatsappDialogProps {
  open: boolean
  selectedCount: number
  isPending: boolean
  canModify: boolean
  onOpenChange: (open: boolean) => void
  onSubmit: (payload: { message: string; image?: File | null }) => void
}

const TEMPLATE_TOKENS = ['{أسم الطالب}', '{أسم الأب}', '{أسم الحلقة}', '{الرابط}'] as const

export function AttendanceWhatsappDialog({
  open,
  selectedCount,
  isPending,
  canModify,
  onOpenChange,
  onSubmit,
}: AttendanceWhatsappDialogProps) {
  const { masgedName } = useMasgedBranding()
  const [message, setMessage] = useState('')
  const [image, setImage] = useState<File | null>(null)

  if (!open) return null

  const appendText = (text: string) => setMessage((prev) => prev + text)

  const handleSubmit = () => {
    onSubmit({ message, image })
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
      <div className="max-h-[90vh] w-full max-w-lg overflow-y-auto rounded-xl bg-white p-6 shadow-lg">
        <div className="mb-4 flex items-center justify-between">
          <h3 className="text-lg font-semibold">إرسال رسالة واتساب</h3>
          <Button type="button" variant="outline" className="h-8 px-3 text-sm" onClick={() => onOpenChange(false)}>
            إغلاق
          </Button>
        </div>

        <p className="mb-4 text-sm text-slate-600">السجلات المحددة: {selectedCount}</p>

        <div className="mb-3 flex flex-wrap gap-2">
          {TEMPLATE_TOKENS.map((token) => (
            <Button key={token} type="button" variant="outline" className="h-8 px-3 text-sm" onClick={() => appendText(token)}>
              {token}
            </Button>
          ))}
        </div>

        <div className="mb-3 space-y-1">
          <Label htmlFor="whatsappMessage">الرسالة</Label>
          <Textarea
            id="whatsappMessage"
            rows={6}
            value={message}
            placeholder="اكتب رسالتك هنا..."
            onChange={(e) => setMessage(e.target.value)}
          />
        </div>

        <div className="mb-3 space-y-1">
          <Label htmlFor="whatsappImage">الصورة (اختياري)</Label>
          <input
            id="whatsappImage"
            type="file"
            accept="image/*"
            onChange={(e) => setImage(e.target.files?.[0] ?? null)}
          />
        </div>

        <div className="mb-4 flex flex-wrap gap-2">
          <Button type="button" variant="outline" className="h-8 px-3 text-sm" onClick={() => appendText(REMINDER_TEXT)}>
            تذكير بحلقة صلاة المغرب
          </Button>
          <Button
            type="button"
            variant="outline"
            className="h-8 px-3 text-sm"
            onClick={() => appendText(buildRegisterLinkText(masgedName))}
          >
            رابط إكمال البيانات
          </Button>
        </div>

        <div className="flex justify-end gap-2">
          <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
            إلغاء
          </Button>
          <Button type="button" disabled={!canModify || isPending} onClick={handleSubmit}>
            {isPending ? 'جاري الإرسال...' : 'إرسال'}
          </Button>
        </div>
      </div>
    </div>
  )
}

const REMINDER_TEXT = `السلام عليكم ورحمة الله وبركاته
تذكير الحلقة بعد صلاة المغرب للطالب
{أسم الطالب}
{أسم الحلقة}`

function buildRegisterLinkText(masgedName: string) {
  return `السلام عليكم ورحمة الله وبركاته
حياكم الله في حلقات تحفيظ القرآن الكريم في ${masgedName}،

نتمنى منكم التكرم بالدخول على الرابط التالي لإكمال بيانات تسجيل {أسم الطالب}، وذلك حتى نتمكن من المتابعة الدقيقة، والحرص على تقديم أفضل رعاية وتعامل مع أبنائنا الطلاب.

📌 رابط التسجيل: {الرابط}

بارك الله فيكم وجزاكم خيرًا على تعاونكم`
}
