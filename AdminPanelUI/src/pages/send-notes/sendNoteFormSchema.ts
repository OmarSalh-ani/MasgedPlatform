import { z } from 'zod'

export const createSendNoteFormSchema = z.object({
  teacherIds: z.array(z.number()).min(1, 'يرجى اختيار معلم واحد على الأقل'),
  note: z
    .string()
    .min(1, 'يرجى كتابة نص الملاحظة')
    .refine((value) => value.trim().length > 0, 'يرجى كتابة نص الملاحظة'),
})

export const editSendNoteFormSchema = z.object({
  note: z
    .string()
    .min(1, 'يرجى كتابة نص الملاحظة')
    .refine((value) => value.trim().length > 0, 'يرجى كتابة نص الملاحظة'),
})

export type CreateSendNoteFormValues = z.infer<typeof createSendNoteFormSchema>
export type EditSendNoteFormValues = z.infer<typeof editSendNoteFormSchema>
