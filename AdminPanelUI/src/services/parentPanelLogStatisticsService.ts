import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type {
  ParentPanelLogStatisticsFilters,
  ParentPanelLogStatisticsResponse,
} from '@/types/parentPanelLogStatistics'

export async function getParentPanelLogStatistics(
  filters: ParentPanelLogStatisticsFilters,
): Promise<ParentPanelLogStatisticsResponse> {
  const { data } = await api.get<ApiResponse<ParentPanelLogStatisticsResponse>>(
    '/adminparentpanellogstatistics',
    {
      params: {
        fromDate: filters.fromDate,
        toDate: filters.toDate,
      },
    },
  )
  return data.data
}
