import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  exportAttendanceReport,
  getAttendanceReport,
  getAttendanceReportFilterOptions,
  saveAttendanceDepartures,
  sendAttendanceWhatsapp,
} from '@/services/attendanceReportService'
import type { AttendanceReportFilters } from '@/types/attendanceReport'

export function useAttendanceReportFilterOptions() {
  return useQuery({
    queryKey: ['attendance-report', 'filter-options'],
    queryFn: getAttendanceReportFilterOptions,
  })
}

export function useAttendanceReport(filters: AttendanceReportFilters | null) {
  const queryClient = useQueryClient()

  const reportQuery = useQuery({
    queryKey: ['attendance-report', filters],
    queryFn: () => getAttendanceReport(filters!),
    enabled: Boolean(filters?.fromDate && filters?.toDate),
  })

  const exportMutation = useMutation({
    mutationFn: () => {
      if (!filters) throw new Error('Filters required')
      const { pageNumber: _pageNumber, pageSize: _pageSize, ...exportFilters } = filters
      return exportAttendanceReport(exportFilters)
    },
    onSuccess: (blob) => {
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = `AttendanceReport_${new Date().toISOString().slice(0, 10)}.xlsx`
      link.click()
      window.URL.revokeObjectURL(url)
    },
  })

  const whatsappMutation = useMutation({
    mutationFn: sendAttendanceWhatsapp,
  })

  const departureMutation = useMutation({
    mutationFn: saveAttendanceDepartures,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['attendance-report'] })
    },
  })

  return { reportQuery, exportMutation, whatsappMutation, departureMutation }
}
