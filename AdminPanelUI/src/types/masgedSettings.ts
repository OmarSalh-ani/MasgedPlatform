export interface MasgedSettings {
  id: number
  masgedName: string
  logoUrl: string | null
  parentAppStoreUrl: string | null
  parentGooglePlayUrl: string | null
  teacherAppStoreUrl: string | null
  teacherGooglePlayUrl: string | null
  primaryColor: string | null
  domain: string | null
  setupCompleted: boolean
}

export interface SetupStatus {
  setupCompleted: boolean
  domain: string | null
}

export interface FirstTimeSetupPayload {
  masgedName: string
  primaryColor: string
  domain: string
  logoFile?: File | null
  parentAppStoreUrl?: string | null
  parentGooglePlayUrl?: string | null
  teacherAppStoreUrl?: string | null
  teacherGooglePlayUrl?: string | null
  adminName: string
  adminEmail: string
  adminPassword: string
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
  domain?: string | null
}
