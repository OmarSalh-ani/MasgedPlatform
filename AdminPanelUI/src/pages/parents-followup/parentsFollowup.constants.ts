export const GENDER_OPTIONS = [
  { label: 'ذكر', value: 'ذكر' },
  { label: 'أنثى', value: 'أنثى' },
] as const

export const MARITAL_STATUS_OPTIONS = [
  { label: 'متزوج / ة', value: 'متزوج / ة' },
  { label: 'متوفي /ة', value: 'متوفي /ة' },
  { label: 'مطلق / ة', value: 'مطلق / ة' },
  { label: 'أعزب', value: 'أعزب' },
] as const

export const YES_NO_OPTIONS = [
  { label: 'نعم', value: 'نعم' },
  { label: 'لا', value: 'لا' },
] as const

export const PHOTO_MAX_BYTES = 1_048_576
export const PHOTO_ALLOWED_TYPES = ['image/jpeg', 'image/jpg', 'image/png']
