import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { deleteMosqueFromList, getMosques } from '@/services/mosquesService'

export const MOSQUES_QUERY_KEY = ['mosques'] as const

export function useMosques() {
  const queryClient = useQueryClient()

  const query = useQuery({
    queryKey: MOSQUES_QUERY_KEY,
    queryFn: getMosques,
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteMosqueFromList(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: MOSQUES_QUERY_KEY })
    },
  })

  return { query, deleteMutation }
}
