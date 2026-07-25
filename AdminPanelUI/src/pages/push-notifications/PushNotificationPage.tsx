import { zodResolver } from '@hookform/resolvers/zod'
import { Bell } from 'lucide-react'
import { useState } from 'react'
import { useForm } from 'react-hook-form'
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
import { Textarea } from '@/components/ui/textarea'
import { canModify } from '@/lib/authStorage'
import { usePushNotificationTeachers, useSendPushNotification } from '@/hooks/usePushNotification'
import { SendPushNotificationDialog } from '@/pages/push-notifications/dialogs/SendPushNotificationDialog'
import { PushNotificationParentPicker } from '@/pages/push-notifications/PushNotificationParentPicker'
import { PushNotificationTeacherPicker } from '@/pages/push-notifications/PushNotificationTeacherPicker'
import {
  pushNotificationSchema,
  type PushNotificationFormValues,
} from '@/pages/push-notifications/pushNotificationSchema'
import type { SendPushNotificationResult } from '@/types/pushNotification'

export function PushNotificationPage() {
  const userCanModify = canModify()
  const teachersQuery = usePushNotificationTeachers()
  const sendMutation = useSendPushNotification()
  const [confirmOpen, setConfirmOpen] = useState(false)
  const [resultMessage, setResultMessage] = useState<string | null>(null)

  const form = useForm<PushNotificationFormValues>({
    resolver: zodResolver(pushNotificationSchema),
    defaultValues: {
      audience: 'teachers',
      targetAll: false,
      teacherIds: [],
      studentIds: [],
      title: '',
      body: '',
    },
  })

  const audience = form.watch('audience')
  const targetAll = form.watch('targetAll')
  const teacherIds = form.watch('teacherIds')
  const studentIds = form.watch('studentIds')
  const title = form.watch('title')
  const body = form.watch('body')

  const selectedCount = audience === 'teachers' ? teacherIds.length : studentIds.length

  const onSubmit = () => setConfirmOpen(true)

  const handleConfirmSend = () => {
    sendMutation.mutate(
      {
        audience,
        targetAll,
        teacherIds,
        studentIds,
        title: title.trim(),
        body: body.trim(),
      },
      {
        onSuccess: (result) => {
          setConfirmOpen(false)
          setResultMessage(formatResultMessage(result))
        },
      },
    )
  }

  if (teachersQuery.isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <PageHeader
        title="إشعارات التطبيق"
        description="إرسال إشعار فوري لتطبيق الجوال"
        className="mb-0"
      />

      {resultMessage ? <Alert>{resultMessage}</Alert> : null}
      {sendMutation.isError ? (
        <Alert variant="destructive">تعذر إرسال الإشعار. يرجى المحاولة مرة أخرى.</Alert>
      ) : null}

      <Card className="overflow-hidden border-0 shadow-md">
        <div className="bg-gradient-to-br from-[#7C8738] to-[#5c6a2a] px-6 py-4 text-white">
          <h2 className="font-semibold">إنشاء إشعار جديد</h2>
        </div>
        <div className="p-6">
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
              <FormField
                control={form.control}
                name="audience"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>نوع الجمهور</FormLabel>
                    <div className="flex flex-wrap gap-4">
                      <AudienceOption
                        checked={field.value === 'teachers'}
                        label="المعلمين"
                        onSelect={() => field.onChange('teachers')}
                      />
                      <AudienceOption
                        checked={field.value === 'parents'}
                        label="أولياء الأمور"
                        onSelect={() => field.onChange('parents')}
                      />
                    </div>
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="targetAll"
                render={({ field }) => (
                  <FormItem>
                    <label className="flex items-center gap-2">
                      <input
                        type="checkbox"
                        checked={field.value}
                        onChange={(e) => field.onChange(e.target.checked)}
                      />
                      <span>
                        {audience === 'teachers' ? 'إرسال لجميع المعلمين' : 'إرسال لجميع أولياء الأمور'}
                      </span>
                    </label>
                  </FormItem>
                )}
              />

              {!targetAll && audience === 'teachers' ? (
                <FormField
                  control={form.control}
                  name="teacherIds"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>اختر المعلمين</FormLabel>
                      <PushNotificationTeacherPicker
                        teachers={teachersQuery.data ?? []}
                        selectedIds={field.value}
                        onChange={field.onChange}
                      />
                      <FormMessage />
                    </FormItem>
                  )}
                />
              ) : null}

              {!targetAll && audience === 'parents' ? (
                <FormField
                  control={form.control}
                  name="studentIds"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>اختر الطلاب (يتم الإرسال لولي أمر كل طالب)</FormLabel>
                      <PushNotificationParentPicker
                        selectedStudentIds={field.value}
                        onChange={field.onChange}
                      />
                      <FormMessage />
                    </FormItem>
                  )}
                />
              ) : null}

              <FormField
                control={form.control}
                name="title"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>العنوان</FormLabel>
                    <FormControl>
                      <Input placeholder="عنوان الإشعار" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="body"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>نص الإشعار</FormLabel>
                    <FormControl>
                      <Textarea rows={5} placeholder="اكتب نص الإشعار هنا..." {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <div className="flex justify-end">
                <Button
                  type="submit"
                  className="bg-[#7C8738] hover:bg-[#5c6a2a]"
                  disabled={!userCanModify || sendMutation.isPending}
                >
                  <Bell className="size-4" />
                  {sendMutation.isPending ? 'جاري الإرسال...' : 'إرسال الإشعار'}
                </Button>
              </div>
            </form>
          </Form>
        </div>
      </Card>

      <SendPushNotificationDialog
        open={confirmOpen}
        audience={audience}
        targetAll={targetAll}
        selectedCount={selectedCount}
        title={title}
        body={body}
        isPending={sendMutation.isPending}
        canModify={userCanModify}
        onOpenChange={setConfirmOpen}
        onConfirm={handleConfirmSend}
      />
    </div>
  )
}

function AudienceOption({
  checked,
  label,
  onSelect,
}: {
  checked: boolean
  label: string
  onSelect: () => void
}) {
  return (
    <label className="flex cursor-pointer items-center gap-2 rounded-lg border px-4 py-2">
      <input type="radio" checked={checked} onChange={onSelect} />
      <span>{label}</span>
    </label>
  )
}

function formatResultMessage(result: SendPushNotificationResult): string {
  return [
    'تم إرسال الإشعار.',
    `المستهدفون: ${result.recipientsResolved}`,
    `بدون أجهزة مسجلة: ${result.recipientsWithoutTokens}`,
    `الأجهزة: ${result.tokensAttempted}`,
    `نجح: ${result.successCount}`,
    `فشل: ${result.failureCount}`,
  ].join(' ')
}
