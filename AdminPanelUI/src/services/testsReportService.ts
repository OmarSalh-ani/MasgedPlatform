import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type {
  TestsReportFilterOptions,
  TestsReportFilters,
  TestsReportListResponse,
} from '@/types/testsReport'

export async function getTestsReportFilterOptions(): Promise<TestsReportFilterOptions> {
  const { data } = await api.get<ApiResponse<TestsReportFilterOptions>>(
    '/admintests/filter-options',
  )
  return data.data
}

export async function getTestsReport(
  filters: TestsReportFilters,
): Promise<TestsReportListResponse> {
  const { data } = await api.get<ApiResponse<TestsReportListResponse>>('/admintests', {
    params: {
      fromDate: filters.fromDate,
      toDate: filters.toDate,
      circleId: filters.circleId || undefined,
      pageNumber: filters.pageNumber,
      pageSize: filters.pageSize,
    },
  })
  return data.data
}

export async function exportTestsReport(
  filters: Omit<TestsReportFilters, 'pageNumber' | 'pageSize'>,
): Promise<Blob> {
  const { data } = await api.get<Blob>('/admintests/export', {
    params: {
      fromDate: filters.fromDate,
      toDate: filters.toDate,
      circleId: filters.circleId || undefined,
    },
    responseType: 'blob',
  })
  return data
}
