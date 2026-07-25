import api from '@/lib/axios'
import type { ApiResponse, PagedResult } from '@/types/api'
import type { PlanLevelDto, PlanLevelListItem, SavePlanLevelPayload } from '@/types/planLevel'

export async function getPlanLevels(): Promise<PlanLevelListItem[]> {
  const { data } = await api.get<PagedResult<PlanLevelListItem>>('/adminplanlevels', {
    params: { pageNumber: 1, pageSize: 0 },
  })
  return data.items
}

export async function createPlanLevel(payload: SavePlanLevelPayload): Promise<PlanLevelDto> {
  const { data } = await api.post<ApiResponse<PlanLevelDto>>('/adminplanlevel', payload)
  return data.data
}

export async function updatePlanLevel(
  id: number,
  payload: SavePlanLevelPayload,
): Promise<PlanLevelDto> {
  const { data } = await api.put<ApiResponse<PlanLevelDto>>(`/adminplanlevel/${id}`, payload)
  return data.data
}

export async function deletePlanLevel(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/adminplanlevels/${id}`)
  return data.data
}
