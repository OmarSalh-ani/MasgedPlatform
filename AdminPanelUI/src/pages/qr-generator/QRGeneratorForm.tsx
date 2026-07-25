import { zodResolver } from '@hookform/resolvers/zod'
import { QrCode, Wand2 } from 'lucide-react'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form'
import { Input } from '@/components/ui/input'
import { isValidUrl } from '@/lib/qrCode'
import { QR_COLOR_OPTIONS, QR_COLOR_PREVIEW_CLASS } from '@/types/qrGenerator'

const qrGeneratorSchema = z.object({
  url: z
    .string()
    .trim()
    .min(1, 'يرجى إدخال رابط صحيح')
    .refine(isValidUrl, 'يرجى إدخال رابط صحيح يبدأ بـ http:// أو https://'),
  color: z.string(),
})

export type QRGeneratorFormValues = z.infer<typeof qrGeneratorSchema>

interface QRGeneratorFormProps {
  isGenerating: boolean
  errorMessage: string | null
  successMessage: string | null
  onSubmit: (values: QRGeneratorFormValues) => void
}

const selectClassName =
  'h-10 w-full rounded-lg border-2 border-slate-200 bg-white px-3 text-sm focus:border-[#7C8738] focus:outline-none focus:ring-2 focus:ring-[#7C8738]/20'

export function QRGeneratorForm({
  isGenerating,
  errorMessage,
  successMessage,
  onSubmit,
}: QRGeneratorFormProps) {
  const form = useForm<QRGeneratorFormValues>({
    resolver: zodResolver(qrGeneratorSchema),
    defaultValues: { url: '', color: '#7C8738' },
  })

  const selectedColor = form.watch('color')

  return (
    <section className="mb-8 rounded-xl border bg-white p-6 shadow-sm">
      <h2 className="mb-5 flex items-center gap-2 text-xl font-semibold text-[#7C8738]">
        <QrCode className="h-5 w-5" />
        إنشاء رمز QR جديد
      </h2>

      <Form {...form}>
        <form
          className="flex flex-col gap-5"
          onSubmit={form.handleSubmit(onSubmit)}
        >
          <FormField
            control={form.control}
            name="url"
            render={({ field }) => (
              <FormItem>
                <FormLabel>أدخل الرابط</FormLabel>
                <FormControl>
                  <Input
                    type="url"
                    placeholder="https://example.com"
                    dir="ltr"
                    {...field}
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="color"
            render={({ field }) => (
              <FormItem>
                <FormLabel>اختر اللون</FormLabel>
                <FormControl>
                  <select className={selectClassName} {...field}>
                    {QR_COLOR_OPTIONS.map((option) => (
                      <option key={option.value} value={option.value}>
                        {option.label}
                      </option>
                    ))}
                  </select>
                </FormControl>
                <div
                  className={`mt-2 inline-block h-8 w-8 rounded-full border-2 border-slate-300 ${QR_COLOR_PREVIEW_CLASS[selectedColor] ?? 'bg-[#7C8738]'}`}
                />
              </FormItem>
            )}
          />

          <Button type="submit" disabled={isGenerating} className="w-fit gap-2">
            <Wand2 className="h-4 w-4" />
            {isGenerating ? 'جاري إنشاء رمز QR...' : 'إنشاء QR'}
          </Button>
        </form>
      </Form>

      {isGenerating && (
        <div className="mt-4 flex flex-col items-center gap-2 py-4">
          <div className="h-10 w-10 animate-spin rounded-full border-4 border-slate-200 border-t-[#7C8738]" />
          <p className="text-sm text-slate-600">جاري إنشاء رمز QR...</p>
        </div>
      )}

      {errorMessage && <Alert variant="destructive" className="mt-4">{errorMessage}</Alert>}
      {successMessage && (
        <Alert className="mt-4 border-green-200 bg-green-50 text-green-800">{successMessage}</Alert>
      )}
    </section>
  )
}
