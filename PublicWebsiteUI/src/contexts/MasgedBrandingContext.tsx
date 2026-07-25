import { createContext, useContext, useEffect, useMemo, type ReactNode } from 'react'
import { useQuery } from '@tanstack/react-query'
import { applyPrimaryColor } from '@/lib/applyPrimaryColor'
import { DEFAULT_LOGO_URL, DEFAULT_MASGED_NAME } from '@/lib/masgedBrandingDefaults'
import { resolveImageUrl } from '@/lib/resolveImageUrl'
import { getMasgedSettings } from '@/services/masgedSettingsService'

export const MASGED_SETTINGS_QUERY_KEY = ['masgedSettings'] as const
export const DEFAULT_PRIMARY_COLOR = '#2563eb'

export interface MobileAppStoreLinks {
  parentAppStoreUrl: string | null
  parentGooglePlayUrl: string | null
  teacherAppStoreUrl: string | null
  teacherGooglePlayUrl: string | null
}

export interface MasgedBrandingValue {
  masgedName: string
  logoUrl: string
  primaryColor: string
  mobileAppLinks: MobileAppStoreLinks
  isLoading: boolean
}

const emptyMobileAppLinks: MobileAppStoreLinks = {
  parentAppStoreUrl: null,
  parentGooglePlayUrl: null,
  teacherAppStoreUrl: null,
  teacherGooglePlayUrl: null,
}

const MasgedBrandingContext = createContext<MasgedBrandingValue>({
  masgedName: DEFAULT_MASGED_NAME,
  logoUrl: DEFAULT_LOGO_URL,
  primaryColor: DEFAULT_PRIMARY_COLOR,
  mobileAppLinks: emptyMobileAppLinks,
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
    const logoUrl = resolveImageUrl(query.data?.logoUrl) || DEFAULT_LOGO_URL
    const primaryColor = query.data?.primaryColor?.trim() || DEFAULT_PRIMARY_COLOR
    const mobileAppLinks: MobileAppStoreLinks = {
      parentAppStoreUrl: query.data?.parentAppStoreUrl?.trim() || null,
      parentGooglePlayUrl: query.data?.parentGooglePlayUrl?.trim() || null,
      teacherAppStoreUrl: query.data?.teacherAppStoreUrl?.trim() || null,
      teacherGooglePlayUrl: query.data?.teacherGooglePlayUrl?.trim() || null,
    }
    return { masgedName, logoUrl, primaryColor, mobileAppLinks, isLoading: query.isLoading }
  }, [query.data, query.isLoading])

  useEffect(() => {
    document.title = value.masgedName
    const meta = document.querySelector('meta[name="description"]')
    if (meta) {
      meta.setAttribute('content', `${value.masgedName} - نشر العلم الشرعي والقيم الإسلامية`)
    }
  }, [value.masgedName])

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
