export interface StudentLookupOption {
  id: number
  name: string
}

export interface StudentFormData {
  circles: StudentLookupOption[]
  planLevels: StudentLookupOption[]
  canModify: boolean
  defaultRegistrationDate: string
}

export interface Student {
  id: number
  studentName: string
  fullName: string
  fatherPhone: string
  alternativePhone: string | null
  parentPanelPassword: string | null
  age: number
  studentGender: string
  quranCircleId: number | null
  planLevelId: number | null
  isSpecial: boolean
  createdAt: string | null
}

export interface SaveStudentPayload {
  fullName: string
  fatherPhone: string
  alternativePhone?: string | null
  parentPanelPassword?: string | null
  age?: number | null
  studentGender: string
  quranCircleId?: number | null
  planLevelId?: number | null
  isSpecial: boolean
}

export const STUDENT_GENDER_OPTIONS = [
  { value: 'ذكر', label: 'ذكر' },
  { value: 'أنثى', label: 'أنثى' },
] as const
