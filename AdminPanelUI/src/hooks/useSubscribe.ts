import { useMutation } from '@tanstack/react-query'
import axios from 'axios'
import { submitSubscribe } from '@/services/subscribeService'
import type { ApiResponse } from '@/types/api'
import type { SubmitSubscribePayload } from '@/types/subscribe'

function getSubmitErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const body = error.response?.data as ApiResponse<unknown> | undefined
    if (body?.errors?.length) return body.errors.join('\n')
    if (body?.message && body.message !== 'Validation failed') return body.message
  }
  if (error instanceof Error) return error.message
  return 'حدث خطأ أثناء التسجيل'
}

export function useSubscribe() {
  const submitMutation = useMutation({
    mutationFn: (payload: SubmitSubscribePayload) => submitSubscribe(payload),
  })

  return { submitMutation, getSubmitErrorMessage }
}
