import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  createWomansActivity,
  deleteWomansActivity,
  exportWomansActivitiesExcel,
  getWomansActivities,
  updateWomansActivity,
} from '@/services/womansActivitiesService'
import type { SaveWomanActivityPayload } from '@/types/womansActivity'

export const WOMANS_ACTIVITIES_QUERY_KEY = ['womans-activities'] as const

export function useWomansActivities() {
  const queryClient = useQueryClient()

  const query = useQuery({
    queryKey: WOMANS_ACTIVITIES_QUERY_KEY,
    queryFn: getWomansActivities,
  })

  const saveMutation = useMutation({
    mutationFn: ({
      id,
      payload,
    }: {
      id?: number
      payload: SaveWomanActivityPayload
    }) => (id ? updateWomansActivity(id, payload) : createWomansActivity(payload)),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: WOMANS_ACTIVITIES_QUERY_KEY })
    },
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteWomansActivity(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: WOMANS_ACTIVITIES_QUERY_KEY })
    },
  })

  const exportMutation = useMutation({
    mutationFn: exportWomansActivitiesExcel,
    onSuccess: (blob) => {
      const url = URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = 'Activities.xlsx'
      link.click()
      URL.revokeObjectURL(url)
    },
  })

  return { query, saveMutation, deleteMutation, exportMutation }
}
