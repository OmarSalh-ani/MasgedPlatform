import { z } from 'zod'

const allowedExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.webp']

const mapLocationSchema = z.object({
  url: z.string().trim().min(1),
  lat: z.string().optional(),
  lng: z.string().optional(),
})

const teacherFormBaseSchema = z.object({
  name: z.string().trim().min(1, 'يرجى إدخال أسم المعلم'),
  mobile: z.string().optional(),
  email: z.string().trim().min(1, 'يرجى إدخال البريد الإلكتروني'),
  password: z.string().optional(),
  baseSalary: z.string().optional(),
  circleId: z.string().optional(),
  isGirlTeacher: z.boolean(),
  usersManage: z.boolean(),
  isViewOnly: z.boolean(),
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
  manualLocations: z.array(mapLocationSchema),
  selectedMosqueIds: z.array(z.number()),
})

export function getTeacherFormSchema(isEdit: boolean) {
  if (isEdit) return teacherFormBaseSchema
  return teacherFormBaseSchema.extend({
    password: z.string().trim().min(1, 'يرجى إدخال كلمة المرور'),
  })
}

export type TeacherFormValues = z.infer<typeof teacherFormBaseSchema>

export function parseBaseSalary(value?: string): number | null {
  const trimmed = value?.trim()
  if (!trimmed) return null
  const parsed = Number(trimmed)
  return Number.isFinite(parsed) ? parsed : null
}
