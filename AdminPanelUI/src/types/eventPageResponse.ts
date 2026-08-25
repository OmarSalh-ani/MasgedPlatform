export const EVENT_PAGE_RESPONSES_PAGE_SIZE = 20
export const EVENT_PAGE_RESPONSES_PAGE_SIZE_OPTIONS = [10, 20, 50, 100] as const

export interface EventPageResponseValue {
  fieldLabel: string
  value: string
}

export interface EventPageResponseListItem {
  id: number
  eventPageId: number
  activityName: string
  submittedAt: string
  values: EventPageResponseValue[]
}

export interface EventPageResponseFilters {
  activityName?: string
  pageNumber: number
  pageSize: number
}

export interface EventPageResponsesPage {
  items: EventPageResponseListItem[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
  fieldLabels: string[]
}
