export interface About {
  id: number
  content: string | null
  address: string | null
  mapsUrl: string | null
}

export interface UpdateAboutRequest {
  content?: string | null
  address?: string | null
  mapsUrl?: string | null
}
