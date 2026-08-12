import type { ApiResponse } from '@/types/publicIndex'

export interface MasgedSettings {
  id: number
  masgedName: string
  logoUrl: string | null
  parentAppStoreUrl: string | null
  parentGooglePlayUrl: string | null
  teacherAppStoreUrl: string | null
  teacherGooglePlayUrl: string | null
  primaryColor: string | null
}

export type MasgedSettingsApiResponse = ApiResponse<MasgedSettings | null>
