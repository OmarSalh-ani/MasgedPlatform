import { useMutation, useQuery } from '@tanstack/react-query'
import { downloadBlob } from '@/lib/download'
import {
  exportTeachersAttendance,
  getTeachersAttendanceFilterOptions,
  getTeachersAttendanceList,
} from '@/services/teachersAttendanceService'
import type { TeachersAttendanceFilters } from '@/types/teachersAttendance'

export function useTeachersAttendanceFilterOptions() {
  return useQuery({
    queryKey: ['teachers-attendance', 'filter-options'],
    queryFn: getTeachersAttendanceFilterOptions,
  })
}

export function useTeachersAttendance(filters: TeachersAttendanceFilters | null) {
  const listQuery = useQuery({
    queryKey: [
      'teachers-attendance',
      filters?.fromDate,
      filters?.toDate,
      filters?.teacherId,
    ],
    queryFn: () => getTeachersAttendanceList(filters!),
    enabled: Boolean(filters?.fromDate && filters?.toDate),
  })

  const exportMutation = useMutation({
    mutationFn: () => {
      if (!filters) throw new Error('Filters required')
      return exportTeachersAttendance(filters)
    },
    onSuccess: (blob) => {
      downloadBlob(blob, `حضور_المعلمين_${new Date().toISOString().slice(0, 10)}.xlsx`)
    },
  })

  return { listQuery, exportMutation }
}
