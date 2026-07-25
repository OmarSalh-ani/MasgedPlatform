import { z } from 'zod'

export const saveWomanActivitySchema = z.object({
  name: z
    .string()
    .trim()
    .min(1, 'يرجى كتابة اسم النشاط أولاً')
    .max(500),
  isVisible: z.boolean(),
})

export type SaveWomanActivityFormValues = z.infer<typeof saveWomanActivitySchema>
