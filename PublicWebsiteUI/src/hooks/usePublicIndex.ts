import { useQuery } from '@tanstack/react-query'
import { getRegistrationConfig, getWebsiteContent } from '@/services/publicIndexService'
import type { RegistrationMode } from '@/types/publicIndex'

export function useWebsiteContent() {
  return useQuery({
    queryKey: ['public-website-content'],
    queryFn: getWebsiteContent,
  })
}

export function useRegistrationConfig(mode: RegistrationMode) {
  return useQuery({
    queryKey: ['public-registration-config', mode],
    queryFn: () => getRegistrationConfig(mode),
  })
}
