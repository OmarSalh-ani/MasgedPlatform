import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { deleteTip, getTips } from '@/services/tipService'

export const TIPS_QUERY_KEY = ['tips'] as const

export function useTips() {
  const queryClient = useQueryClient()

  const query = useQuery({
    queryKey: TIPS_QUERY_KEY,
    queryFn: getTips,
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteTip(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TIPS_QUERY_KEY })
    },
  })

  return { query, deleteMutation }
}
