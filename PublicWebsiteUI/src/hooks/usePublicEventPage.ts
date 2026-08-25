import { useMutation, useQuery } from '@tanstack/react-query'
import {
  getPublicEventPage,
  submitPublicEventPageRegistration,
} from '@/services/publicEventPageService'
import type { SubmitEventPageRegistrationPayload } from '@/types/eventPage'

export function usePublicEventPage(slug: string | undefined) {
  return useQuery({
    queryKey: ['public-event-page', slug],
    queryFn: () => getPublicEventPage(slug!),
    enabled: Boolean(slug),
    retry: false,
  })
}

export function useSubmitPublicEventPage(slug: string | undefined) {
  return useMutation({
    mutationFn: (payload: SubmitEventPageRegistrationPayload) =>
      submitPublicEventPageRegistration(slug!, payload),
  })
}
