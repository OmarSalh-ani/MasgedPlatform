import api from '@/lib/axios'
import type {
  Expensive,
  ExpensiveListItem,
  ExpensiveSummary,
  SaveExpensivePayload,
} from '@/types/expensives'
import type { ApiResponse, PagedResult } from '@/types/api'

export async function getExpensives(): Promise<ExpensiveListItem[]> {
  const { data } = await api.get<PagedResult<ExpensiveListItem>>('/adminexpensives', {
    params: { pageNumber: 1, pageSize: 0 },
  })
  return data.items
}

export async function getExpensiveSummary(): Promise<ExpensiveSummary> {
  const { data } = await api.get<ApiResponse<ExpensiveSummary>>('/adminexpensives/summary')
  return data.data
}

export async function deleteExpensive(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/adminexpensives/${id}`)
  return data.data
}

function toFormData(payload: SaveExpensivePayload): FormData {
  const formData = new FormData()
  formData.append('reason', payload.reason)
  formData.append('totalAmount', String(payload.totalAmount))
  formData.append('supplier', payload.supplier)
  if (payload.notes) {
    formData.append('notes', payload.notes)
  }
  payload.files?.forEach((file) => formData.append('files', file))
  return formData
}

export async function getExpensive(id: number): Promise<Expensive> {
  const { data } = await api.get<ApiResponse<Expensive>>(`/adminexpensives/${id}`)
  return data.data
}

export async function createExpensive(payload: SaveExpensivePayload): Promise<Expensive> {
  const { data } = await api.post<ApiResponse<Expensive>>(
    '/adminexpensives',
    toFormData(payload),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.data
}

export async function updateExpensive(
  id: number,
  payload: SaveExpensivePayload,
): Promise<Expensive> {
  const { data } = await api.put<ApiResponse<Expensive>>(
    `/adminexpensives/${id}`,
    toFormData(payload),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.data
}

export async function deleteExpensiveAttachment(
  id: number,
  fileName: string,
): Promise<boolean> {
  const encoded = encodeURIComponent(fileName)
  const { data } = await api.delete<ApiResponse<boolean>>(
    `/adminexpensives/${id}/attachments/${encoded}`,
  )
  return data.data
}

export async function downloadExpensiveAttachment(
  id: number,
  fileName: string,
): Promise<void> {
  const encoded = encodeURIComponent(fileName)
  const { data } = await api.get<Blob>(`/adminexpensives/${id}/attachments/${encoded}`, {
    responseType: 'blob',
  })
  const url = window.URL.createObjectURL(data)
  const link = document.createElement('a')
  link.href = url
  link.download = fileName
  document.body.appendChild(link)
  link.click()
  link.remove()
  window.URL.revokeObjectURL(url)
}

export async function exportExpensivesToExcel(): Promise<void> {
  const { data } = await api.get<Blob>('/adminexpensives/export', {
    responseType: 'blob',
  })
  const url = window.URL.createObjectURL(data)
  const link = document.createElement('a')
  link.href = url
  link.download = `Expenses_${new Date().toISOString().replace(/[-:T]/g, '').slice(0, 15)}.xlsx`
  document.body.appendChild(link)
  link.click()
  link.remove()
  window.URL.revokeObjectURL(url)
}
