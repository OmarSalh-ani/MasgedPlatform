export interface CircleListItem {
  id: number
  name: string
  studentsCount: number
  teacherName: string
  createdAt: string
  createdBy: string
  teacherId: number | null
  forGirls: boolean
}

export interface CircleDetail {
  id: number
  name: string
  teacherId: number | null
  forGirls: boolean
}

export interface CircleTeacherOption {
  id: number
  name: string
}

export interface SaveCirclePayload {
  name: string
  teacherId: number | null
  forGirls: boolean
}

export function formatCircleCreatedAt(value: string): string {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  const day = String(date.getDate()).padStart(2, '0')
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const year = date.getFullYear()
  return `${day}/${month}/${year}`
}

export function getCirclesEmptyMessage(hasSearch: boolean): string {
  if (hasSearch) return 'لا توجد نتائج مطابقة للبحث'
  return 'لا توجد حلقات متاحة'
}
