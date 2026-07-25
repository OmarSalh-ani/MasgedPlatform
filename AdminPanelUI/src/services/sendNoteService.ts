import api from '@/lib/axios'
import type { ApiResponse, PagedResult } from '@/types/api'
import type {
  CreateSendNotePayload,
  SendNote,
  SendNoteListItem,
  TeacherOption,
  UpdateSendNotePayload,
} from '@/types/sendNote'
import { SEND_NOTES_PAGE_SIZE } from '@/types/sendNote'

export async function getSendNotesList(
  pageNumber: number,
): Promise<PagedResult<SendNoteListItem>> {
  const { data } = await api.get<PagedResult<SendNoteListItem>>('/adminteachersendnotes', {
    params: { pageNumber, pageSize: SEND_NOTES_PAGE_SIZE },
  })
  return data
}

export async function getSendNoteTeachers(): Promise<TeacherOption[]> {
  const { data } = await api.get<ApiResponse<TeacherOption[]>>('/adminteachersendnotes/teachers')
  return data.data
}

export async function getSendNote(id: number): Promise<SendNote> {
  const { data } = await api.get<ApiResponse<SendNote>>(`/adminteachersendnotes/${id}`)
  return data.data
}

export async function createSendNotes(payload: CreateSendNotePayload): Promise<void> {
  await api.post<ApiResponse<boolean>>('/adminteachersendnotes', payload)
}

export async function updateSendNote(
  id: number,
  payload: UpdateSendNotePayload,
): Promise<SendNote> {
  const { data } = await api.put<ApiResponse<SendNote>>(`/adminteachersendnotes/${id}`, payload)
  return data.data
}

export async function deleteSendNote(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/adminteachersendnotes/${id}`)
  return data.data
}
