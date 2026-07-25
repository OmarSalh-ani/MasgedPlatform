import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type { WhatsappQrStatus } from '@/types/whatsappQr'

export async function getWhatsappQrStatus(): Promise<WhatsappQrStatus> {
  const { data } = await api.get<ApiResponse<WhatsappQrStatus>>('/adminwhatsappqr')
  return data.data
}

export async function refreshWhatsappQr(): Promise<WhatsappQrStatus> {
  const { data } = await api.post<ApiResponse<WhatsappQrStatus>>('/adminwhatsappqr/refresh')
  return data.data
}

export async function checkWhatsappQrHealth(): Promise<WhatsappQrStatus> {
  const { data } = await api.post<ApiResponse<WhatsappQrStatus>>('/adminwhatsappqr/check-health')
  return data.data
}

export async function createWhatsappSession(phoneNumber: string): Promise<WhatsappQrStatus> {
  const { data } = await api.post<ApiResponse<WhatsappQrStatus>>('/adminwhatsappqr/create-session', {
    phoneNumber,
  })
  return data.data
}

export async function disconnectWhatsapp(): Promise<WhatsappQrStatus> {
  const { data } = await api.post<ApiResponse<WhatsappQrStatus>>('/adminwhatsappqr/disconnect')
  return data.data
}

export async function reconnectWhatsapp(): Promise<WhatsappQrStatus> {
  const { data } = await api.post<ApiResponse<WhatsappQrStatus>>('/adminwhatsappqr/reconnect')
  return data.data
}
