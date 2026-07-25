import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type { WhatsappPendingMessage } from '@/types/whatsappPending'

export async function getWhatsappPendingMessages(): Promise<WhatsappPendingMessage[]> {
  const { data } = await api.get<ApiResponse<WhatsappPendingMessage[]>>('/adminwhatsapppending')
  return data.data
}

export async function deleteSelectedWhatsappPending(ids: number[]): Promise<number> {
  const { data } = await api.post<ApiResponse<number>>('/adminwhatsapppending/delete-selected', { ids })
  return data.data
}

export async function deleteAllWhatsappPending(): Promise<number> {
  const { data } = await api.delete<ApiResponse<number>>('/adminwhatsapppending')
  return data.data
}
