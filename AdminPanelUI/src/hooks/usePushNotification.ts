import { useMutation, useQuery } from '@tanstack/react-query'
import {
  getPushNotificationFilterOptions,
  getPushNotificationStudents,
  getPushNotificationTeachers,
  sendPushNotification,
} from '@/services/pushNotificationService'
import type { PushNotificationStudentFilters } from '@/types/pushNotification'

const LIST_KEY = ['push-notifications', 'students'] as const

export function usePushNotificationTeachers() {
  return useQuery({
    queryKey: ['push-notifications', 'teachers'],
    queryFn: getPushNotificationTeachers,
  })
}

export function usePushNotificationFilterOptions() {
  return useQuery({
    queryKey: ['push-notifications', 'filter-options'],
    queryFn: getPushNotificationFilterOptions,
  })
}

export function usePushNotificationStudents(appliedFilters: PushNotificationStudentFilters | null) {
  return useQuery({
    queryKey: [...LIST_KEY, appliedFilters],
    queryFn: () => getPushNotificationStudents(appliedFilters!),
    enabled: appliedFilters != null,
  })
}

export function useSendPushNotification() {
  return useMutation({ mutationFn: sendPushNotification })
}
