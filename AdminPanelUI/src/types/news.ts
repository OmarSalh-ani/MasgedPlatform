export interface NewsListItem {
  id: number
  title: string
  newsDate: string
}

export function formatNewsDate(isoDate: string): string {
  const date = new Date(isoDate)
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  return `${year}/${month}/${day}`
}

export interface News {
  id: number
  title: string
  description: string | null
  newsDate: string
  sortOrder: number
  imageUrl: string | null
  createdAt: string
}

export interface SaveNewsPayload {
  title: string
  description: string | null
  newsDate: string
  sortOrder: number
  imageFile?: File
}
