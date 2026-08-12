export const CIRCLE_VISIT_RATING_CRITERIA = [
  'حضور المحفظ',
  'انضباط وقت الحلقة',
  'عدد الطلاب الحاضرين',
  'مستوى حفظ الطلاب',
  'المراجعة اليومية',
  'السلوك والانضباط',
  'سجل الحضور',
  'متابعة أولياء الامور',
  'البيئة التعليمية',
] as const

export const CIRCLE_VISIT_RATING_VALUES = [
  'ممتاز',
  'جيد جدا',
  'جيد',
  'يحتاج متابعة',
] as const

export type CircleVisitRatingValue = (typeof CIRCLE_VISIT_RATING_VALUES)[number]

export interface CircleVisitRatingTeacherOption {
  id: number
  name: string
}

export interface CircleVisitRatingCircleOption {
  id: number
  name: string
}

export interface CircleVisitRatingListItem {
  id: number
  teacherName: string
  circleName: string
  visitDate: string
  visitTime: string
  visitNumberInMonth: number
  createdByName: string
  createdAt: string
}

export interface CircleVisitRatingItem {
  sequence: number
  criterion: string
  rating: string
  notes?: string | null
}

export interface CircleVisitRatingDetail {
  id: number
  teacherId: number
  teacherName: string
  quranCircleId: number
  circleName: string
  visitDate: string
  visitTime: string
  visitNumberInMonth: number
  createdByName: string
  createdAt: string
  items: CircleVisitRatingItem[]
}

export interface CreateCircleVisitRatingPayload {
  teacherId: number
  quranCircleId: number
  visitDate: string
  visitTime: string
  items: CircleVisitRatingItem[]
}

export function toTeacherDropdownOptions(
  teachers: CircleVisitRatingTeacherOption[],
): { value: string; label: string }[] {
  return teachers.map((t) => ({ value: String(t.id), label: t.name }))
}

export function toCircleDropdownOptions(
  circles: CircleVisitRatingCircleOption[],
): { value: string; label: string }[] {
  return circles.map((c) => ({ value: String(c.id), label: c.name }))
}

export function formatVisitDate(value: string): string {
  const d = new Date(value)
  if (Number.isNaN(d.getTime())) return value
  return d.toLocaleDateString('ar-KW')
}
