import { z } from 'zod'

const allowedExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.webp']

export const newsFormSchema = z.object({
  title: z.string().trim().min(1, 'العنوان مطلوب').max(300),
  description: z.string().optional(),
  newsDate: z.string().min(1),
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
      { message: 'الامتدادات المسموحة: jpg, jpeg, png, gif, webp' },
    ),
})

export type NewsFormValues = z.infer<typeof newsFormSchema>

export function todayDateInputValue(): string {
  return new Date().toISOString().slice(0, 10)
}

export function toNewsDateInputValue(isoDate: string): string {
  return isoDate.slice(0, 10)
}
