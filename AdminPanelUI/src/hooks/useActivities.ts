import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { deleteActivity, getActivities } from '@/services/activitiesService'

export const ACTIVITIES_QUERY_KEY = ['activities'] as const

export function useActivities() {
  const queryClient = useQueryClient()

  const query = useQuery({
    queryKey: ACTIVITIES_QUERY_KEY,
    queryFn: getActivities,
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteActivity(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ACTIVITIES_QUERY_KEY })
    },
  })

  return { query, deleteMutation }
}
