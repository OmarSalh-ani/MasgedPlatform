import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  createStudentPlan,
  deleteStudentPlanItem,
  getAyahsBySurah,
  getEditPrefill,
  getStudentPlanDetail,
  getStudentPlanFormData,
  resolveStudentPlan,
  saveStudentPlan,
  updateStudentPlanItem,
} from '@/services/studentPlanService'
import type {
  CreateStudentPlanPayload,
  SaveStudentPlanPayload,
  UpdateStudentPlanItemPayload,
} from '@/types/studentPlan'

export const STUDENT_PLAN_FORM_KEY = ['student-plan', 'form-data'] as const

export function studentPlanDetailKey(studentId: number, planId: number) {
  return ['student-plan', 'detail', studentId, planId] as const
}

export function useStudentPlanFormData() {
  return useQuery({
    queryKey: STUDENT_PLAN_FORM_KEY,
    queryFn: getStudentPlanFormData,
  })
}

export function useStudentPlanAyahs(surahId: number) {
  return useQuery({
    queryKey: ['student-plan', 'ayahs', surahId],
    queryFn: () => getAyahsBySurah(surahId),
    enabled: surahId > 0 && surahId <= 114,
  })
}

export function useStudentPlanDetail(studentId?: number, planId?: number) {
  return useQuery({
    queryKey: studentId && planId ? studentPlanDetailKey(studentId, planId) : ['student-plan', 'detail'],
    queryFn: () => getStudentPlanDetail(studentId!, planId!),
    enabled: !!studentId && !!planId,
  })
}

export function useStudentPlanMutations(studentId?: number, planId?: number) {
  const queryClient = useQueryClient()

  const invalidateDetail = () => {
    if (studentId && planId) {
      queryClient.invalidateQueries({ queryKey: studentPlanDetailKey(studentId, planId) })
    }
  }

  const createPlanMutation = useMutation({
    mutationFn: (payload: CreateStudentPlanPayload) => createStudentPlan(payload),
  })

  const saveMutation = useMutation({
    mutationFn: (payload: SaveStudentPlanPayload) => saveStudentPlan(payload),
    onSuccess: invalidateDetail,
  })

  const updateItemMutation = useMutation({
    mutationFn: (payload: UpdateStudentPlanItemPayload) => updateStudentPlanItem(payload),
    onSuccess: invalidateDetail,
  })

  const deleteItemMutation = useMutation({
    mutationFn: (editKey: string) => deleteStudentPlanItem(editKey),
    onSuccess: invalidateDetail,
  })

  return { createPlanMutation, saveMutation, updateItemMutation, deleteItemMutation }
}

export { resolveStudentPlan, getEditPrefill }
