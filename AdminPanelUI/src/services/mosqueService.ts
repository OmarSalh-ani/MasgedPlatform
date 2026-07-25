import api from '@/lib/axios'
import type { Mosque, SaveMosquePayload } from '@/types/mosque'
import type { ApiResponse } from '@/types/api'

function toFormData(payload: SaveMosquePayload): FormData {
  const formData = new FormData()
  formData.append('name', payload.name)
  if (payload.description) {
    formData.append('description', payload.description)
  }
  if (payload.googleMapsUrl) {
    formData.append('googleMapsUrl', payload.googleMapsUrl)
  }
  formData.append('sortOrder', String(payload.sortOrder))
  if (payload.imageFile) {
    formData.append('image', payload.imageFile)
  }
  return formData
}

export async function getMosque(id: number): Promise<Mosque> {
  const { data } = await api.get<ApiResponse<Mosque>>(`/adminmosque/${id}`)
  return data.data
}

export async function getNextMosqueSortOrder(): Promise<number> {
  const { data } = await api.get<ApiResponse<number>>('/adminmosque/next-sort-order')
  return data.data
}

export async function createMosque(payload: SaveMosquePayload): Promise<Mosque> {
  const { data } = await api.post<ApiResponse<Mosque>>(
    '/adminmosque',
    toFormData(payload),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.data
}

export async function updateMosque(id: number, payload: SaveMosquePayload): Promise<Mosque> {
  const { data } = await api.put<ApiResponse<Mosque>>(
    `/adminmosque/${id}`,
    toFormData(payload),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.data
}

export async function deleteMosque(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/adminmosque/${id}`)
  return data.data
}
