import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  getWhatsappPreConfiguredMessages,
  getWhatsappPreConfiguredTestPreview,
  setWhatsappPreConfiguredEnabled,
  updateWhatsappPreConfiguredMessage,
} from '@/services/whatsappPreConfiguredService'

const QUERY_KEY = ['whatsapp-preconfigured'] as const

export function useWhatsappPreConfigured() {
  const queryClient = useQueryClient()
  const query = useQuery({ queryKey: QUERY_KEY, queryFn: getWhatsappPreConfiguredMessages })

  const saveMutation = useMutation({
    mutationFn: ({ id, message }: { id: number; message: string }) =>
      updateWhatsappPreConfiguredMessage(id, message),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: QUERY_KEY }),
  })

  const enabledMutation = useMutation({
    mutationFn: ({ id, isEnabled }: { id: number; isEnabled: boolean }) =>
      setWhatsappPreConfiguredEnabled(id, isEnabled),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: QUERY_KEY }),
  })

  const testMutation = useMutation({
    mutationFn: getWhatsappPreConfiguredTestPreview,
  })

  return { query, saveMutation, enabledMutation, testMutation }
}
