import api from '@/lib/axios'
import type { ApiResponse, PagedResult } from '@/types/api'
import type { HomeFilterOptions, HomeStudentListItem } from '@/types/home'
import type {
  PushNotificationStudentFilters,
  PushNotificationTeacherOption,
  SendPushNotificationPayload,
  SendPushNotificationResult,
} from '@/types/pushNotification'

export async function getPushNotificationTeachers(): Promise<PushNotificationTeacherOption[]> {
  const { data } = await api.get<ApiResponse<PushNotificationTeacherOption[]>>(
    '/adminpushnotifications/teachers',
  )
  return data.data
}

export async function getPushNotificationStudents(
  filters: PushNotificationStudentFilters,
): Promise<PagedResult<HomeStudentListItem>> {
  const { data } = await api.get<PagedResult<HomeStudentListItem>>(
    '/adminpushnotifications/students',
    { params: filters },
  )
  return data
}

export async function getPushNotificationFilterOptions(): Promise<HomeFilterOptions> {
  const { data } = await api.get<ApiResponse<HomeFilterOptions>>(
    '/adminpushnotifications/students/filter-options',
  )
  return data.data
}

export async function sendPushNotification(
  payload: SendPushNotificationPayload,
): Promise<SendPushNotificationResult> {
  const { data } = await api.post<ApiResponse<SendPushNotificationResult>>(
    '/adminpushnotifications/send',
    payload,
  )
  return data.data
}
