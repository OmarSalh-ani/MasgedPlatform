import api from '@/lib/axios'
import type { ContactInfoListItem } from '@/types/contactInfo'
import type { ApiResponse, PagedResult } from '@/types/api'

export async function getContactInfos(): Promise<ContactInfoListItem[]> {
  const { data } = await api.get<PagedResult<ContactInfoListItem>>('/admincontactinfos', {
    params: { pageNumber: 1, pageSize: 0 },
  })
  return data.items
}

export async function deleteContactInfoFromList(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/admincontactinfos/${id}`)
  return data.data
}
