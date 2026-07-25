import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import axios from 'axios'
import {
  createPlanLevel,
  deletePlanLevel,
  getPlanLevels,
  updatePlanLevel,
} from '@/services/planLevelService'
import type { ApiResponse } from '@/types/api'
import type { SavePlanLevelPayload } from '@/types/planLevel'

export const PLAN_LEVELS_QUERY_KEY = ['planLevels'] as const

function getMutationErrorMessage(error: unknown, fallback: string): string {
  if (axios.isAxiosError(error)) {
    const body = error.response?.data as ApiResponse<unknown> | undefined
    if (body?.errors?.length) return body.errors.join('\n')
    if (body?.message && body.message !== 'Validation failed') return body.message
  }
  if (error instanceof Error) return error.message
  return fallback
}

export function usePlanLevels() {
  const queryClient = useQueryClient()

  const query = useQuery({
    queryKey: PLAN_LEVELS_QUERY_KEY,
    queryFn: getPlanLevels,
  })

  const saveMutation = useMutation({
    mutationFn: ({ id, payload }: { id?: number; payload: SavePlanLevelPayload }) =>
      id ? updatePlanLevel(id, payload) : createPlanLevel(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: PLAN_LEVELS_QUERY_KEY })
    },
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deletePlanLevel(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: PLAN_LEVELS_QUERY_KEY })
    },
  })

  return {
    query,
    saveMutation,
    deleteMutation,
    getSaveErrorMessage: (error: unknown) =>
      getMutationErrorMessage(error, 'تعذر حفظ المستوى. يرجى المحاولة مرة أخرى.'),
    getDeleteErrorMessage: (error: unknown) =>
      getMutationErrorMessage(error, 'تعذر حذف المستوى. يرجى المحاولة مرة أخرى.'),
  }
}
