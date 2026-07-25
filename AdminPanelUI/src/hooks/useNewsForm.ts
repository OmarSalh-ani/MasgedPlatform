import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import {
  createNews,
  deleteNews,
  getNews,
  getNextSortOrder,
  updateNews,
} from '@/services/newsService'
import { NEWS_LIST_QUERY_KEY } from '@/hooks/useNews'
import type { SaveNewsPayload } from '@/types/news'

export function useNewsForm(newsId?: number) {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const isEdit = newsId !== undefined

  const newsQuery = useQuery({
    queryKey: ['news', newsId],
    queryFn: () => getNews(newsId!),
    enabled: isEdit,
  })

  const nextSortOrderQuery = useQuery({
    queryKey: ['news', 'next-sort-order'],
    queryFn: getNextSortOrder,
    enabled: !isEdit,
  })

  const invalidateAndGoToList = () => {
    queryClient.invalidateQueries({ queryKey: NEWS_LIST_QUERY_KEY })
    navigate('/news')
  }

  const saveMutation = useMutation({
    mutationFn: (payload: SaveNewsPayload) =>
      isEdit ? updateNews(newsId!, payload) : createNews(payload),
    onSuccess: invalidateAndGoToList,
  })

  const deleteMutation = useMutation({
    mutationFn: () => deleteNews(newsId!),
    onSuccess: invalidateAndGoToList,
  })

  return {
    isEdit,
    newsQuery,
    nextSortOrderQuery,
    saveMutation,
    deleteMutation,
  }
}
