import publicApi from '@/lib/publicAxios'
import type { ApiResponse } from '@/types/api'
import type { SubmitSubscribePayload, SubmitSubscribeResponse } from '@/types/subscribe'

export async function submitSubscribe(
  payload: SubmitSubscribePayload,
): Promise<SubmitSubscribeResponse> {
  const { data } = await publicApi.post<ApiResponse<SubmitSubscribeResponse>>(
    '/adminsubscribe',
    {
      fullName: payload.fullName,
      mobile: payload.mobile,
    },
  )
  return data.data
}
