export interface Activity {
  id: number
  title: string
  description: string | null
  sortOrder: number
  imageUrl: string | null
  createdAt: string
}

export interface SaveActivityPayload {
  title: string
  description: string | null
  sortOrder: number
  imageFile?: File
}
