import {
  countDaysInclusive,
  getDefaultDateRange,
  type AttendanceFilter,
  type AttendanceReportFilters,
  type SelectedAttendanceRow,
} from '@/types/attendanceReport'

export function buildAppliedFilters(input: {
  fromDate: string
  toDate: string
  circleId: string
  teacherId: string
  attendanceFilter: AttendanceFilter
  pageNumber: number
  pageSize: number
}): AttendanceReportFilters {
  return {
    fromDate: input.fromDate,
    toDate: input.toDate,
    circleId: input.circleId ? Number(input.circleId) : undefined,
    teacherId: input.teacherId ? Number(input.teacherId) : undefined,
    attendanceFilter: input.attendanceFilter,
    pageNumber: input.pageNumber,
    pageSize: input.pageSize,
  }
}

export function validateReportDates(fromDate: string, toDate: string): string | null {
  if (!fromDate || !toDate) return 'يرجى تحديد تاريخ البداية والنهاية'
  if (fromDate > toDate) return 'تاريخ البداية يجب أن يكون قبل تاريخ النهاية'
  if (countDaysInclusive(fromDate, toDate) > 30) {
    return 'تاريخ الفترة كبير جداً. الحد الأقصى هو 30 يوم. يرجى تقليل فترة التقرير.'
  }
  return null
}

export function validateExportDates(fromDate: string, toDate: string): string | null {
  if (!fromDate || !toDate) return 'يرجى تحديد تاريخ البداية والنهاية'
  if (fromDate > toDate) return 'تاريخ البداية يجب أن يكون قبل تاريخ النهاية'
  if (countDaysInclusive(fromDate, toDate) > 365) {
    return 'تاريخ الفترة كبير جداً. الحد الأقصى للتصدير هو 365 يوم. يرجى تقليل فترة التقرير.'
  }
  return null
}

export function getQuickDateRange(
  range: 'today' | 'yesterday' | 'week' | 'month' | 'lastMonth',
): { fromDate: string; toDate: string } {
  const today = new Date()
  const toDate = today.toISOString().split('T')[0]

  if (range === 'today') return { fromDate: toDate, toDate }
  if (range === 'yesterday') {
    const yesterday = new Date(today)
    yesterday.setDate(yesterday.getDate() - 1)
    const value = yesterday.toISOString().split('T')[0]
    return { fromDate: value, toDate: value }
  }
  if (range === 'week') {
    const from = new Date(today)
    const day = from.getDay()
    from.setDate(from.getDate() - day)
    return { fromDate: from.toISOString().split('T')[0], toDate }
  }
  if (range === 'month') {
    const from = new Date(today.getFullYear(), today.getMonth(), 1)
    return { fromDate: from.toISOString().split('T')[0], toDate }
  }

  const from = new Date(today.getFullYear(), today.getMonth() - 1, 1)
  const to = new Date(today.getFullYear(), today.getMonth(), 0)
  return {
    fromDate: from.toISOString().split('T')[0],
    toDate: to.toISOString().split('T')[0],
  }
}

export function getInitialDatesByType(type: string | null): { fromDate: string; toDate: string } {
  const today = new Date().toISOString().split('T')[0]
  if (type === 'present' || type === 'departed' || type === 'absent') {
    return { fromDate: today, toDate: today }
  }
  return getDefaultDateRange()
}

export function getInitialAttendanceFilter(type: string | null): AttendanceFilter {
  if (type === 'present' || type === 'departed' || type === 'absent') return type
  return 'all'
}

export function parseSelectedRows(selectedRows: Set<string>): SelectedAttendanceRow[] {
  return [...selectedRows].map((key) => {
    const [studentId, date] = key.split('|')
    return { studentId: Number(studentId), date }
  })
}

export function uniqueStudentIdsFromRows(rows: SelectedAttendanceRow[]): number[] {
  return [...new Set(rows.map((row) => row.studentId))]
}
