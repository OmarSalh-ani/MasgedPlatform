import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  getIntegrationSettings,
  saveIntegrationSettings,
} from '@/services/integrationsService'
import type { UpdateIntegrationSettingsPayload } from '@/types/integrations'

export const INTEGRATIONS_QUERY_KEY = ['integrations'] as const

export function useIntegrations() {
  const queryClient = useQueryClient()
  const query = useQuery({
    queryKey: INTEGRATIONS_QUERY_KEY,
    queryFn: getIntegrationSettings,
  })

  const mutation = useMutation({
    mutationFn: (payload: UpdateIntegrationSettingsPayload) =>
      saveIntegrationSettings(payload),
    onSuccess: (data) => {
      queryClient.setQueryData(INTEGRATIONS_QUERY_KEY, data)
    },
  })

  return { query, mutation }
}
