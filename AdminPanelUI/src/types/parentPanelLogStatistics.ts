export interface ParentPanelLogStatisticsSummary {
  parentsOpened: number
  parentsNotOpened: number
  totalLogEntries: number
  percentage: string
}

export interface ParentPanelLogEntry {
  parentMobile: string
  studentName: string
  studentId: number
  accessDate: string
  accessTime: string
}

export interface ParentPanelLogStatisticsResponse {
  fromDate: string
  toDate: string
  summary: ParentPanelLogStatisticsSummary
  entries: ParentPanelLogEntry[]
}

export interface ParentPanelLogStatisticsFilters {
  fromDate: string
  toDate: string
}

export function getDefaultDateRange(): ParentPanelLogStatisticsFilters {
  const to = new Date()
  const from = new Date()
  from.setDate(from.getDate() - 30)
  return {
    fromDate: formatDateInput(from),
    toDate: formatDateInput(to),
  }
}

export function formatDateInput(date: Date): string {
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}
