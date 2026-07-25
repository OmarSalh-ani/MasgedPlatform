import api from '@/lib/axios'
import type { Competition, CompetitionListItem, SaveCompetitionPayload } from '@/types/competition'
import type { ApiResponse, PagedResult } from '@/types/api'

export async function getCompetitions(): Promise<CompetitionListItem[]> {
  const { data } = await api.get<PagedResult<CompetitionListItem>>('/admincompetition', {
    params: { pageNumber: 1, pageSize: 0 },
  })
  return data.items
}

function toFormData(payload: SaveCompetitionPayload): FormData {
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

export async function getCompetition(id: number): Promise<Competition> {
  const { data } = await api.get<ApiResponse<Competition>>(`/admincompetition/${id}`)
  return data.data
}

export async function getNextSortOrder(): Promise<number> {
  const { data } = await api.get<ApiResponse<number>>('/admincompetition/next-sort-order')
  return data.data
}

export async function createCompetition(payload: SaveCompetitionPayload): Promise<Competition> {
  const { data } = await api.post<ApiResponse<Competition>>(
    '/admincompetition',
    toFormData(payload),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.data
}

export async function updateCompetition(
  id: number,
  payload: SaveCompetitionPayload,
): Promise<Competition> {
  const { data } = await api.put<ApiResponse<Competition>>(
    `/admincompetition/${id}`,
    toFormData(payload),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.data
}

export async function deleteCompetition(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/admincompetition/${id}`)
  return data.data
}
