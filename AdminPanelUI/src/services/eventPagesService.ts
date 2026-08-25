import api from '@/lib/axios'
import type { ApiResponse, PagedResult } from '@/types/api'
import type {
  EventPage,
  EventPageListItem,
  EventPageLookup,
  SaveEventPagePayload,
} from '@/types/eventPage'

function toFormData(payload: SaveEventPagePayload): FormData {
  const formData = new FormData()
  formData.append('activityName', payload.activityName)
  formData.append('slug', payload.slug)
  formData.append('courseTitle', payload.courseTitle)
  if (payload.invitationText) formData.append('invitationText', payload.invitationText)
  if (payload.mosqueName) formData.append('mosqueName', payload.mosqueName)
  if (payload.subjectText) formData.append('subjectText', payload.subjectText)
  if (payload.dateText) formData.append('dateText', payload.dateText)
  if (payload.timeText) formData.append('timeText', payload.timeText)
  if (payload.extraNotes) formData.append('extraNotes', payload.extraNotes)
  if (payload.supervisorsText) formData.append('supervisorsText', payload.supervisorsText)
  if (payload.contactPhone) formData.append('contactPhone', payload.contactPhone)
  if (payload.socialAccounts) formData.append('socialAccounts', payload.socialAccounts)
  if (payload.locationNote) formData.append('locationNote', payload.locationNote)
  formData.append('isPublished', String(payload.isPublished))
  formData.append('isRegistrationOpen', String(payload.isRegistrationOpen))
  formData.append('tracksJson', JSON.stringify(payload.tracks))
  formData.append('fieldsJson', JSON.stringify(payload.formFields))
  if (payload.imageFile) formData.append('image', payload.imageFile)
  return formData
}

export async function getEventPages(): Promise<EventPageListItem[]> {
  const { data } = await api.get<PagedResult<EventPageListItem>>('/admineventpages', {
    params: { pageNumber: 1, pageSize: 0 },
  })
  return data.items
}

export async function getEventPageLookups(): Promise<EventPageLookup[]> {
  const { data } = await api.get<ApiResponse<EventPageLookup[]>>('/admineventpages/lookups')
  return data.data
}

export async function getEventPage(id: number): Promise<EventPage> {
  const { data } = await api.get<ApiResponse<EventPage>>(`/admineventpages/${id}`)
  return data.data
}

export async function createEventPage(payload: SaveEventPagePayload): Promise<EventPage> {
  const { data } = await api.post<ApiResponse<EventPage>>(
    '/admineventpages',
    toFormData(payload),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.data
}

export async function updateEventPage(
  id: number,
  payload: SaveEventPagePayload,
): Promise<EventPage> {
  const { data } = await api.put<ApiResponse<EventPage>>(
    `/admineventpages/${id}`,
    toFormData(payload),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  )
  return data.data
}

export async function deleteEventPage(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/admineventpages/${id}`)
  return data.data
}
