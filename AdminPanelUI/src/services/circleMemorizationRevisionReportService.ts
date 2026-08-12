import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type {
  CircleMemorizationTeacherOption,
  CircleReportExportFormat,
} from '@/types/circleMemorizationRevisionReport'

export async function getCircleMemorizationTeachers(): Promise<
  CircleMemorizationTeacherOption[]
> {
  const { data } = await api.get<ApiResponse<CircleMemorizationTeacherOption[]>>(
    '/admincirclememorizationrevisionreport/teachers',
  )
  return data.data
}

export async function exportCircleMemorizationRevisionReport(params: {
  teacherId: number
  fromDate: string
  toDate: string
  format: CircleReportExportFormat
}): Promise<Blob> {
  const { data } = await api.get<Blob>('/admincirclememorizationrevisionreport/export', {
    params: {
      teacherId: params.teacherId,
      fromDate: params.fromDate,
      toDate: params.toDate,
      format: params.format,
    },
    responseType: 'blob',
  })
  return data
}
