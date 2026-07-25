import { z } from 'zod'
import type { SaveCirclePayload } from '@/types/circle'

export const circleFormSchema = z.object({
  name: z.string().trim().min(1, 'يرجى إدخال اسم الحلقة'),
  teacherId: z.string(),
  forGirls: z.boolean(),
})

export type CircleFormValues = z.infer<typeof circleFormSchema>

export const circleFormDefaultValues: CircleFormValues = {
  name: '',
  teacherId: '',
  forGirls: false,
}

export function toSaveCirclePayload(values: CircleFormValues): SaveCirclePayload {
  return {
    name: values.name.trim(),
    teacherId: values.teacherId ? Number(values.teacherId) : null,
    forGirls: values.forGirls,
  }
}
