import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type {
  CreateStudentPlanPayload,
  PlanRowInput,
  SaveStudentPlanPayload,
  StudentPlanAyah,
  StudentPlanDetail,
  StudentPlanEditPrefill,
  StudentPlanFormData,
  StudentPlanResolve,
  UpdateStudentPlanItemPayload,
} from '@/types/studentPlan'

export async function getStudentPlanFormData(): Promise<StudentPlanFormData> {
  const { data } = await api.get<ApiResponse<StudentPlanFormData>>('/adminstudentplan/form-data')
  return data.data
}

export async function getAyahsBySurah(surahId: number): Promise<StudentPlanAyah[]> {
  const { data } = await api.get<ApiResponse<StudentPlanAyah[]>>(`/adminstudentplan/ayahs/${surahId}`)
  return data.data
}

export async function resolveStudentPlan(
  studentId: number,
  planId?: number,
  edit?: string,
): Promise<StudentPlanResolve> {
  const { data } = await api.get<ApiResponse<StudentPlanResolve>>(
    `/adminstudentplan/students/${studentId}/resolve`,
    { params: { planId, edit } },
  )
  return data.data
}

export async function getStudentPlanDetail(
  studentId: number,
  planId: number,
): Promise<StudentPlanDetail> {
  const { data } = await api.get<ApiResponse<StudentPlanDetail>>(
    `/adminstudentplan/students/${studentId}/plans/${planId}`,
  )
  return data.data
}

export async function getEditPrefill(editKey: string): Promise<StudentPlanEditPrefill> {
  const { data } = await api.get<ApiResponse<StudentPlanEditPrefill>>('/adminstudentplan/edit-prefill', {
    params: { editKey },
  })
  return data.data
}

export async function createStudentPlan(payload: CreateStudentPlanPayload): Promise<number> {
  const { data } = await api.post<ApiResponse<{ planId: number }>>('/adminstudentplan/plans', payload)
  return data.data.planId
}

export async function saveStudentPlan(payload: SaveStudentPlanPayload): Promise<void> {
  await api.post('/adminstudentplan/save', payload)
}

export async function updateStudentPlanItem(payload: UpdateStudentPlanItemPayload): Promise<void> {
  await api.put('/adminstudentplan/items', payload)
}

export async function deleteStudentPlanItem(editKey: string): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/adminstudentplan/items/${encodeURIComponent(editKey)}`)
  return data.data
}

export function isValidPlanRow(row: PlanRowInput): boolean {
  if (row.surahId <= 0) return false
  if (row.surahId > 1000) return true
  return row.fromAyahNumber > 0 && row.toAyahNumber > 0
}
