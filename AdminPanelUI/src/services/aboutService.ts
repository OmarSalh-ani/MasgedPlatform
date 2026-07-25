import api from '@/lib/axios'
import type { About, UpdateAboutRequest } from '@/types/about'
import type { ApiResponse } from '@/types/api'

export async function getAbout(): Promise<About | null> {
  const { data } = await api.get<ApiResponse<About | null>>('/adminabout')
  return data.data
}

export async function saveAbout(request: UpdateAboutRequest): Promise<About> {
  const { data } = await api.put<ApiResponse<About>>('/adminabout', request)
  return data.data
}
