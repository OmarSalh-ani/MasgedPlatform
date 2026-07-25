export interface TestsReportCircleOption {
  id: number
  name: string
}

export interface TestsReportFilterOptions {
  circles: TestsReportCircleOption[]
}

export interface TestsReportRow {
  studentId: number
  studentName: string
  parentPhone: string
  teacherName: string
  circleName: string
  programType: string
  testFrom: string
  testTo: string
  testDate: string
  finalResults: number
  notes: string
  testType: string
}

export interface TestsReportListResponse {
  items: TestsReportRow[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
}

export interface TestsReportFilters {
  fromDate: string
  toDate: string
  circleId: string
  pageNumber: number
  pageSize: number
}

export const TESTS_REPORT_PAGE_SIZE = 20
