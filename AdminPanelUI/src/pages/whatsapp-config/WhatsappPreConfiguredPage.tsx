import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useWhatsappPreConfigured } from '@/hooks/useWhatsappPreConfigured'
import { WhatsappPreConfiguredCard } from '@/pages/whatsapp-config/WhatsappPreConfiguredCard'

export function WhatsappPreConfiguredPage() {
  const { query, saveMutation, enabledMutation, testMutation } = useWhatsappPreConfigured()

  if (query.isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <PageHeader
        title="إدارة رسائل الواتساب المحددة مسبقاً"
        description="تكوين الرسائل التلقائية للأحداث المختلفة"
        className="mb-0"
      />

      {query.isError ? <Alert variant="destructive">تعذر تحميل إعدادات الرسائل</Alert> : null}

      <div className="space-y-5">
        {(query.data ?? []).map((item) => (
          <WhatsappPreConfiguredCard
            key={item.id}
            item={item}
            isSaving={saveMutation.isPending}
            isTesting={testMutation.isPending}
            onSave={(id, message) => saveMutation.mutate({ id, message })}
            onToggleEnabled={(id, isEnabled) => enabledMutation.mutate({ id, isEnabled })}
            onTest={(id) =>
              testMutation.mutate(id, {
                onSuccess: (preview) => window.alert(`رسالة الاختبار:\n\n${preview}`),
              })
            }
          />
        ))}
      </div>
    </div>
  )
}
