import { z } from 'zod'
import {
  GENDER_OPTIONS,
  MARITAL_STATUS_OPTIONS,
  PHOTO_ALLOWED_TYPES,
  PHOTO_MAX_BYTES,
  YES_NO_OPTIONS,
} from '@/pages/parents-followup/parentsFollowup.constants'

import {
  PHOTO_ASPECT_ERROR_MESSAGE,
  validatePhotoAspectRatio,
} from '@/pages/parents-followup/parentsFollowupPhotoValidation'

const genderValues: string[] = GENDER_OPTIONS.map((o) => o.value)
const maritalValues: string[] = MARITAL_STATUS_OPTIONS.map((o) => o.value)
const yesNoValues: string[] = YES_NO_OPTIONS.map((o) => o.value)

const photoFileSchema = z
  .instanceof(File, { message: 'الرجاء رفع صورة شخصية للطالب' })
  .refine((file) => PHOTO_ALLOWED_TYPES.includes(file.type), {
    message: 'نوع الملف غير مدعوم. يرجى اختيار صورة بصيغة JPG أو JPEG أو PNG',
  })
  .refine((file) => file.size <= PHOTO_MAX_BYTES, {
    message: 'حجم الملف كبير جداً. يرجى اختيار صورة أقل من 1 ميجابايت',
  })
  .refine((file) => validatePhotoAspectRatio(file), {
    message: PHOTO_ASPECT_ERROR_MESSAGE,
  })

export function createParentsFollowupSchema(hasExistingPhoto: boolean) {
  return z.object({
    studentName: z.string().min(1, 'الرجاء إدخال اسم الطالب'),
    birthdate: z.string().min(1, 'الرجاء إدخال تاريخ الميلاد'),
    studentGender: z
      .string()
      .min(1, 'الرجاء أختيار الجنس')
      .refine((v) => genderValues.includes(v), 'الرجاء أختيار الجنس'),
    address: z.string().min(1, 'الرجاء ادخال العنوان'),
    fatherName: z.string().min(1, 'الرجاء ادخال اسم ولي الامر'),
    fatherPhone: z.string().min(1, 'الرجاء ادخال رقم ولي الامر'),
    maritalStatus: z
      .string()
      .min(1, 'الرجاء أختيار الحالة الأجتماعية')
      .refine((v) => maritalValues.includes(v), 'الرجاء أختيار الحالة الأجتماعية'),
    healthCondition: z
      .string()
      .min(1, 'الرجاء اختيار الحالة الصحية والتعليمية')
      .refine((v) => yesNoValues.includes(v), 'الرجاء اختيار الحالة الصحية والتعليمية'),
    healthDetails: z.string().optional(),
    learningDifficulties: z
      .string()
      .min(1, 'الرجاء أختيار هل يعاني الطالب من صعوبات تعليمية أو سلوكية')
      .refine(
        (v) => yesNoValues.includes(v),
        'الرجاء اختيار هل يعاني الطالب من صعوبات تعليمية أو سلوكية',
      ),
    learningDifficultiesNotes: z.string().optional(),
    photoFile: hasExistingPhoto ? photoFileSchema.optional() : photoFileSchema,
  })
}

export type ParentsFollowupFormValues = z.infer<ReturnType<typeof createParentsFollowupSchema>>

export function toBirthdateInputValue(iso: string | null | undefined): string {
  if (!iso) return ''
  const date = new Date(iso)
  if (Number.isNaN(date.getTime())) return ''
  return date.toISOString().slice(0, 10)
}
