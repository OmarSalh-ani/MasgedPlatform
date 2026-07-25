export interface CurrentStudentPlanListItem {
  id: number
  studentId: number
  studentName: string
  planName: string
  fromDate: string
  toDate: string
  createdAt: string | null
  totalDays: number
  elapsedDays: number
  remainingDays: number
  circleName: string
}

export const CURRENT_STUDENT_PLAN_PAGE_SIZE = 20

export const CURRENT_STUDENT_PLAN_PAGE_SIZE_OPTIONS = [10, 20, 50, 100, 200, 500, 1000] as const

export const CURRENT_STUDENT_PLAN_STUDENT_LOOKUP_PAGE_SIZE = 20

export interface CurrentStudentPlanFilters {
  studentId: string
  pageNumber: number
  pageSize: number
}

export interface CurrentStudentPlanStudentLookup {
  id: number
  name: string
  label: string
}

export interface CurrentStudentPlanStudentLookupFilters {
  search?: string
  pageNumber: number
  pageSize: number
}

export function buildCurrentStudentPlanFilters(
  studentId: string,
  pageNumber: number,
  pageSize: number,
): CurrentStudentPlanFilters {
  return {
    studentId,
    pageNumber,
    pageSize,
  }
}

export function formatPlanDate(value: string): string {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleDateString('en-CA')
}

export function formatPlanCreatedAt(value: string | null): string {
  if (!value) return '—'
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleString('en-GB', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    hour12: true,
  })
}
