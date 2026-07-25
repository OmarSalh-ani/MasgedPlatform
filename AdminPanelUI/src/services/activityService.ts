import api from '@/lib/axios'
import type { Activity, SaveActivityPayload } from '@/types/activity'
import type { ApiResponse } from '@/types/api'

function toFormData(payload: SaveActivityPayload): FormData {
  const formData = new FormData()
  formData.append('title', payload.title)
  if (payload.description) {
    formData.append('description', payload.description)
  }
  formData.append('sortOrder', String(payload.sortOrder))
  if (payload.imageFile) {
    formData.append('image', payload.imageFile)
  }
  return formData
}

export async function getActivity(id: number): Promise<Activity> {
  const { data } = await api.get<ApiResponse<Activity>>(`/adminactivity/${id}`)
  return data.data
}

export async function getNextSortOrder(): Promise<number> {
  const { data } = await api.get<ApiResponse<number>>('/adminactivity/next-sort-order')
  return data.data
}

export async function createActivity(payload: SaveActivityPayload): Promise<Activity> {
  const { data } = await api.post<ApiResponse<Activity>>(
    '/adminactivity',
    toFormData(payload),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.data
}

export async function updateActivity(
  id: number,
  payload: SaveActivityPayload,
): Promise<Activity> {
  const { data } = await api.put<ApiResponse<Activity>>(
    `/adminactivity/${id}`,
    toFormData(payload),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.data
}

export async function deleteActivity(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/adminactivity/${id}`)
  return data.data
}
