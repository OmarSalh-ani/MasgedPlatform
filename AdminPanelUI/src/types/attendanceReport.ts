export type AttendanceFilter = 'all' | 'present' | 'departed' | 'absent'

export interface AttendanceReportLookup {
  id: number
  name: string
}

export interface AttendanceReportFilterOptions {
  circles: AttendanceReportLookup[]
  teachers: AttendanceReportLookup[]
}

export interface AttendanceReportRow {
  studentId: number
  studentName: string
  circleName: string
  teacherName: string
  fatherPhone: string | null
  date: string
  dayOfWeek: string
  isPresent: boolean
  isDeparted: boolean
  departureTime: string | null
  status: string
  color: 'red' | 'yellow' | 'green' | string
}

export interface AttendanceReportSummary {
  totalStudents: number
  totalDays: number
  totalAttendance: number
  totalDeparture: number
}

export interface AttendanceReportListResponse {
  items: AttendanceReportRow[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
  summary: AttendanceReportSummary
}

export interface AttendanceReportFilters {
  fromDate: string
  toDate: string
  circleId?: number
  teacherId?: number
  attendanceFilter: AttendanceFilter
  pageNumber: number
  pageSize: number
}

export interface SaveDepartureResult {
  message: string
  savedCount: number
  skippedCount: number
  errorCount: number
}

export interface SelectedAttendanceRow {
  studentId: number
  date: string
}

export function getRowKey(row: SelectedAttendanceRow): string {
  return `${row.studentId}|${row.date}`
}

export function getDefaultDateRange(): { fromDate: string; toDate: string } {
  const today = new Date()
  const from = new Date(today)
  from.setDate(from.getDate() - 30)
  return {
    fromDate: from.toISOString().split('T')[0],
    toDate: today.toISOString().split('T')[0],
  }
}

export function countDaysInclusive(fromDate: string, toDate: string): number {
  const from = new Date(fromDate)
  const to = new Date(toDate)
  return Math.ceil((to.getTime() - from.getTime()) / (1000 * 60 * 60 * 24)) + 1
}

export function formatReportDate(value: string): string {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleDateString('ar-KW')
}

export function getStatusRowClass(color: string): string {
  if (color === 'red') return 'bg-red-50'
  if (color === 'yellow') return 'bg-amber-50'
  if (color === 'green') return 'bg-green-50'
  if (color === 'gray') return 'bg-slate-50'
  return ''
}
