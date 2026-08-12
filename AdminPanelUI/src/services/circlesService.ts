import api from '@/lib/axios'
import type { ApiResponse, PagedResult } from '@/types/api'
import type {
  CircleDetail,
  CircleListItem,
  CircleTeacherOption,
  SaveCirclePayload,
} from '@/types/circle'

export async function getCircles(teacherId?: number): Promise<CircleListItem[]> {
  const { data } = await api.get<PagedResult<CircleListItem>>('/adminqurancircles', {
    params: {
      pageNumber: 1,
      pageSize: 0,
      ...(teacherId !== undefined ? { teacher: teacherId } : {}),
    },
  })
  return data.items
}

export async function getCircle(id: number): Promise<CircleDetail> {
  const { data } = await api.get<ApiResponse<CircleDetail>>(`/adminqurancircle/${id}`)
  return data.data
}

export async function getCircleTeachers(): Promise<CircleTeacherOption[]> {
  const { data } = await api.get<ApiResponse<CircleTeacherOption[]>>('/adminqurancircle/teachers')
  return data.data
}

export async function createCircle(payload: SaveCirclePayload): Promise<CircleDetail> {
  const { data } = await api.post<ApiResponse<CircleDetail>>('/adminqurancircle', payload)
  return data.data
}

export async function updateCircle(id: number, payload: SaveCirclePayload): Promise<CircleDetail> {
  const { data } = await api.put<ApiResponse<CircleDetail>>(`/adminqurancircle/${id}`, payload)
  return data.data
}

export async function deleteCircle(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/adminqurancircle/${id}`)
  return data.data
}

export async function deleteCirclePlans(circleIds: number[]): Promise<boolean> {
  const { data } = await api.post<ApiResponse<boolean>>('/adminqurancircle/delete-plans', {
    circleIds,
  })
  return data.data
}

export async function exportCirclesExcel(): Promise<Blob> {
  const { data } = await api.get<Blob>('/adminqurancircles/export/excel', {
    responseType: 'blob',
  })
  return data
}
