import api from '@/lib/axios'
import type { ApiResponse, PagedResult } from '@/types/api'
import type {
  SaveTeacherPayload,
  Teacher,
  TeacherCircleOption,
  TeacherListItem,
  TeacherMosqueOption,
} from '@/types/teacher'
export async function getTeachers(): Promise<TeacherListItem[]> {
  const { data } = await api.get<PagedResult<TeacherListItem>>('/adminteacher', {
    params: { pageNumber: 1, pageSize: 0 },
  })
  return data.items
}

export async function deleteTeacher(id: number, fromForm = false): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/adminteacher/${id}`, {
    params: fromForm ? { fromForm: true } : undefined,
  })
  return data.data
}

function toFormData(payload: SaveTeacherPayload): FormData {
  const formData = new FormData()
  formData.append('name', payload.name)
  if (payload.mobile) formData.append('mobile', payload.mobile)
  formData.append('email', payload.email)
  if (payload.password) formData.append('password', payload.password)
  if (payload.baseSalary !== undefined && payload.baseSalary !== null) {
    formData.append('baseSalary', String(payload.baseSalary))
  }
  if (payload.circleId) formData.append('circleId', String(payload.circleId))
  formData.append('isGirlTeacher', String(payload.isGirlTeacher))
  formData.append('usersManage', String(payload.usersManage))
  formData.append('isViewOnly', String(payload.isViewOnly))
  if (payload.removeImage) formData.append('removeImage', 'true')
  if (payload.imageFile) formData.append('image', payload.imageFile)
  if (payload.selectedMosqueIds.length > 0) {
    formData.append('selectedMosqueIds', payload.selectedMosqueIds.join(','))
  }
  if (payload.manualLocations.length > 0) {
    formData.append('manualLocationsJson', JSON.stringify(payload.manualLocations))
  }
  return formData
}

export async function getTeacher(id: number): Promise<Teacher> {
  const { data } = await api.get<ApiResponse<Teacher>>(`/adminteacher/${id}`)
  return data.data
}

export async function getTeacherCircles(forGirls: boolean): Promise<TeacherCircleOption[]> {
  const { data } = await api.get<ApiResponse<TeacherCircleOption[]>>('/adminteacher/circles', {
    params: { forGirls },
  })
  return data.data
}

export async function getTeacherMosques(): Promise<TeacherMosqueOption[]> {
  const { data } = await api.get<ApiResponse<TeacherMosqueOption[]>>('/adminteacher/mosques')
  return data.data
}

export async function createTeacher(payload: SaveTeacherPayload): Promise<Teacher> {
  const { data } = await api.post<ApiResponse<Teacher>>('/adminteacher', toFormData(payload), {
    headers: { 'Content-Type': 'multipart/form-data' },
  })
  return data.data
}

export async function updateTeacher(id: number, payload: SaveTeacherPayload): Promise<Teacher> {
  const { data } = await api.put<ApiResponse<Teacher>>(
    `/adminteacher/${id}`,
    toFormData(payload),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.data
}
export async function exportTeachersExcel(): Promise<void> {
  const response = await api.get<Blob>('/adminteacher/export/excel', {
    responseType: 'blob',
  })

  const disposition = response.headers['content-disposition'] as string | undefined
  const fileName =
    disposition?.match(/filename="?([^";]+)"?/)?.[1] ??
    `Teachers_${new Date().toISOString().replace(/[-:T]/g, '').slice(0, 15)}.xlsx`

  const url = window.URL.createObjectURL(response.data)
  const link = document.createElement('a')
  link.href = url
  link.download = fileName
  link.click()
  window.URL.revokeObjectURL(url)
}
