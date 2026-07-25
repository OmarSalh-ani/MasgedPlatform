import api from '@/lib/axios'
import type { ApiResponse, PagedResult } from '@/types/api'
import type {
  CreateHomeCirclePayload,
  HomeFilterOptions,
  HomeFilters,
  HomeListResponse,
  HomeStudentListItem,
  HomeStudentNameLookup,
  HomeStudentNameLookupFilters,
  HomeStudentReview,
  HomeStudentTest,
  TransferStudentsPayload,
} from '@/types/home'

export async function getOthaiminCenterFilterOptions(): Promise<HomeFilterOptions> {
  const { data } = await api.get<ApiResponse<HomeFilterOptions>>('/adminothaimincenter/filter-options')
  return data.data
}

export async function getOthaiminCenterStudentNames(
  filters: HomeStudentNameLookupFilters,
): Promise<PagedResult<HomeStudentNameLookup>> {
  const { data } = await api.get<PagedResult<HomeStudentNameLookup>>('/adminothaimincenter/student-names', { params: filters })
  return data
}

export async function getOthaiminCenterStudents(filters: HomeFilters): Promise<HomeListResponse> {
  const { data } = await api.get<PagedResult<HomeStudentListItem>>('/adminothaimincenter', { params: filters })
  return data
}

export async function getOthaiminCenterCircleTitle(circleId: number): Promise<string> {
  const { data } = await api.get<ApiResponse<string>>(`/adminothaimincenter/circle-title/${circleId}`)
  return data.data
}

export async function exportOthaiminCenterStudents(filters: Omit<HomeFilters, 'pageNumber' | 'pageSize'>): Promise<Blob> {
  const { data } = await api.get<Blob>('/adminothaimincenter/export/excel', {
    params: filters,
    responseType: 'blob',
  })
  return data
}

export async function sendOthaiminCenterWhatsapp(payload: {
  studentIds: number[]
  message: string
  image?: File | null
}): Promise<string> {
  const formData = new FormData()
  formData.append('studentIds', payload.studentIds.join(','))
  formData.append('message', payload.message)
  if (payload.image) formData.append('image', payload.image)

  const { data } = await api.post<ApiResponse<string>>('/adminothaimincenter/whatsapp', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  })
  return data.message
}

export async function transferOthaiminCenterStudents(payload: TransferStudentsPayload): Promise<number> {
  const { data } = await api.post<ApiResponse<number>>('/adminothaimincenter/transfer', payload)
  return data.data
}

export async function createOthaiminCenterCircle(payload: CreateHomeCirclePayload): Promise<number> {
  const { data } = await api.post<ApiResponse<number>>('/adminothaimincenter/create-circle', payload)
  return data.data
}

export async function deleteOthaiminCenterStudent(id: number): Promise<void> {
  await api.delete(`/adminothaimincenter/${id}`)
}

export async function getOthaiminCenterStudentTests(id: number): Promise<HomeStudentTest[]> {
  const { data } = await api.get<ApiResponse<HomeStudentTest[]>>(`/adminothaimincenter/${id}/tests`)
  return data.data
}

export async function getOthaiminCenterStudentReviews(id: number): Promise<HomeStudentReview[]> {
  const { data } = await api.get<ApiResponse<HomeStudentReview[]>>(`/adminothaimincenter/${id}/reviews`)
  return data.data
}
