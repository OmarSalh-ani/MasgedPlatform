import { z } from 'zod'

export const pushNotificationSchema = z
  .object({
    audience: z.enum(['teachers', 'parents']),
    targetAll: z.boolean(),
    teacherIds: z.array(z.number()),
    studentIds: z.array(z.number()),
    title: z.string().trim().min(1, 'يرجى كتابة عنوان الإشعار').max(100, 'العنوان طويل جداً'),
    body: z.string().trim().min(1, 'يرجى كتابة نص الإشعار').max(500, 'نص الإشعار طويل جداً'),
  })
  .superRefine((values, ctx) => {
    if (values.targetAll) return

    if (values.audience === 'teachers' && values.teacherIds.length === 0) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: 'يرجى اختيار معلم واحد على الأقل',
        path: ['teacherIds'],
      })
    }

    if (values.audience === 'parents' && values.studentIds.length === 0) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: 'يرجى اختيار طالب واحد على الأقل',
        path: ['studentIds'],
      })
    }
  })

export type PushNotificationFormValues = z.infer<typeof pushNotificationSchema>
