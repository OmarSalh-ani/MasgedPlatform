export interface TeacherSalaryListItem {
  id: number
  teacherName: string
  teacherId: number
  month: number
  year: number
  daysAttended: number
  totalHours: number
  baseSalary: number | null
  calculatedSalary: number
  dayOffDate: string | null
  notes: string
  createdAt: string
}

export interface TeacherSalary {
  id: number
  teacherId: number
  teacherName: string
  month: number
  year: number
  baseSalary: number | null
  daysAttended: number
  totalHours: number
  dayOffDate: string | null
  calculatedSalary: number
  status: string
  notes: string | null
  createdAt: string
}

export interface TeacherSalaryOption {
  label: string
  value: number
}

export interface TeacherSalaryFilterOptions {
  months: TeacherSalaryOption[]
  years: TeacherSalaryOption[]
  teachers: TeacherSalaryOption[]
  defaultMonth: number
  defaultYear: number
}

export interface TeacherSalaryFormTeacher {
  id: number
  name: string
  baseSalary: number | null
}

export interface SaveTeacherSalaryPayload {
  teacherId: number
  month: number
  year: number
  baseSalary: number
  daysAttended: number
  totalHours: number
  calculatedSalary: number
  notes?: string
  dayOffDate?: string | null
}

export interface DailyAttendanceDetail {
  date: string
  dateFormatted: string
  attendanceTime: string
  departureTime: string
  hours: number
  isValid: boolean
}

export interface SalaryCalculationResult {
  daysAttended: number
  totalHours: number
  baseSalary: number
  calculatedSalary: number
  deduction: number
  requiredDays: number
  dailyDetails: DailyAttendanceDetail[]
}

export interface AutoCalculateMonthResult {
  successCount: number
  errorCount: number
  errors: string[]
}

export interface PaySelectedSalariesResult {
  expensesCreated: number
  message: string
  errors: string[]
}

export interface TeacherSalaryReportSummary {
  totalTeachers: number
  totalSalary: number
  averageSalary: number
  fullAttendance: number
  withDeductions: number
}

export interface TeacherSalaryReportItem {
  id: number
  teacherName: string
  daysAttended: number
  totalHours: number
  baseSalary: number | null
  calculatedSalary: number
  deduction: number
}

export interface TeacherSalaryReport {
  summary: TeacherSalaryReportSummary
  items: TeacherSalaryReportItem[]
}

export const REQUIRED_ATTENDANCE_DAYS = 16

export function formatTeacherSalaryDate(value: string): string {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  const d = String(date.getDate()).padStart(2, '0')
  const m = String(date.getMonth() + 1).padStart(2, '0')
  const y = date.getFullYear()
  return `${d}/${m}/${y}`
}

export function getMonthName(month: number): string {
  const date = new Date(2000, month - 1, 1)
  return date.toLocaleString('ar', { month: 'long' })
}

export function formatCurrency(value: number | null | undefined): string {
  return `${(value ?? 0).toFixed(2)} د.ك`
}
