import api from '@/lib/axios'
import type { ActivityListItem } from '@/types/activities'
import type { ApiResponse, PagedResult } from '@/types/api'

export async function getActivities(): Promise<ActivityListItem[]> {
  const { data } = await api.get<PagedResult<ActivityListItem>>('/adminactivities', {
    params: { pageNumber: 1, pageSize: 0 },
  })
  return data.items
}

export async function deleteActivity(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/adminactivities/${id}`)
  return data.data
}
