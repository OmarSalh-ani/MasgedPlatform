import { useMutation, useQuery } from '@tanstack/react-query'
import axios from 'axios'
import {
  getParentsFollowup,
  submitParentsFollowup,
} from '@/services/parentsFollowupService'
import type { ApiResponse } from '@/types/api'
import type { SaveParentsFollowupPayload } from '@/types/parentsFollowup'

function getSubmitErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const body = error.response?.data as ApiResponse<unknown> | undefined
    if (body?.errors?.length) return body.errors.join('\n')
    if (body?.message) return body.message
  }
  if (error instanceof Error) return error.message
  return 'حدث خطأ أثناء الحفظ'
}

export function useParentsFollowup(studentId: number | undefined) {
  const query = useQuery({
    queryKey: ['parentsFollowup', studentId],
    queryFn: () => getParentsFollowup(studentId!),
    enabled: studentId !== undefined && studentId > 0,
    retry: false,
  })

  const submitMutation = useMutation({
    mutationFn: (payload: SaveParentsFollowupPayload) =>
      submitParentsFollowup(studentId!, payload),
  })

  return { query, submitMutation, getSubmitErrorMessage }
}
