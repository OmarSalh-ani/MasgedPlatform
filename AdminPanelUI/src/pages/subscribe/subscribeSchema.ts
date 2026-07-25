import { z } from 'zod'

export const subscribeSchema = z.object({
  fullName: z.string().trim().min(1, 'يرجى إدخال الاسم الثلاثي'),
  mobile: z
    .string()
    .trim()
    .min(1, 'يرجى إدخال رقم الموبايل')
    .regex(/^[0-9]{8}$/, 'يرجى إدخال رقم الموبايل الصحيح (8 أرقام)'),
})

export type SubscribeFormValues = z.infer<typeof subscribeSchema>
