import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type { SpecialStudentsReport } from '@/types/specialStudentsReport'

export async function getSpecialStudentsReport(): Promise<SpecialStudentsReport> {
  const { data } = await api.get<ApiResponse<SpecialStudentsReport>>(
    '/adminspecialstudentsreport',
  )
  return data.data
}

export async function exportSpecialStudentsReport(): Promise<Blob> {
  const { data } = await api.get<Blob>('/adminspecialstudentsreport/export', {
    responseType: 'blob',
  })
  return data
}
