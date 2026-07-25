import type { HomeFilters, HomeFilterOptions, HomeStudentListItem } from '@/types/home'

export type PushNotificationAudience = 'teachers' | 'parents'

export const PUSH_NOTIFICATION_PAGE_SIZE = 20

export const PUSH_NOTIFICATION_PAGE_SIZE_OPTIONS = [10, 20, 50, 100, 200, 500, 1000] as const

export interface PushNotificationTeacherOption {
  id: number
  name: string
}

export interface PushNotificationFilterForm {
  studentName: string
  ageFrom: string
  ageTo: string
  circleId: string
  fatherMobile: string
  formStatus: string
  specialOnly: boolean
  boysOnly: boolean
  girlsOnly: boolean
}

export type PushNotificationStudentFilters = Omit<HomeFilters, 'womanActivityTypeId' | 'eliteOnly'>

export interface SelectedPushNotificationStudent {
  id: number
  studentName: string
  fatherName: string
  fatherPhone: string
  circleName: string
}

export interface SendPushNotificationPayload {
  audience: PushNotificationAudience
  targetAll: boolean
  teacherIds: number[]
  studentIds: number[]
  title: string
  body: string
}

export interface SendPushNotificationResult {
  recipientsResolved: number
  recipientsWithoutTokens: number
  tokensAttempted: number
  successCount: number
  failureCount: number
}

export type { HomeStudentListItem, HomeFilterOptions }
