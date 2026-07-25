import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type {
  IntegrationSettings,
  UpdateIntegrationSettingsPayload,
} from '@/types/integrations'

export async function getIntegrationSettings(): Promise<IntegrationSettings> {
  const { data } = await api.get<ApiResponse<IntegrationSettings>>('/adminintegrations')
  return data.data
}

export async function saveIntegrationSettings(
  payload: UpdateIntegrationSettingsPayload,
): Promise<IntegrationSettings> {
  const { data } = await api.put<ApiResponse<IntegrationSettings>>(
    '/adminintegrations',
    payload,
  )
  return data.data
}
