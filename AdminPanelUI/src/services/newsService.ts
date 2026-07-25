import api from '@/lib/axios'
import type { News, NewsListItem, SaveNewsPayload } from '@/types/news'
import type { ApiResponse, PagedResult } from '@/types/api'

export async function getNewsList(): Promise<NewsListItem[]> {
  const { data } = await api.get<PagedResult<NewsListItem>>('/adminnewsitems', {
    params: { pageNumber: 1, pageSize: 0 },
  })
  return data.items
}

export async function deleteNewsItem(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/adminnewsitems/${id}`)
  return data.data
}

function toFormData(payload: SaveNewsPayload): FormData {
  const formData = new FormData()
  formData.append('title', payload.title)
  if (payload.description) {
    formData.append('description', payload.description)
  }
  formData.append('newsDate', payload.newsDate)
  formData.append('sortOrder', String(payload.sortOrder))
  if (payload.imageFile) {
    formData.append('image', payload.imageFile)
  }
  return formData
}

export async function getNews(id: number): Promise<News> {
  const { data } = await api.get<ApiResponse<News>>(`/adminnews/${id}`)
  return data.data
}

export async function getNextSortOrder(): Promise<number> {
  const { data } = await api.get<ApiResponse<number>>('/adminnews/next-sort-order')
  return data.data
}

export async function createNews(payload: SaveNewsPayload): Promise<News> {
  const { data } = await api.post<ApiResponse<News>>(
    '/adminnews',
    toFormData(payload),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.data
}

export async function updateNews(id: number, payload: SaveNewsPayload): Promise<News> {
  const { data } = await api.put<ApiResponse<News>>(
    `/adminnews/${id}`,
    toFormData(payload),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.data
}

export async function deleteNews(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/adminnews/${id}`)
  return data.data
}
