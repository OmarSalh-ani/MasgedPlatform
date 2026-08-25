export const EVENT_PAGE_FIELD_TYPES = [
  { value: 'Text', label: 'نص' },
  { value: 'Number', label: 'رقم' },
  { value: 'SingleSelect', label: 'اختيار واحد' },
  { value: 'MultiSelect', label: 'اختيار متعدد' },
] as const

export type EventPageFieldType = (typeof EVENT_PAGE_FIELD_TYPES)[number]['value']

export interface EventPageTrack {
  id: number
  title: string
  description: string | null
  sortOrder: number
}

export interface EventPageFormField {
  id: number
  label: string
  fieldType: EventPageFieldType
  isRequired: boolean
  sortOrder: number
  options: string[]
}

export interface EventPageListItem {
  id: number
  activityName: string
  slug: string
  courseTitle: string
  imageUrl: string | null
  isPublished: boolean
  isRegistrationOpen: boolean
  createdAt: string
}

export interface EventPageLookup {
  id: number
  activityName: string
}

export interface EventPage {
  id: number
  activityName: string
  slug: string
  courseTitle: string
  invitationText: string | null
  mosqueName: string | null
  subjectText: string | null
  dateText: string | null
  timeText: string | null
  extraNotes: string | null
  supervisorsText: string | null
  contactPhone: string | null
  socialAccounts: string | null
  locationNote: string | null
  imageUrl: string | null
  isPublished: boolean
  isRegistrationOpen: boolean
  createdAt: string
  tracks: EventPageTrack[]
  formFields: EventPageFormField[]
}

export interface SaveEventPageTrackPayload {
  title: string
  description?: string
  sortOrder: number
}

export interface SaveEventPageFieldPayload {
  id?: number
  label: string
  fieldType: EventPageFieldType
  isRequired: boolean
  sortOrder: number
  options: string[]
}

export interface SaveEventPagePayload {
  activityName: string
  slug: string
  courseTitle: string
  invitationText?: string
  mosqueName?: string
  subjectText?: string
  dateText?: string
  timeText?: string
  extraNotes?: string
  supervisorsText?: string
  contactPhone?: string
  socialAccounts?: string
  locationNote?: string
  isPublished: boolean
  isRegistrationOpen: boolean
  imageFile?: File
  tracks: SaveEventPageTrackPayload[]
  formFields: SaveEventPageFieldPayload[]
}
