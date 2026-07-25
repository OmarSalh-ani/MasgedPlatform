export interface ContactInfo {
  id: number
  contactType: string
  label: string | null
  value: string
  sortOrder: number
}

export interface ContactInfoListItem {
  id: number
  contactType: string
  label: string | null
  value: string
}

export interface SaveContactInfoPayload {
  contactType: string
  label: string | null
  value: string
  sortOrder: number
}
