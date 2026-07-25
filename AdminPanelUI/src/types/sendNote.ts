export interface SendNoteListItem {
  id: number
  teacherName: string
  note: string
  createdAt: string
  isRead: boolean
  readTime: string | null
}

export interface SendNote {
  id: number
  teacherId: number
  teacherName: string
  note: string
}

export interface TeacherOption {
  id: number
  name: string
}

export interface CreateSendNotePayload {
  teacherIds: number[]
  note: string
}

export interface UpdateSendNotePayload {
  note: string
}

export const SEND_NOTES_PAGE_SIZE = 10

export function formatSendNoteDate(value: string): string {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  const y = date.getFullYear()
  const m = String(date.getMonth() + 1).padStart(2, '0')
  const d = String(date.getDate()).padStart(2, '0')
  const h = String(date.getHours()).padStart(2, '0')
  const min = String(date.getMinutes()).padStart(2, '0')
  return `${y}/${m}/${d} ${h}:${min}`
}
