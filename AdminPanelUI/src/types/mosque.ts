export interface Mosque {
  id: number
  name: string
  description: string | null
  googleMapsUrl: string | null
  imageUrl: string | null
  sortOrder: number
}

export interface MosqueListItem {
  id: number
  name: string
  description: string | null
  imageUrl: string | null
}

export interface SaveMosquePayload {
  name: string
  description: string | null
  googleMapsUrl: string | null
  sortOrder: number
  imageFile?: File
}
