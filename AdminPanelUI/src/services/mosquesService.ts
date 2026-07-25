import api from '@/lib/axios'
import type { MosqueListItem } from '@/types/mosque'
import type { ApiResponse, PagedResult } from '@/types/api'

export async function getMosques(): Promise<MosqueListItem[]> {
  const { data } = await api.get<PagedResult<MosqueListItem>>('/adminmosques', {
    params: { pageNumber: 1, pageSize: 0 },
  })
  return data.items
}

export async function deleteMosqueFromList(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/adminmosques/${id}`)
  return data.data
}
