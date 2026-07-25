import api from '@/lib/axios'
import type { ApiResponse, PagedResult } from '@/types/api'
import type { HomeFilterOptions, HomeStudentListItem } from '@/types/home'
import type { WhatsappSenderFilters, WhatsappSenderFormOption } from '@/types/whatsappSender'

export async function getWhatsappSenderStudents(
  filters: WhatsappSenderFilters,
): Promise<PagedResult<HomeStudentListItem>> {
  const { data } = await api.get<PagedResult<HomeStudentListItem>>('/adminwhatsappsender', {
    params: filters,
  })
  return data
}

export async function getWhatsappSenderFilterOptions(): Promise<HomeFilterOptions> {
  const { data } = await api.get<ApiResponse<HomeFilterOptions>>('/adminwhatsappsender/filter-options')
  return data.data
}

export async function getWhatsappSenderFormOptions(): Promise<WhatsappSenderFormOption[]> {
  const { data } = await api.get<ApiResponse<WhatsappSenderFormOption[]>>(
    '/adminwhatsappsender/form-options',
  )
  return data.data
}

export async function sendWhatsappSenderMessage(payload: {
  studentIds: number[]
  message: string
  formId?: number | null
  image?: File | null
}): Promise<string> {
  const formData = new FormData()
  formData.append('studentIds', payload.studentIds.join(','))
  formData.append('message', payload.message)
  if (payload.formId) formData.append('formId', String(payload.formId))
  if (payload.image) formData.append('image', payload.image)

  const { data } = await api.post<ApiResponse<string>>('/adminwhatsappsender/whatsapp', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  })
  return data.message
}
