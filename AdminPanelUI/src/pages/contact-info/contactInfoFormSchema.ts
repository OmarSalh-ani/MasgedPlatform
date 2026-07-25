import { z } from 'zod'

export const contactInfoFormSchema = z.object({
  contactType: z.string().trim().min(1, 'نوع التواصل مطلوب').max(50),
  label: z.string().max(100).optional(),
  value: z.string().trim().min(1, 'القيمة مطلوبة').max(500),
  sortOrder: z.number(),
})

export type ContactInfoFormValues = z.infer<typeof contactInfoFormSchema>
