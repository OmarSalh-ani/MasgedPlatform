import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  deleteAllWhatsappPending,
  deleteSelectedWhatsappPending,
  getWhatsappPendingMessages,
} from '@/services/whatsappPendingService'

const QUERY_KEY = ['whatsapp-pending'] as const

export function useWhatsappPending() {
  const queryClient = useQueryClient()
  const query = useQuery({ queryKey: QUERY_KEY, queryFn: getWhatsappPendingMessages })

  const deleteSelectedMutation = useMutation({
    mutationFn: deleteSelectedWhatsappPending,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: QUERY_KEY }),
  })

  const deleteAllMutation = useMutation({
    mutationFn: deleteAllWhatsappPending,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: QUERY_KEY }),
  })

  return { query, deleteSelectedMutation, deleteAllMutation }
}
