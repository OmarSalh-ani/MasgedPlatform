import { useMutation, useQuery } from '@tanstack/react-query'
import {
  exportTestsReport,
  getTestsReport,
  getTestsReportFilterOptions,
} from '@/services/testsReportService'
import type { TestsReportFilters } from '@/types/testsReport'

export function useTestsReportFilterOptions() {
  return useQuery({
    queryKey: ['tests-report', 'filter-options'],
    queryFn: getTestsReportFilterOptions,
  })
}

export function useTestsReport(filters: TestsReportFilters | null) {
  const reportQuery = useQuery({
    queryKey: ['tests-report', filters],
    queryFn: () => getTestsReport(filters!),
    enabled: Boolean(filters?.fromDate && filters?.toDate),
  })

  const exportMutation = useMutation({
    mutationFn: () => {
      if (!filters) throw new Error('Filters required')
      const { pageNumber: _pageNumber, pageSize: _pageSize, ...exportFilters } = filters
      return exportTestsReport(exportFilters)
    },
    onSuccess: (blob) => {
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = `تقرير_الاختبارات_${new Date().toISOString().slice(0, 10)}.xlsx`
      link.click()
      window.URL.revokeObjectURL(url)
    },
  })

  return { reportQuery, exportMutation }
}
