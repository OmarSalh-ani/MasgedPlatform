import api from '@/lib/axios'
import type { ContactInfo, SaveContactInfoPayload } from '@/types/contactInfo'
import type { ApiResponse } from '@/types/api'

export async function getContactInfo(id: number): Promise<ContactInfo> {
  const { data } = await api.get<ApiResponse<ContactInfo>>(`/admincontactinfo/${id}`)
  return data.data
}

export async function getNextContactInfoSortOrder(): Promise<number> {
  const { data } = await api.get<ApiResponse<number>>('/admincontactinfo/next-sort-order')
  return data.data
}

export async function createContactInfo(payload: SaveContactInfoPayload): Promise<ContactInfo> {
  const { data } = await api.post<ApiResponse<ContactInfo>>('/admincontactinfo', payload)
  return data.data
}

export async function updateContactInfo(
  id: number,
  payload: SaveContactInfoPayload,
): Promise<ContactInfo> {
  const { data } = await api.put<ApiResponse<ContactInfo>>(`/admincontactinfo/${id}`, payload)
  return data.data
}

export async function deleteContactInfo(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/admincontactinfo/${id}`)
  return data.data
}
