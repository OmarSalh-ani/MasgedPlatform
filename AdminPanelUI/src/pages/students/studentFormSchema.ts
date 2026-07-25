import { z } from 'zod'

export const studentFormSchema = z.object({
  fullName: z.string().trim().min(1, 'يرجى إدخال اسم الطالب'),
  fatherPhone: z.string().trim().min(1, 'يرجى إدخال رقم الهاتف'),
  alternativePhone: z.string().optional(),
  parentPanelPassword: z.string().optional(),
  age: z.string().optional(),
  studentGender: z.enum(['ذكر', 'أنثى']),
  quranCircleId: z.string().optional(),
  planLevelId: z.string().optional(),
  isSpecial: z.boolean(),
})

export type StudentFormValues = z.infer<typeof studentFormSchema>

export const studentFormDefaultValues: StudentFormValues = {
  fullName: '',
  fatherPhone: '',
  alternativePhone: '',
  parentPanelPassword: '',
  age: '',
  studentGender: 'ذكر',
  quranCircleId: '',
  planLevelId: '',
  isSpecial: false,
}

export function parseStudentAge(age: string | undefined): number | null {
  if (!age?.trim()) return null
  const parsed = Number.parseInt(age, 10)
  return Number.isNaN(parsed) ? null : parsed
}
