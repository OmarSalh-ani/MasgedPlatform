import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { deleteCompetition, getCompetitions } from '@/services/competitionService'

export const COMPETITIONS_QUERY_KEY = ['competitions'] as const

export function useCompetitions() {
  const queryClient = useQueryClient()

  const query = useQuery({
    queryKey: COMPETITIONS_QUERY_KEY,
    queryFn: getCompetitions,
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteCompetition(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: COMPETITIONS_QUERY_KEY })
    },
  })

  return { query, deleteMutation }
}
