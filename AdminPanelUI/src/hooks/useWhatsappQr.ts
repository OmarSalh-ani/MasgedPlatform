import { useMutation, useQuery } from '@tanstack/react-query'
import {
  checkWhatsappQrHealth,
  createWhatsappSession,
  disconnectWhatsapp,
  getWhatsappQrStatus,
  reconnectWhatsapp,
  refreshWhatsappQr,
} from '@/services/whatsappQrService'

const QUERY_KEY = ['whatsapp-qr'] as const

export function useWhatsappQr() {
  const query = useQuery({ queryKey: QUERY_KEY, queryFn: getWhatsappQrStatus })

  const refreshMutation = useMutation({ mutationFn: refreshWhatsappQr })
  const healthMutation = useMutation({ mutationFn: checkWhatsappQrHealth })
  const createMutation = useMutation({ mutationFn: createWhatsappSession })
  const disconnectMutation = useMutation({ mutationFn: disconnectWhatsapp })
  const reconnectMutation = useMutation({ mutationFn: reconnectWhatsapp })

  return {
    query,
    refreshMutation,
    healthMutation,
    createMutation,
    disconnectMutation,
    reconnectMutation,
  }
}
