import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type {
  TeachersAttendanceFilterOptions,
  TeachersAttendanceFilters,
  TeachersAttendanceListResponse,
} from '@/types/teachersAttendance'

export async function getTeachersAttendanceFilterOptions(): Promise<TeachersAttendanceFilterOptions> {
  const { data } = await api.get<ApiResponse<TeachersAttendanceFilterOptions>>(
    '/adminteachersattendance/filter-options',
  )
  return data.data
}

export async function getTeachersAttendanceList(
  filters: TeachersAttendanceFilters,
): Promise<TeachersAttendanceListResponse> {
  const teacherId = filters.teacherId && filters.teacherId !== '0'
    ? Number(filters.teacherId)
    : undefined

  const { data } = await api.get<ApiResponse<TeachersAttendanceListResponse>>(
    '/adminteachersattendance',
    {
      params: {
        fromDate: filters.fromDate,
        toDate: filters.toDate,
        teacherId,
      },
    },
  )
  return data.data
}

export async function exportTeachersAttendance(
  filters: TeachersAttendanceFilters,
): Promise<Blob> {
  const teacherId = filters.teacherId && filters.teacherId !== '0'
    ? Number(filters.teacherId)
    : undefined

  const { data } = await api.get<Blob>('/adminteachersattendance/export/excel', {
    params: {
      fromDate: filters.fromDate,
      toDate: filters.toDate,
      teacherId,
    },
    responseType: 'blob',
  })
  return data
}
