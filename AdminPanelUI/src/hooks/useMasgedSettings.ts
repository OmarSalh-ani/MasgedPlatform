import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { MASGED_SETTINGS_QUERY_KEY } from '@/contexts/MasgedBrandingContext'
import { getMasgedSettings, saveMasgedSettings } from '@/services/masgedSettingsService'
import type { SaveMasgedSettingsPayload } from '@/types/masgedSettings'

export function useMasgedSettings() {
  const queryClient = useQueryClient()

  const query = useQuery({
    queryKey: MASGED_SETTINGS_QUERY_KEY,
    queryFn: getMasgedSettings,
  })

  const mutation = useMutation({
    mutationFn: (payload: SaveMasgedSettingsPayload) => saveMasgedSettings(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: MASGED_SETTINGS_QUERY_KEY })
    },
  })

  return { query, mutation }
}
