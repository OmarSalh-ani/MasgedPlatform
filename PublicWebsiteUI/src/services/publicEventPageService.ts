import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type {
  PublicEventPage,
  SubmitEventPageRegistrationPayload,
} from '@/types/eventPage'

export async function getPublicEventPage(slug: string): Promise<PublicEventPage> {
  const { data } = await api.get<ApiResponse<PublicEventPage>>(`/publiceventpages/${slug}`)
  return data.data
}

export async function submitPublicEventPageRegistration(
  slug: string,
  payload: SubmitEventPageRegistrationPayload,
): Promise<void> {
  await api.post<ApiResponse<boolean>>(`/publiceventpages/${slug}/register`, payload)
}
