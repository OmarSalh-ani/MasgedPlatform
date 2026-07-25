import { z } from 'zod'

export const expensiveFormSchema = z.object({
  reason: z.string().trim().min(1, 'سبب الصرف مطلوب').max(500),
  totalAmount: z.number().min(0, 'القيمة يجب أن تكون صفراً أو أكثر'),
  supplier: z.string().trim().min(1, 'اسم المورد مطلوب').max(250),
  notes: z.string().max(500).optional(),
  files: z.custom<FileList | undefined>().optional(),
})

export type ExpensiveFormValues = z.infer<typeof expensiveFormSchema>
