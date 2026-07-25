export interface StudentPlanSurahOption {
  id: number
  nameAr: string
}

export interface StudentPlanAyah {
  ayahNumber: number
}

export interface StudentPlanCircleOption {
  id: number
  name: string
}

export interface StudentPlanStudentOption {
  id: number
  name: string
  quranCircleId: number | null
}

export interface StudentPlanFormData {
  circles: StudentPlanCircleOption[]
  students: StudentPlanStudentOption[]
  surahs: StudentPlanSurahOption[]
  memorizationLevels: string[]
  planTypes: string[]
  canModify: boolean
}

export interface StudentPlanListOption {
  id: number
  display: string
}

export interface StudentPlanItem {
  key: string
  planType: string
  memorizationLevel: string
  surahId: number
  surahName: string
  fromAyahNumber: number
  toAyahNumber: number
  planDateFormatted: string
}

export interface StudentPlanHeader {
  memorizationLevel: string
  planStartDate: string
  planEndDate: string
}

export interface StudentPlanDetail {
  studentId: number
  studentName: string
  planId: number
  planName: string
  plans: StudentPlanListOption[]
  header: StudentPlanHeader
  items: StudentPlanItem[]
  canModify: boolean
}

export interface StudentPlanResolve {
  studentId: number
  studentName: string
  planId: number | null
  shouldCreateNew: boolean
}

export interface StudentPlanEditPrefill {
  memorizationLevel: string
  planStartDate: string
  planEndDate: string
  surahId: number
  fromAyahNumber: number
  toAyahNumber: number
  planType: string
  planId: number | null
}

export interface PlanRowInput {
  surahId: number
  fromAyahNumber: number
  toAyahNumber: number
  planType: string
}

export interface EditPlanRowInput {
  key: string
  surahId: number
  fromAyahNumber: number
  toAyahNumber: number
  planType: string
}

export interface SaveStudentPlanPayload {
  studentIds: number[]
  studentId?: number
  planId?: number
  memorizationLevel: string
  planStartDate: string
  planEndDate: string
  editMode: boolean
  editRows: EditPlanRowInput[]
  newRows: PlanRowInput[]
}

export interface CreateStudentPlanPayload {
  studentId: number
  name: string
  fromDate: string
  toDate: string
}

export interface UpdateStudentPlanItemPayload {
  editKey: string
  memorizationLevel: string
  planStartDate: string
  planEndDate: string
  surahId: number
  fromAyahNumber: number
  toAyahNumber: number
  planType: string
}

export function calcPlanDays(start: string, end: string): number | null {
  if (!start || !end) return null
  const d1 = new Date(start)
  const d2 = new Date(end)
  if (Number.isNaN(d1.getTime()) || Number.isNaN(d2.getTime())) return null
  const ms = d2.getTime() - d1.getTime()
  const days = Math.round(ms / (1000 * 60 * 60 * 24))
  return days >= 0 ? days : 0
}

export function todayIsoDate(): string {
  return new Date().toLocaleDateString('en-CA')
}
