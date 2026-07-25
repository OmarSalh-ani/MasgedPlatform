import type { HomeFilters, HomeFilterOptions, HomeStudentListItem } from '@/types/home'

export const WHATSAPP_SENDER_PAGE_SIZE = 20
export const WHATSAPP_SENDER_PAGE_SIZE_OPTIONS = [10, 20, 50, 100, 200, 500, 1000] as const

export interface WhatsappSenderFormOption {
  id: number
  title: string
}

export interface WhatsappSenderFilterForm {
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

export type WhatsappSenderFilters = Omit<HomeFilters, 'womanActivityTypeId' | 'eliteOnly'>

export interface SelectedWhatsappSenderStudent {
  id: number
  studentName: string
  fatherName: string
  fatherPhone: string
  circleName: string
}

export type { HomeStudentListItem, HomeFilterOptions }

export function formatWhatsappSenderDate(value?: string | null): string {
  if (!value) return ''
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleDateString('en-GB')
}
