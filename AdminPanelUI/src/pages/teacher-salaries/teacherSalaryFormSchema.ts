import { z } from 'zod'

export const teacherSalaryFormSchema = z.object({
  teacherId: z.number().min(1, 'يرجى اختيار المعلم'),
  month: z.number().min(1, 'يرجى اختيار الشهر').max(12),
  year: z.number().min(1, 'يرجى اختيار السنة'),
  baseSalary: z.number().positive('يرجى إدخال الراتب الأساسي بشكل صحيح'),
  daysAttended: z.number().min(0),
  totalHours: z.number().min(0),
  calculatedSalary: z.number().min(0),
  dayOffDate: z.string().optional(),
  notes: z.string().optional(),
})

export type TeacherSalaryFormValues = z.infer<typeof teacherSalaryFormSchema>
