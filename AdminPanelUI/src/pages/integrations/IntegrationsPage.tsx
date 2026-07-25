import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Card } from '@/components/ui/card'
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form'
import { Input } from '@/components/ui/input'
import { Skeleton } from '@/components/ui/skeleton'
import { useIntegrations } from '@/hooks/useIntegrations'

const schema = z.object({
  wasenderApiToken: z.string().optional(),
  wasenderSessionApiKey: z.string().optional(),
  agoraAppId: z.string().optional(),
  agoraAppCertificate: z.string().optional(),
})

type FormValues = z.infer<typeof schema>

function StatusBadge({ configured, hint }: { configured: boolean; hint: string | null }) {
  return (
    <p className="text-xs text-slate-500">
      {configured ? (
        <>
          مُعدّ حالياً{hint ? ` (${hint})` : ''} — اترك الحقل فارغاً للإبقاء، أو اكتب قيمة جديدة
        </>
      ) : (
        'غير مُعدّ — أدخل القيمة للحفظ'
      )}
    </p>
  )
}

export function IntegrationsPage() {
  const { query, mutation } = useIntegrations()

  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      wasenderApiToken: '',
      wasenderSessionApiKey: '',
      agoraAppId: '',
      agoraAppCertificate: '',
    },
  })

  const onSubmit = (values: FormValues) => {
    const payload: Record<string, string | null> = {}
    if (values.wasenderApiToken?.trim())
      payload.wasenderApiToken = values.wasenderApiToken.trim()
    if (values.wasenderSessionApiKey?.trim())
      payload.wasenderSessionApiKey = values.wasenderSessionApiKey.trim()
    if (values.agoraAppId?.trim()) payload.agoraAppId = values.agoraAppId.trim()
    if (values.agoraAppCertificate?.trim())
      payload.agoraAppCertificate = values.agoraAppCertificate.trim()

    if (Object.keys(payload).length === 0) {
      form.setError('root', { message: 'أدخل قيمة واحدةً واحدةً على الأقل للحفظ' })
      return
    }

    mutation.mutate(payload, {
      onSuccess: () => {
        form.reset({
          wasenderApiToken: '',
          wasenderSessionApiKey: '',
          agoraAppId: '',
          agoraAppCertificate: '',
        })
      },
    })
  }

  if (query.isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  if (query.isError) {
    return (
      <Alert variant="destructive">تعذر تحميل إعدادات التكامل.</Alert>
    )
  }

  const data = query.data

  return (
    <div>
      <PageHeader
        title="التكاملات"
        description="مفاتيح WhatsApp (Wasender) و Agora — تُحفظ في قاعدة البيانات وتتجاوز قيم .env عند التعيين"
      />

      {mutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر الحفظ. تحقق من الصلاحيات وحاول مرة أخرى.
        </Alert>
      )}
      {mutation.isSuccess && (
        <Alert className="mb-4 border-green-200 bg-green-50 text-green-800">
          تم حفظ التكاملات بنجاح.
        </Alert>
      )}
      {form.formState.errors.root && (
        <Alert variant="destructive" className="mb-4">
          {form.formState.errors.root.message}
        </Alert>
      )}

      <Card className="max-w-2xl p-6">
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
            <div className="space-y-4">
              <h3 className="text-sm font-semibold">WhatsApp (Wasender)</h3>
              <FormField
                control={form.control}
                name="wasenderApiToken"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Api Token</FormLabel>
                    <StatusBadge
                      configured={!!data?.wasenderApiTokenConfigured}
                      hint={data?.wasenderApiTokenHint ?? null}
                    />
                    <FormControl>
                      <Input type="password" dir="ltr" autoComplete="off" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="wasenderSessionApiKey"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Session API Key</FormLabel>
                    <StatusBadge
                      configured={!!data?.wasenderSessionApiKeyConfigured}
                      hint={data?.wasenderSessionApiKeyHint ?? null}
                    />
                    <FormControl>
                      <Input type="password" dir="ltr" autoComplete="off" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <div className="space-y-4">
              <h3 className="text-sm font-semibold">Agora (مكالمات الفيديو)</h3>
              <FormField
                control={form.control}
                name="agoraAppId"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>App Id</FormLabel>
                    <StatusBadge
                      configured={!!data?.agoraAppIdConfigured}
                      hint={data?.agoraAppIdHint ?? null}
                    />
                    <FormControl>
                      <Input type="password" dir="ltr" autoComplete="off" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="agoraAppCertificate"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>App Certificate</FormLabel>
                    <StatusBadge
                      configured={!!data?.agoraAppCertificateConfigured}
                      hint={data?.agoraAppCertificateHint ?? null}
                    />
                    <FormControl>
                      <Input type="password" dir="ltr" autoComplete="off" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <Button type="submit" disabled={mutation.isPending}>
              {mutation.isPending ? 'جاري الحفظ...' : 'حفظ'}
            </Button>
          </form>
        </Form>
      </Card>
    </div>
  )
}
