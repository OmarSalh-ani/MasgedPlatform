import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type { WhatsappPreConfiguredMessage } from '@/types/whatsappPreConfigured'

export async function getWhatsappPreConfiguredMessages(): Promise<WhatsappPreConfiguredMessage[]> {
  const { data } = await api.get<ApiResponse<WhatsappPreConfiguredMessage[]>>(
    '/adminwhatsapppreconfigured',
  )
  return data.data
}

export async function updateWhatsappPreConfiguredMessage(
  id: number,
  whatsappMessage: string,
): Promise<WhatsappPreConfiguredMessage> {
  const { data } = await api.put<ApiResponse<WhatsappPreConfiguredMessage>>(
    `/adminwhatsapppreconfigured/${id}`,
    { whatsappMessage },
  )
  return data.data
}

export async function setWhatsappPreConfiguredEnabled(
  id: number,
  isEnabled: boolean,
): Promise<WhatsappPreConfiguredMessage> {
  const { data } = await api.put<ApiResponse<WhatsappPreConfiguredMessage>>(
    `/adminwhatsapppreconfigured/${id}/enabled`,
    { isEnabled },
  )
  return data.data
}

export async function getWhatsappPreConfiguredTestPreview(id: number): Promise<string> {
  const { data } = await api.get<ApiResponse<string>>(
    `/adminwhatsapppreconfigured/${id}/test-preview`,
  )
  return data.data
}
