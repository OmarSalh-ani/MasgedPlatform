export interface ExpensiveListItem {
  id: number
  reason: string
  createdAt: string
  createdBy: string
  supplier: string
  totalAmount: number
  forGirls: boolean
}

export interface ExpensiveSummary {
  totalCount: number
  totalAmount: number
  thisMonthCount: number
  thisMonthAmount: number
  averageAmount: number
}

export interface ExpensiveAttachment {
  fileName: string
  uploadDate: string
}

export interface Expensive {
  id: number
  reason: string
  totalAmount: number
  supplier: string
  notes: string | null
  createdAt: string
  attachments: ExpensiveAttachment[]
}

export interface SaveExpensivePayload {
  reason: string
  totalAmount: number
  supplier: string
  notes: string | null
  files?: File[]
}

export type ExpensiveFormMode = 'create' | 'edit' | 'view'

export function getExpensivesEmptyMessage(hasSearch: boolean): string {
  if (hasSearch) return 'لا توجد نتائج مطابقة للبحث'
  return 'لا توجد مصروفات متاحة'
}
