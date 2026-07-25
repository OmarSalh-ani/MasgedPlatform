import { z } from 'zod'

export const savePlanLevelSchema = z.object({
  levelName: z.string().trim().min(1, 'الاسم مطلوب'),
  unitType: z.union([z.literal(0), z.literal(1), z.literal(3)]),
  quantity: z
    .number({ message: 'الكمية مطلوبة' })
    .int('الكمية يجب أن تكون رقمًا موجبًا')
    .min(1, 'الكمية يجب أن تكون رقمًا موجبًا')
    .max(1000, 'الكمية يجب أن تكون رقمًا موجبًا'),
})

export type SavePlanLevelFormValues = z.infer<typeof savePlanLevelSchema>
