import { useEffect, useMemo, useState } from 'react'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import {
  WHATSAPP_MEET_TAGS,
  WHATSAPP_PARAMETER_TAGS,
  WHATSAPP_REVISE_TAGS,
  WHATSAPP_TEST_TAGS,
  type WhatsappPreConfiguredMessage,
} from '@/types/whatsappPreConfigured'
import { applyWhatsappPreviewTokens } from '@/pages/whatsapp-config/whatsappPreview'

interface WhatsappPreConfiguredCardProps {
  item: WhatsappPreConfiguredMessage
  isSaving: boolean
  isTesting: boolean
  onSave: (id: number, message: string) => void
  onToggleEnabled: (id: number, enabled: boolean) => void
  onTest: (id: number) => void
}

export function WhatsappPreConfiguredCard({
  item,
  isSaving,
  isTesting,
  onSave,
  onToggleEnabled,
  onTest,
}: WhatsappPreConfiguredCardProps) {
  const [message, setMessage] = useState(item.whatsappMessage)
  const preview = useMemo(() => applyWhatsappPreviewTokens(message), [message])

  useEffect(() => {
    setMessage(item.whatsappMessage)
  }, [item.whatsappMessage])

  const extraTags =
    item.event === 'StudentRevise'
      ? WHATSAPP_REVISE_TAGS
      : item.event === 'StudentTest'
        ? WHATSAPP_TEST_TAGS
        : item.event === 'GoogleMeetCreated'
          ? WHATSAPP_MEET_TAGS
          : []

  const insertTag = (tag: string) => setMessage((prev) => prev + tag)

  return (
    <article className="overflow-hidden rounded-xl border bg-white shadow-sm">
      <header className="border-b bg-slate-50 p-5">
        <div className="flex flex-wrap items-center gap-2">
          <h2 className="text-lg font-semibold text-[var(--color-primary)]">{item.eventDisplayName}</h2>
          <span
            className={`rounded-full px-3 py-1 text-xs font-semibold ${
              item.isEnabled ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
            }`}
          >
            {item.isEnabled ? 'مفعل' : 'معطل'}
          </span>
        </div>
        <p className="mt-2 text-sm text-slate-600">{item.eventDescription}</p>
      </header>

      <div className="space-y-4 p-5">
        <div className="space-y-1">
          <Label htmlFor={`message-${item.id}`}>الرسالة</Label>
          <Textarea
            id={`message-${item.id}`}
            rows={4}
            value={message}
            onChange={(e) => setMessage(e.target.value)}
          />
        </div>

        <div className="rounded-lg bg-slate-50 p-4">
          <p className="mb-2 font-semibold text-[var(--color-primary)]">المعاملات المتاحة</p>
          <div className="flex flex-wrap gap-2">
            {[...WHATSAPP_PARAMETER_TAGS, ...extraTags].map((tag) => (
              <Button key={tag} type="button" variant="outline" className="h-8 px-3 text-xs" onClick={() => insertTag(tag)}>
                {tag}
              </Button>
            ))}
          </div>
        </div>

        <div className="rounded-lg border bg-slate-50 p-4">
          <p className="mb-2 font-semibold text-[var(--color-primary)]">معاينة الرسالة</p>
          <pre className="whitespace-pre-wrap rounded-md border bg-white p-3 text-sm">{preview}</pre>
        </div>

        <label className="flex items-center gap-3">
          <input
            type="checkbox"
            checked={item.isEnabled}
            onChange={(e) => onToggleEnabled(item.id, e.target.checked)}
          />
          <span>تفعيل هذا الحدث</span>
        </label>

        <div className="flex flex-wrap justify-end gap-2">
          <Button type="button" onClick={() => onSave(item.id, message)} disabled={isSaving}>
            {isSaving ? 'جاري الحفظ...' : 'حفظ التغييرات'}
          </Button>
          <Button type="button" variant="outline" onClick={() => onTest(item.id)} disabled={isTesting}>
            {isTesting ? 'جاري الاختبار...' : 'اختبار الرسالة'}
          </Button>
        </div>
      </div>
    </article>
  )
}
