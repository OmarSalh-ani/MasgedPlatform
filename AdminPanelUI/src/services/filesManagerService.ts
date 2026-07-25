import api from '@/lib/axios'
import type { ApiResponse, PagedResult } from '@/types/api'
import type {
  FilesManager,
  FilesManagerListItem,
  SaveFilesManagerPayload,
} from '@/types/filesManager'

function toFormData(payload: SaveFilesManagerPayload): FormData {
  const formData = new FormData()
  formData.append('name', payload.name)
  formData.append('file', payload.file)
  return formData
}

export async function getFilesManagers(): Promise<FilesManagerListItem[]> {
  const { data } = await api.get<PagedResult<FilesManagerListItem>>('/adminfilesmanagers', {
    params: { pageNumber: 1, pageSize: 0 },
  })
  return data.items
}

export async function getFilesManager(id: number): Promise<FilesManager> {
  const { data } = await api.get<ApiResponse<FilesManager>>(`/adminfilesmanagers/${id}`)
  return data.data
}

export async function createFilesManager(payload: SaveFilesManagerPayload): Promise<FilesManager> {
  const { data } = await api.post<ApiResponse<FilesManager>>(
    '/adminfilesmanagers',
    toFormData(payload),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.data
}

export async function updateFilesManager(
  id: number,
  payload: SaveFilesManagerPayload,
): Promise<FilesManager> {
  const { data } = await api.put<ApiResponse<FilesManager>>(
    `/adminfilesmanagers/${id}`,
    toFormData(payload),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.data
}

export async function deleteFilesManager(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/adminfilesmanagers/${id}`)
  return data.data
}

export async function exportFilesManagersExcel(): Promise<void> {
  const response = await api.get<Blob>('/adminfilesmanagers/export/excel', {
    responseType: 'blob',
  })

  const disposition = response.headers['content-disposition'] as string | undefined
  const fileName =
    disposition?.match(/filename="?([^";]+)"?/)?.[1] ??
    `Files_${new Date().toISOString().replace(/[-:T]/g, '').slice(0, 15)}.xlsx`

  const url = window.URL.createObjectURL(response.data)
  const link = document.createElement('a')
  link.href = url
  link.download = fileName
  link.click()
  window.URL.revokeObjectURL(url)
}
