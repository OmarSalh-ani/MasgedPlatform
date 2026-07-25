import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type {
  MemorizationRevisionReport,
  MemorizationRevisionStudentPick,
} from '@/types/memorizationRevisionReport'

export async function getMemorizationRevisionStudents(): Promise<MemorizationRevisionStudentPick[]> {
  const { data } = await api.get<ApiResponse<MemorizationRevisionStudentPick[]>>(
    '/adminmemorizationrevisionreport/students',
  )
  return data.data
}

export async function getMemorizationRevisionReport(
  studentId: number,
): Promise<MemorizationRevisionReport> {
  const { data } = await api.get<ApiResponse<MemorizationRevisionReport>>(
    `/adminmemorizationrevisionreport/${studentId}`,
  )
  return data.data
}

export async function exportMemorizationRevisionReport(studentId: number): Promise<Blob> {
  const { data } = await api.get<Blob>(
    `/adminmemorizationrevisionreport/${studentId}/export`,
    { responseType: 'blob' },
  )
  return data
}

export async function exportCompletedSurahsReport(studentId: number): Promise<Blob> {
  const { data } = await api.get<Blob>(
    `/adminmemorizationrevisionreport/${studentId}/export-completed-surahs`,
    { responseType: 'blob' },
  )
  return data
}
