import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type { StatisticsResponse } from '@/types/statistics'

export async function getStatistics(): Promise<StatisticsResponse> {
  const { data } = await api.get<ApiResponse<StatisticsResponse>>('/adminstatistics')
  return data.data
}
