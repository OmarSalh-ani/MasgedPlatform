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

export interface SaveMasgedSettingsPayload {
  masgedName: string
  logoFile?: File | null
  removeLogo?: boolean
  parentAppStoreUrl?: string | null
  parentGooglePlayUrl?: string | null
  teacherAppStoreUrl?: string | null
  teacherGooglePlayUrl?: string | null
  primaryColor?: string | null
}
