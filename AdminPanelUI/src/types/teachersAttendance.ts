import type { SearchableDropdownOption } from '@/components/shared/SearchableDropdown'

export interface TeachersAttendanceTeacherOption {
  id: number
  name: string
}
export interface TeachersAttendanceFilterOptions {
  teachers: TeachersAttendanceTeacherOption[]
}

export interface TeachersAttendanceRow {
  teacherName: string
  attendanceDateTime: string
  departureDateTime: string | null
  hoursWorked: number
  status: string
  statusClass: string
}

export interface TeachersAttendanceListResponse {
  fromDate: string
  toDate: string
  items: TeachersAttendanceRow[]
}

export interface TeachersAttendanceFilters {
  fromDate: string
  toDate: string
  teacherId: string
}

export function getDefaultTeachersAttendanceFilters(): TeachersAttendanceFilters {
  const to = new Date()
  const from = new Date()
  from.setDate(from.getDate() - 30)
  return {
    fromDate: formatDateInput(from),
    toDate: formatDateInput(to),
    teacherId: '0',
  }
}

export function formatDateInput(date: Date): string {
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

export function toTeachersAttendanceDropdownOptions(
  teachers: TeachersAttendanceTeacherOption[] | undefined,
): SearchableDropdownOption[] {
  return [
    { value: '0', label: 'جميع المعلمين' },
    ...(teachers ?? []).map((teacher) => ({
      value: String(teacher.id),
      label: teacher.name,
    })),
  ]
}

export function formatHoursWorked(hoursWorked: number): string {
  const totalMinutes = Math.round(hoursWorked * 60)
  const hours = Math.floor(totalMinutes / 60)
  const minutes = totalMinutes % 60

  if (hours === 0 && minutes === 0) {
    return '0 دقيقة'
  }

  const parts: string[] = []
  if (hours > 0) {
    parts.push(`${hours} ساعة`)
  }
  if (minutes > 0) {
    parts.push(`${minutes} دقيقة`)
  }

  return parts.join(' و ')
}
