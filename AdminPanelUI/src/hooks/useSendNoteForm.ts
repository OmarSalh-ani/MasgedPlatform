import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import {
  createSendNotes,
  getSendNote,
  getSendNoteTeachers,
  updateSendNote,
} from '@/services/sendNoteService'
import { SEND_NOTES_LIST_QUERY_KEY } from '@/hooks/useSendNotes'
import type { CreateSendNotePayload, UpdateSendNotePayload } from '@/types/sendNote'

export function useSendNoteForm(noteId?: number) {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const isEdit = noteId !== undefined

  const noteQuery = useQuery({
    queryKey: ['send-notes', noteId],
    queryFn: () => getSendNote(noteId!),
    enabled: isEdit,
  })

  const teachersQuery = useQuery({
    queryKey: ['send-notes', 'teachers'],
    queryFn: getSendNoteTeachers,
    enabled: !isEdit,
  })

  const invalidateAndGoToList = () => {
    queryClient.invalidateQueries({ queryKey: SEND_NOTES_LIST_QUERY_KEY })
    navigate('/send-notes')
  }

  const createMutation = useMutation({
    mutationFn: (payload: CreateSendNotePayload) => createSendNotes(payload),
    onSuccess: invalidateAndGoToList,
  })

  const updateMutation = useMutation({
    mutationFn: (payload: UpdateSendNotePayload) => updateSendNote(noteId!, payload),
    onSuccess: invalidateAndGoToList,
  })

  return {
    isEdit,
    noteQuery,
    teachersQuery,
    createMutation,
    updateMutation,
  }
}
