import api from '@/lib/axios'
import type { SocialLinkListItem } from '@/types/socialLink'
import type { ApiResponse, PagedResult } from '@/types/api'

export async function getSocialLinks(): Promise<SocialLinkListItem[]> {
  const { data } = await api.get<PagedResult<SocialLinkListItem>>('/adminsociallinks', {
    params: { pageNumber: 1, pageSize: 0 },
  })
  return data.items
}

export async function deleteSocialLinkFromList(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/adminsociallinks/${id}`)
  return data.data
}
