export type PublicEventPageFieldType = 'Text' | 'Number' | 'SingleSelect' | 'MultiSelect'

export interface PublicEventPageTrack {
  title: string
  description: string | null
  sortOrder: number
}

export interface PublicEventPageFormField {
  id: number
  label: string
  fieldType: PublicEventPageFieldType
  isRequired: boolean
  sortOrder: number
  options: string[]
}

export interface PublicEventPage {
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
  isRegistrationOpen: boolean
  tracks: PublicEventPageTrack[]
  formFields: PublicEventPageFormField[]
}

export interface SubmitEventPageAnswer {
  fieldId: number
  value?: string
  values?: string[]
}

export interface SubmitEventPageRegistrationPayload {
  answers: SubmitEventPageAnswer[]
}
