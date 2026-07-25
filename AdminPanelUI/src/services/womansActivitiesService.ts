import api from '@/lib/axios'
import type { ApiResponse, PagedResult } from '@/types/api'
import type {
  SaveWomanActivityPayload,
  WomanActivity,
  WomanActivityListItem,
} from '@/types/womansActivity'

export async function getWomansActivities(): Promise<WomanActivityListItem[]> {
  const { data } = await api.get<PagedResult<WomanActivityListItem>>('/adminwomansactivities', {
    params: { pageNumber: 1, pageSize: 0 },
  })
  return data.items
}

export async function createWomansActivity(
  payload: SaveWomanActivityPayload,
): Promise<WomanActivity> {
  const { data } = await api.post<ApiResponse<WomanActivity>>('/adminwomansactivity', payload)
  return data.data
}

export async function updateWomansActivity(
  id: number,
  payload: SaveWomanActivityPayload,
): Promise<WomanActivity> {
  const { data } = await api.put<ApiResponse<WomanActivity>>(
    `/adminwomansactivity/${id}`,
    payload,
  )
  return data.data
}

export async function deleteWomansActivity(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/adminwomansactivity/${id}`)
  return data.data
}

export async function exportWomansActivitiesExcel(): Promise<Blob> {
  const { data } = await api.get<Blob>('/adminwomansactivities/export/excel', {
    responseType: 'blob',
  })
  return data
}
