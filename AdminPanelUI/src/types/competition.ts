export interface Competition {
  id: number
  title: string
  description: string | null
  imageUrl: string | null
  linkUrl: string | null
  sortOrder: number
  createdAt: string
}

export interface CompetitionListItem {
  id: number
  title: string
  description: string | null
}

export interface SaveCompetitionPayload {
  title: string
  description: string | null
  linkUrl: string | null
  sortOrder: number
  imageFile?: File
}
