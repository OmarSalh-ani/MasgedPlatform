import type { PagedResult } from '@/types/api'

export const HOME_FORM_STATUS_OPTIONS = [
  { value: '', label: 'جميع الاستمارات' },
  { value: 'نعم', label: 'الاستمارات المكتملة' },
  { value: 'لا', label: 'الاستمارات غير المكتملة' },
] as const

export const HOME_STUDENT_NAME_LOOKUP_PAGE_SIZE = 20

export const HOME_PAGE_SIZE = 20

export const HOME_PAGE_SIZE_OPTIONS = [10, 20, 50, 100, 200, 500, 1000] as const

export interface HomeLookup {
  id: number
  name: string
}

export interface HomeStudentNameLookup {
  name: string
}

export interface HomeStudentNameLookupFilters {
  search?: string
  pageNumber: number
  pageSize: number
}

export interface HomeStudentListItem {
  id: number
  studentName: string
  fatherName: string
  fatherPhone: string
  fatherPhone2?: string | null
  studentPhone?: string | null
  studentGender: string
  age: number
  birthdate?: string | null
  createdAt?: string | null
  circleName: string
  quranCircleId?: number | null
  leaveCount: number
  womanActivityType: string
  learnCertificate?: string | null
  completeFollowup: string
  isSpecial: string
  isElite: string
  studentImage?: string | null
  planLevelName: string
}

export interface HomeFilterOptions {
  circles: HomeLookup[]
  transferCircles: HomeLookup[]
  teachers: HomeLookup[]
  womanActivityTypes: HomeLookup[]
}

export interface HomeFilters {
  studentName?: string
  ageFrom?: number
  ageTo?: number
  circleId?: number
  fatherMobile?: string
  womanActivityTypeId?: number
  formStatus?: string
  specialOnly?: boolean
  eliteOnly?: boolean
  boysOnly?: boolean
  girlsOnly?: boolean
  circleQuery?: number
  pageNumber: number
  pageSize: number
}

export interface HomeRegistrationSettings {
  menEnabled: boolean
  womenEnabled: boolean
  showControls: boolean
}

export interface HomeStudentTest {
  testName: string
  testType: string
  from: string
  to: string
  testDegree: string
  notes: string
}

export interface HomeStudentReview {
  reviewType: string
  createdAt: string
  testFrom: string
  testTo: string
  surahName: string
  isDone: string
  notes: string
  parentNotes: string
  isSaveDone: string
  displayNotes: string
}

export interface SelectedHomeStudent {
  id: number
  studentName: string
  fatherName: string
  fatherPhone: string
  circleName: string
}

export type HomeListResponse = PagedResult<HomeStudentListItem>

export function getHomeStudentKey(id: number): string {
  return String(id)
}

export interface TransferStudentsPayload {
  studentIds: number[]
  circleId: number
}

export interface RemoveFromCirclePayload {
  studentIds: number[]
}

export interface CreateHomeCirclePayload {
  circleName: string
  teacherId: number
  studentIds: number[]
}

export interface UpdateRegistrationPayload {
  forGirl: boolean
  enabled: boolean
}
