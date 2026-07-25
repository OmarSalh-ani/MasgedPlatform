import api from '@/lib/axios'
import type { ApiResponse, PagedResult } from '@/types/api'
import type {
  CreateHomeCirclePayload,
  HomeFilterOptions,
  HomeFilters,
  HomeListResponse,
  HomeRegistrationSettings,
  HomeStudentListItem,
  HomeStudentNameLookup,
  HomeStudentNameLookupFilters,
  HomeStudentReview,
  HomeStudentTest,
  RemoveFromCirclePayload,
  TransferStudentsPayload,
  UpdateRegistrationPayload,
} from '@/types/home'

export async function getHomeFilterOptions(): Promise<HomeFilterOptions> {
  const { data } = await api.get<ApiResponse<HomeFilterOptions>>('/adminhome/filter-options')
  return data.data
}

export async function getHomeStudentNames(
  filters: HomeStudentNameLookupFilters,
): Promise<PagedResult<HomeStudentNameLookup>> {
  const { data } = await api.get<PagedResult<HomeStudentNameLookup>>('/adminhome/student-names', { params: filters })
  return data
}

export async function getHomeStudents(filters: HomeFilters): Promise<HomeListResponse> {
  const { data } = await api.get<PagedResult<HomeStudentListItem>>('/adminhome', { params: filters })
  return data
}

export async function getHomeCircleTitle(circleId: number): Promise<string> {
  const { data } = await api.get<ApiResponse<string>>(`/adminhome/circle-title/${circleId}`)
  return data.data
}

export async function exportHomeStudents(filters: Omit<HomeFilters, 'pageNumber' | 'pageSize'>): Promise<Blob> {
  const { data } = await api.get<Blob>('/adminhome/export/excel', {
    params: filters,
    responseType: 'blob',
  })
  return data
}

export async function sendHomeWhatsapp(payload: {
  studentIds: number[]
  message: string
  image?: File | null
}): Promise<string> {
  const formData = new FormData()
  formData.append('studentIds', payload.studentIds.join(','))
  formData.append('message', payload.message)
  if (payload.image) formData.append('image', payload.image)

  const { data } = await api.post<ApiResponse<string>>('/adminhome/whatsapp', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  })
  return data.message
}

export async function transferHomeStudents(payload: TransferStudentsPayload): Promise<number> {
  const { data } = await api.post<ApiResponse<number>>('/adminhome/transfer', payload)
  return data.data
}

export async function removeHomeStudentsFromCircle(payload: RemoveFromCirclePayload): Promise<number> {
  const { data } = await api.post<ApiResponse<number>>('/adminhome/remove-from-circle', payload)
  return data.data
}

export async function createHomeCircle(payload: CreateHomeCirclePayload): Promise<number> {
  const { data } = await api.post<ApiResponse<number>>('/adminhome/create-circle', payload)
  return data.data
}

export async function deleteHomeStudent(id: number): Promise<void> {
  await api.delete(`/adminhome/${id}`)
}

export async function getHomeStudentTests(id: number): Promise<HomeStudentTest[]> {
  const { data } = await api.get<ApiResponse<HomeStudentTest[]>>(`/adminhome/${id}/tests`)
  return data.data
}

export async function getHomeStudentReviews(id: number): Promise<HomeStudentReview[]> {
  const { data } = await api.get<ApiResponse<HomeStudentReview[]>>(`/adminhome/${id}/reviews`)
  return data.data
}

export async function getHomeStudentQrToken(id: number): Promise<string> {
  const { data } = await api.get<ApiResponse<{ token: string }>>(`/adminhome/${id}/qr-token`)
  return data.data.token
}

export async function getHomeRegistrationSettings(): Promise<HomeRegistrationSettings> {
  const { data } = await api.get<ApiResponse<HomeRegistrationSettings>>('/adminhome/registration-settings')
  return data.data
}

export async function updateHomeRegistrationSettings(payload: UpdateRegistrationPayload): Promise<void> {
  await api.put('/adminhome/registration-settings', payload)
}
