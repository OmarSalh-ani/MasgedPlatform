import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type {
  AttendanceReportFilterOptions,
  AttendanceReportFilters,
  AttendanceReportListResponse,
  SaveDepartureResult,
  SelectedAttendanceRow,
} from '@/types/attendanceReport'

export async function getAttendanceReportFilterOptions(): Promise<AttendanceReportFilterOptions> {
  const { data } = await api.get<ApiResponse<AttendanceReportFilterOptions>>(
    '/adminattendancereport/filter-options',
  )
  return data.data
}

export async function getAttendanceReport(
  filters: AttendanceReportFilters,
): Promise<AttendanceReportListResponse> {
  const { data } = await api.get<ApiResponse<AttendanceReportListResponse>>(
    '/adminattendancereport',
    {
      params: {
        fromDate: filters.fromDate,
        toDate: filters.toDate,
        circleId: filters.circleId,
        teacherId: filters.teacherId,
        attendanceFilter: filters.attendanceFilter,
        pageNumber: filters.pageNumber,
        pageSize: filters.pageSize,
      },
    },
  )
  return data.data
}

export async function exportAttendanceReport(
  filters: Omit<AttendanceReportFilters, 'pageNumber' | 'pageSize'>,
): Promise<Blob> {
  const { data } = await api.get<Blob>('/adminattendancereport/export', {
    params: {
      fromDate: filters.fromDate,
      toDate: filters.toDate,
      circleId: filters.circleId,
      teacherId: filters.teacherId,
      attendanceFilter: filters.attendanceFilter,
    },
    responseType: 'blob',
  })
  return data
}

export async function sendAttendanceWhatsapp(payload: {
  studentIds: number[]
  message: string
  image?: File | null
}): Promise<string> {
  const formData = new FormData()
  formData.append('studentIds', payload.studentIds.join(','))
  formData.append('message', payload.message)
  if (payload.image) formData.append('image', payload.image)

  const { data } = await api.post<ApiResponse<string>>(
    '/adminattendancereport/whatsapp',
    formData,
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.message
}

export async function saveAttendanceDepartures(
  items: SelectedAttendanceRow[],
): Promise<SaveDepartureResult> {
  const { data } = await api.post<ApiResponse<SaveDepartureResult>>(
    '/adminattendancereport/departures',
    items,
  )
  return data.data
}
