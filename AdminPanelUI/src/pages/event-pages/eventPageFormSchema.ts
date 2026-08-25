import { z } from 'zod'

const allowedExtensions = ['.jpg', '.jpeg', '.png', '.gif']
const slugPattern = /^[a-z0-9]+(?:-[a-z0-9]+)*$/

export function parseOptionsText(value?: string): string[] {
  return (value ?? '')
    .split('\n')
    .map((line) => line.trim())
    .filter((line) => line.length > 0)
}

const trackSchema = z.object({
  title: z.string().trim().min(1, 'عنوان المسار مطلوب').max(300),
  description: z.string().optional(),
  sortOrder: z.number(),
})

const fieldSchema = z
  .object({
    fieldId: z.number().optional(),
    label: z.string().trim().min(1, 'عنوان الحقل مطلوب').max(300),
    fieldType: z.enum(['Text', 'Number', 'SingleSelect', 'MultiSelect']),
    isRequired: z.boolean(),
    sortOrder: z.number(),
    optionsText: z.string().optional(),
  })
  .superRefine((value, ctx) => {
    if (value.fieldType !== 'SingleSelect' && value.fieldType !== 'MultiSelect') return
    if (parseOptionsText(value.optionsText).length === 0) {
      ctx.addIssue({
        code: 'custom',
        message: 'خيارات الحقل مطلوبة',
        path: ['optionsText'],
      })
    }
  })

export const eventPageFormSchema = z.object({
  activityName: z.string().trim().min(1, 'اسم النشاط مطلوب').max(200),
  slug: z
    .string()
    .trim()
    .min(1, 'رابط الصفحة مطلوب')
    .max(120)
    .regex(slugPattern, 'الرابط يجب أن يكون أحرفاً إنجليزية صغيرة وأرقاماً وشرطات فقط'),
  courseTitle: z.string().trim().min(1, 'عنوان الدورة مطلوب').max(300),
  invitationText: z.string().max(500).optional(),
  mosqueName: z.string().max(300).optional(),
  subjectText: z.string().max(1000).optional(),
  dateText: z.string().max(300).optional(),
  timeText: z.string().max(300).optional(),
  extraNotes: z.string().optional(),
  supervisorsText: z.string().max(1000).optional(),
  contactPhone: z.string().max(50).optional(),
  socialAccounts: z.string().max(200).optional(),
  locationNote: z.string().max(500).optional(),
  isPublished: z.boolean(),
  isRegistrationOpen: z.boolean(),
  imageFile: z
    .instanceof(File)
    .optional()
    .refine((file) => {
      if (!file) return true
      const ext = file.name.slice(file.name.lastIndexOf('.')).toLowerCase()
      return allowedExtensions.includes(ext)
    }, { message: 'الامتدادات المسموحة: jpg, jpeg, png, gif' }),
  tracks: z.array(trackSchema),
  formFields: z.array(fieldSchema),
})

export type EventPageFormValues = z.infer<typeof eventPageFormSchema>

export const eventPageFormDefaults: EventPageFormValues = {
  activityName: '',
  slug: '',
  courseTitle: '',
  invitationText: '',
  mosqueName: '',
  subjectText: '',
  dateText: '',
  timeText: '',
  extraNotes: '',
  supervisorsText: '',
  contactPhone: '',
  socialAccounts: '',
  locationNote: '',
  isPublished: true,
  isRegistrationOpen: true,
  tracks: [],
  formFields: [],
}
