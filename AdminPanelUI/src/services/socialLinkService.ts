import api from '@/lib/axios'
import type { SocialLink, SaveSocialLinkPayload } from '@/types/socialLink'
import type { ApiResponse } from '@/types/api'

export async function getSocialLink(id: number): Promise<SocialLink> {
  const { data } = await api.get<ApiResponse<SocialLink>>(`/adminsociallink/${id}`)
  return data.data
}

export async function getNextSocialLinkSortOrder(): Promise<number> {
  const { data } = await api.get<ApiResponse<number>>('/adminsociallink/next-sort-order')
  return data.data
}

export async function createSocialLink(payload: SaveSocialLinkPayload): Promise<SocialLink> {
  const { data } = await api.post<ApiResponse<SocialLink>>('/adminsociallink', payload)
  return data.data
}

export async function updateSocialLink(
  id: number,
  payload: SaveSocialLinkPayload,
): Promise<SocialLink> {
  const { data } = await api.put<ApiResponse<SocialLink>>(`/adminsociallink/${id}`, payload)
  return data.data
}

export async function deleteSocialLink(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/adminsociallink/${id}`)
  return data.data
}
