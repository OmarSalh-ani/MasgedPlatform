import { z } from 'zod'

export const testCertificateNotificationSchema = z.object({
  title: z.string().trim().min(1, 'يرجى كتابة عنوان الإشعار').max(100, 'العنوان طويل جداً'),
  body: z.string().trim().min(1, 'يرجى كتابة نص الإشعار').max(500, 'نص الإشعار طويل جداً'),
})

export type TestCertificateNotificationFormValues = z.infer<
  typeof testCertificateNotificationSchema
>
