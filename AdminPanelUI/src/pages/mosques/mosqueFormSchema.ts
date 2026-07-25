import { z } from 'zod'

const allowedExtensions = ['.jpg', '.jpeg', '.png', '.gif']

export const mosqueFormSchema = z.object({
  name: z.string().trim().min(1, 'اسم المسجد مطلوب').max(200),
  description: z.string().optional(),
  googleMapsUrl: z.string().max(500).optional(),
  sortOrder: z.number(),
  imageFile: z
    .instanceof(File)
    .optional()
    .refine(
      (file) => {
        if (!file) return true
        const ext = file.name.slice(file.name.lastIndexOf('.')).toLowerCase()
        return allowedExtensions.includes(ext)
      },
      { message: 'الامتدادات المسموحة: jpg, jpeg, png, gif' },
    ),
})

export type MosqueFormValues = z.infer<typeof mosqueFormSchema>
