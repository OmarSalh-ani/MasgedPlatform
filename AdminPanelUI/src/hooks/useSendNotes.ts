import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { deleteSendNote, getSendNotesList } from '@/services/sendNoteService'

export const SEND_NOTES_LIST_QUERY_KEY = ['send-notes', 'list'] as const

export function useSendNotes(pageNumber: number) {
  const queryClient = useQueryClient()

  const query = useQuery({
    queryKey: [...SEND_NOTES_LIST_QUERY_KEY, pageNumber],
    queryFn: () => getSendNotesList(pageNumber),
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteSendNote(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: SEND_NOTES_LIST_QUERY_KEY })
    },
  })

  return { query, deleteMutation }
}
