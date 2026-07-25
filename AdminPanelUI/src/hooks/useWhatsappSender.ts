import { useMutation, useQuery } from '@tanstack/react-query'
import {
  getWhatsappSenderFilterOptions,
  getWhatsappSenderFormOptions,
  getWhatsappSenderStudents,
  sendWhatsappSenderMessage,
} from '@/services/whatsappSenderService'
import type { WhatsappSenderFilters } from '@/types/whatsappSender'

const LIST_KEY = ['whatsapp-sender', 'list'] as const

export function useWhatsappSenderFilterOptions() {
  return useQuery({
    queryKey: ['whatsapp-sender', 'filter-options'],
    queryFn: getWhatsappSenderFilterOptions,
  })
}

export function useWhatsappSenderFormOptions() {
  return useQuery({
    queryKey: ['whatsapp-sender', 'form-options'],
    queryFn: getWhatsappSenderFormOptions,
  })
}

export function useWhatsappSender(appliedFilters: WhatsappSenderFilters | null) {
  const listQuery = useQuery({
    queryKey: [...LIST_KEY, appliedFilters],
    queryFn: () => getWhatsappSenderStudents(appliedFilters!),
    enabled: appliedFilters != null,
  })

  const whatsappMutation = useMutation({ mutationFn: sendWhatsappSenderMessage })

  return { listQuery, whatsappMutation }
}
