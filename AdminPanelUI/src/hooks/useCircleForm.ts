import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import axios from 'axios'
import { useNavigate } from 'react-router-dom'
import {
  createCircle,
  deleteCircle,
  getCircle,
  getCircleTeachers,
  updateCircle,
} from '@/services/circlesService'
import type { ApiResponse } from '@/types/api'
import type { SaveCirclePayload } from '@/types/circle'
import { CIRCLES_QUERY_KEY } from '@/hooks/useCircles'

export const CIRCLE_TEACHERS_QUERY_KEY = ['circle', 'teachers'] as const

function getMutationErrorMessage(error: unknown, fallback: string): string {
  if (axios.isAxiosError(error)) {
    const body = error.response?.data as ApiResponse<unknown> | undefined
    if (body?.errors?.length) return body.errors.join('\n')
    if (body?.message && body.message !== 'Validation failed') return body.message
  }
  if (error instanceof Error) return error.message
  return fallback
}

export function useCircleForm(circleId?: number) {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const isEdit = circleId !== undefined

  const circleQuery = useQuery({
    queryKey: ['circle', circleId],
    queryFn: () => getCircle(circleId!),
    enabled: isEdit,
  })

  const teachersQuery = useQuery({
    queryKey: CIRCLE_TEACHERS_QUERY_KEY,
    queryFn: getCircleTeachers,
  })

  const invalidateAndGoToList = () => {
    queryClient.invalidateQueries({ queryKey: CIRCLES_QUERY_KEY })
    navigate('/circles')
  }

  const saveMutation = useMutation({
    mutationFn: (payload: SaveCirclePayload) =>
      isEdit ? updateCircle(circleId!, payload) : createCircle(payload),
    onSuccess: invalidateAndGoToList,
  })

  const deleteMutation = useMutation({
    mutationFn: () => deleteCircle(circleId!),
    onSuccess: invalidateAndGoToList,
  })

  return {
    isEdit,
    circleQuery,
    teachersQuery,
    saveMutation,
    deleteMutation,
    getSaveErrorMessage: (error: unknown) =>
      getMutationErrorMessage(error, 'حدث خطأ أثناء حفظ البيانات'),
    getDeleteErrorMessage: (error: unknown) =>
      getMutationErrorMessage(error, 'حدث خطأ أثناء حذف الحلقة'),
  }
}
