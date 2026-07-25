import { createContext, useContext, useEffect, useMemo, type ReactNode } from 'react'
import { useQuery } from '@tanstack/react-query'
import { applyPrimaryColor } from '@/lib/applyPrimaryColor'
import {
  DEFAULT_MASGED_LOGO_URL,
  DEFAULT_MASGED_NAME,
  DEFAULT_PRIMARY_COLOR,
} from '@/lib/masgedBrandingDefaults'
import { resolveLogoUrl } from '@/lib/resolveLogoUrl'
import { getMasgedSettings } from '@/services/masgedSettingsService'

export const MASGED_SETTINGS_QUERY_KEY = ['masgedSettings'] as const

export interface MasgedBrandingValue {
  masgedName: string
  logoUrl: string
  primaryColor: string
  domain: string | null
  setupCompleted: boolean
  isLoading: boolean
}

const MasgedBrandingContext = createContext<MasgedBrandingValue>({
  masgedName: DEFAULT_MASGED_NAME,
  logoUrl: DEFAULT_MASGED_LOGO_URL,
  primaryColor: DEFAULT_PRIMARY_COLOR,
  domain: null,
  setupCompleted: true,
  isLoading: false,
})

export function MasgedBrandingProvider({ children }: { children: ReactNode }) {
  const query = useQuery({
    queryKey: MASGED_SETTINGS_QUERY_KEY,
    queryFn: getMasgedSettings,
    staleTime: 5 * 60 * 1000,
  })

  const value = useMemo<MasgedBrandingValue>(() => {
    const masgedName = query.data?.masgedName?.trim() || DEFAULT_MASGED_NAME
    const logoUrl = resolveLogoUrl(query.data?.logoUrl) || DEFAULT_MASGED_LOGO_URL
    const primaryColor = query.data?.primaryColor?.trim() || DEFAULT_PRIMARY_COLOR
    return {
      masgedName,
      logoUrl,
      primaryColor,
      domain: query.data?.domain ?? null,
      setupCompleted: query.data?.setupCompleted === true,
      isLoading: query.isLoading,
    }
  }, [query.data, query.isLoading])

  useEffect(() => {
    applyPrimaryColor(value.primaryColor)
  }, [value.primaryColor])

  return (
    <MasgedBrandingContext.Provider value={value}>{children}</MasgedBrandingContext.Provider>
  )
}

export function useMasgedBranding() {
  return useContext(MasgedBrandingContext)
}
