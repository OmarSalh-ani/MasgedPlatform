import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { deleteNewsItem, getNewsList } from '@/services/newsService'

export const NEWS_LIST_QUERY_KEY = ['news', 'list'] as const

export function useNews() {
  const queryClient = useQueryClient()

  const query = useQuery({
    queryKey: NEWS_LIST_QUERY_KEY,
    queryFn: getNewsList,
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteNewsItem(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: NEWS_LIST_QUERY_KEY })
    },
  })

  return { query, deleteMutation }
}
