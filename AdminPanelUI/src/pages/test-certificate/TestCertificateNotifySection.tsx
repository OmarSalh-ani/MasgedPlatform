import { zodResolver } from '@hookform/resolvers/zod'
import { useEffect, useState } from 'react'
import { useForm } from 'react-hook-form'
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
import { Textarea } from '@/components/ui/textarea'
import { useSendTestCertificateNotification } from '@/hooks/useSendTestCertificateNotification'
import { canModify } from '@/lib/authStorage'
import { SendTestCertificateNotificationDialog } from '@/pages/test-certificate/dialogs/SendTestCertificateNotificationDialog'
import {
  testCertificateNotificationSchema,
  type TestCertificateNotificationFormValues,
} from '@/pages/test-certificate/testCertificateNotificationSchema'
import { buildTestCertificateNotificationDefaults } from '@/pages/test-certificate/testCertificateUtils'
import type { SendTestCertificateNotificationResult } from '@/types/testCertificate'

interface TestCertificateNotifySectionProps {
  testId: number
  studentName: string
  grade: string
}

export function TestCertificateNotifySection({
  testId,
  studentName,
  grade,
}: TestCertificateNotifySectionProps) {
  const userCanModify = canModify()
  const sendMutation = useSendTestCertificateNotification(testId)
  const [confirmOpen, setConfirmOpen] = useState(false)
  const [resultMessage, setResultMessage] = useState<string | null>(null)

  const defaults = buildTestCertificateNotificationDefaults(studentName, grade)
  const form = useForm<TestCertificateNotificationFormValues>({
    resolver: zodResolver(testCertificateNotificationSchema),
    defaultValues: defaults,
  })

  useEffect(() => {
    form.reset(buildTestCertificateNotificationDefaults(studentName, grade))
  }, [form, grade, studentName])

  const title = form.watch('title')
  const body = form.watch('body')

  const handleConfirmSend = () => {
    sendMutation.mutate(
      { title: title.trim(), body: body.trim() },
      {
        onSuccess: (result) => {
          setConfirmOpen(false)
          setResultMessage(formatResultMessage(result))
        },
      },
    )
  }

  return (
    <div className="no-print mx-auto mb-6 max-w-3xl rounded-lg border bg-white p-4 shadow-sm">
      <h2 className="mb-4 text-lg font-semibold text-slate-800">إرسال إشعار لولي الأمر</h2>

      {resultMessage ? (
        <Alert className="mb-4">
          <p className="text-sm">{resultMessage}</p>
        </Alert>
      ) : null}

      <Form {...form}>
        <form
          className="space-y-4"
          onSubmit={form.handleSubmit(() => setConfirmOpen(true))}
        >
          <FormField
            control={form.control}
            name="title"
            render={({ field }) => (
              <FormItem>
                <FormLabel>عنوان الإشعار</FormLabel>
                <FormControl>
                  <Input {...field} disabled={!userCanModify} />
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
                  <Textarea rows={4} {...field} disabled={!userCanModify} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          {sendMutation.isError ? (
            <Alert variant="destructive">
              <p className="text-sm">تعذر إرسال الإشعار. يرجى المحاولة مرة أخرى.</p>
            </Alert>
          ) : null}

          <Button type="submit" disabled={!userCanModify || sendMutation.isPending}>
            {sendMutation.isPending ? 'جاري الإرسال...' : 'إرسال الإشعار'}
          </Button>
        </form>
      </Form>

      <SendTestCertificateNotificationDialog
        open={confirmOpen}
        studentName={studentName}
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

function formatResultMessage(result: SendTestCertificateNotificationResult): string {
  return [
    'تم إرسال الإشعار.',
    `المستهدفون: ${result.recipientsResolved}`,
    `بدون أجهزة مسجلة: ${result.recipientsWithoutTokens}`,
    `الأجهزة: ${result.tokensAttempted}`,
    `نجح: ${result.successCount}`,
    `فشل: ${result.failureCount}`,
  ].join(' ')
}
