import { z } from 'zod'
import {
  CIRCLE_VISIT_RATING_CRITERIA,
  CIRCLE_VISIT_RATING_VALUES,
} from '@/types/circleVisitRating'

const ratingItemSchema = z.object({
  sequence: z.number().int().min(1).max(9),
  criterion: z.string().min(1),
  rating: z
    .string()
    .min(1, 'يرجى اختيار التقييم')
    .refine((v) => (CIRCLE_VISIT_RATING_VALUES as readonly string[]).includes(v), {
      message: 'قيمة التقييم غير صالحة',
    }),
  notes: z.string().max(1000).optional().or(z.literal('')),
})

export const circleRatingFormSchema = z.object({
  teacherId: z.string().min(1, 'يرجى اختيار المعلم'),
  quranCircleId: z.string().min(1, 'يرجى اختيار الحلقة'),
  visitDate: z.string().min(1, 'يرجى إدخال تاريخ الزيارة'),
  visitTime: z.string().min(1, 'يرجى إدخال وقت الزيارة'),
  items: z
    .array(ratingItemSchema)
    .length(CIRCLE_VISIT_RATING_CRITERIA.length, 'يجب تعبئة جميع عناصر التقييم'),
})

export type CircleRatingFormValues = z.infer<typeof circleRatingFormSchema>

export function buildDefaultCircleRatingFormValues(): CircleRatingFormValues {
  const now = new Date()
  const date = now.toISOString().slice(0, 10)
  const time = `${String(now.getHours()).padStart(2, '0')}:${String(now.getMinutes()).padStart(2, '0')}`

  return {
    teacherId: '',
    quranCircleId: '',
    visitDate: date,
    visitTime: time,
    items: CIRCLE_VISIT_RATING_CRITERIA.map((criterion, index) => ({
      sequence: index + 1,
      criterion,
      rating: '',
      notes: '',
    })),
  }
}
