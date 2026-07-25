import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type { WorkDays } from '@/types/workDays'

export async function getWorkDays(): Promise<WorkDays> {
  const { data } = await api.get<ApiResponse<WorkDays>>('/adminworkdays')
  return data.data
}

export async function saveWorkDays(dayNumbers: number[]): Promise<WorkDays> {
  const { data } = await api.put<ApiResponse<WorkDays>>('/adminworkdays', { dayNumbers })
  return data.data
}
