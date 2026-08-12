export interface Tip {
  id: number
  title: string
  description: string | null
  imageUrl: string | null
  linkUrl: string | null
  sortOrder: number
  createdAt: string
}

export interface TipListItem {
  id: number
  title: string
  description: string | null
}

export interface SaveTipPayload {
  title: string
  description: string | null
  linkUrl: string | null
  sortOrder: number
  imageFile?: File
}
