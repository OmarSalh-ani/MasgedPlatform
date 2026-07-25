export interface ApiResponse<T> {
  success: boolean
  message: string
  data: T
  errors: string[]
}

export interface PublicCompetitionItem {
  id: number
  title: string
  description?: string | null
  imageUrl?: string | null
  linkUrl?: string | null
}

export interface PublicMosqueItem {
  id: number
  name: string
  description?: string | null
  googleMapsUrl?: string | null
  imageUrl?: string | null
}

export interface PublicNewsItem {
  id: number
  title: string
  description?: string | null
  newsDate: string
  imageUrl?: string | null
  linkUrl?: string | null
}

export interface PublicActivityItem {
  id: number
  title: string
  description?: string | null
  imageUrl?: string | null
  iconClass?: string | null
}

export interface PublicHeroSlideItem {
  id: number
  imageUrl: string
}

export interface PublicSocialLinkItem {
  id: number
  platformName: string
  url: string
  iconClass?: string | null
  resolvedIconClass: string
}

export interface PublicAbout {
  content: string
  address?: string | null
  mapsUrl?: string | null
}

export interface PublicWebsiteContent {
  heroSlides: PublicHeroSlideItem[]
  competitions: PublicCompetitionItem[]
  mosques: PublicMosqueItem[]
  news: PublicNewsItem[]
  activities: PublicActivityItem[]
  about: PublicAbout
  socialLinks: PublicSocialLinkItem[]
}

export interface PublicWomanActivityOption {
  id: number
  name: string
}

export interface PublicRegistrationFormLabels {
  fullNameLabel: string
  parentPhone1Label: string
  learnCertificateLabel: string
  showLearnDiv: boolean
  showBirthdateDiv: boolean
  showAgeDiv: boolean
  showPhone2Div: boolean
  showActivitiesSection: boolean
  showActivitiesNav: boolean
}

export interface PublicRegistrationConfig {
  mode: string
  registrationEnabled: boolean
  labels: PublicRegistrationFormLabels
  womanActivities: PublicWomanActivityOption[]
}

export interface CountryDialEntry {
  name: string
  dial_code: string
  code: string
}

export interface SubmitRegistrationPayload {
  mode: string
  fullName: string
  birthdate?: string
  age?: number
  parentPhoneCountryIso: string
  parentPhone1: string
  parentPhone2?: string
  parentPhone2CountryIso?: string
  learnCertificate?: string
  womanActivityTypeId: number
}

export interface PublicRegisterSuccess {
  headText: string
  titleText: string
  subscribeText: string
  whatsappUrl: string
  socialLinks: PublicSocialLinkItem[]
}

export type RegistrationMode = 'default' | 'mregister' | 'wregister'

export function parseRegistrationMode(value: string | null): RegistrationMode {
  const normalized = value?.toLowerCase()
  if (normalized === 'mregister' || normalized === 'wregister') return normalized
  return 'default'
}
