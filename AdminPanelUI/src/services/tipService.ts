import api from '@/lib/axios'
import type { Tip, TipListItem, SaveTipPayload } from '@/types/tip'
import type { ApiResponse, PagedResult } from '@/types/api'

export async function getTips(): Promise<TipListItem[]> {
  const { data } = await api.get<PagedResult<TipListItem>>('/admintips', {
    params: { pageNumber: 1, pageSize: 0 },
  })
  return data.items
}

function toFormData(payload: SaveTipPayload): FormData {
  const formData = new FormData()
  formData.append('title', payload.title)
  if (payload.description) {
    formData.append('description', payload.description)
  }
  if (payload.linkUrl) {
    formData.append('linkUrl', payload.linkUrl)
  }
  formData.append('sortOrder', String(payload.sortOrder))
  if (payload.imageFile) {
    formData.append('image', payload.imageFile)
  }
  return formData
}

export async function getTip(id: number): Promise<Tip> {
  const { data } = await api.get<ApiResponse<Tip>>(`/admintips/${id}`)
  return data.data
}

export async function getNextSortOrder(): Promise<number> {
  const { data } = await api.get<ApiResponse<number>>('/admintips/next-sort-order')
  return data.data
}

export async function createTip(payload: SaveTipPayload): Promise<Tip> {
  const { data } = await api.post<ApiResponse<Tip>>(
    '/admintips',
    toFormData(payload),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.data
}

export async function updateTip(id: number, payload: SaveTipPayload): Promise<Tip> {
  const { data } = await api.put<ApiResponse<Tip>>(
    `/admintips/${id}`,
    toFormData(payload),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.data
}

export async function deleteTip(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/admintips/${id}`)
  return data.data
}
