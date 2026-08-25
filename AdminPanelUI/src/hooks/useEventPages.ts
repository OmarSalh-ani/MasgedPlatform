import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { deleteEventPage, getEventPages } from '@/services/eventPagesService'

export const EVENT_PAGES_QUERY_KEY = ['event-pages'] as const

export function useEventPages() {
  const queryClient = useQueryClient()

  const query = useQuery({
    queryKey: EVENT_PAGES_QUERY_KEY,
    queryFn: getEventPages,
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteEventPage(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: EVENT_PAGES_QUERY_KEY })
    },
  })

  return { query, deleteMutation }
}
