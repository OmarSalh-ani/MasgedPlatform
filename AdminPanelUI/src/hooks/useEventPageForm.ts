import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import {
  createEventPage,
  deleteEventPage,
  getEventPage,
  updateEventPage,
} from '@/services/eventPagesService'
import { EVENT_PAGES_QUERY_KEY } from '@/hooks/useEventPages'
import type { SaveEventPagePayload } from '@/types/eventPage'

export function useEventPageForm(eventPageId?: number) {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const isEdit = eventPageId !== undefined

  const eventPageQuery = useQuery({
    queryKey: ['event-page', eventPageId],
    queryFn: () => getEventPage(eventPageId!),
    enabled: isEdit,
  })

  const invalidateAndGoToList = () => {
    queryClient.invalidateQueries({ queryKey: EVENT_PAGES_QUERY_KEY })
    navigate('/event-pages')
  }

  const saveMutation = useMutation({
    mutationFn: (payload: SaveEventPagePayload) =>
      isEdit ? updateEventPage(eventPageId!, payload) : createEventPage(payload),
    onSuccess: invalidateAndGoToList,
  })

  const deleteMutation = useMutation({
    mutationFn: () => deleteEventPage(eventPageId!),
    onSuccess: invalidateAndGoToList,
  })

  return {
    isEdit,
    eventPageQuery,
    saveMutation,
    deleteMutation,
  }
}
